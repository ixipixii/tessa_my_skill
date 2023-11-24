using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Tessa.Platform.ConsoleApps;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Scheme;

namespace Tessa.Extensions.Default.Console.SchemeRename
{
    public static class Operation
    {
        public static async Task<int> ExecuteAsync(
            IConsoleLogger logger,
            TextWriter stdOut,
            string source,
            string tableName,
            string columnName,
            string target,
            Dbms dbms)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Table name isn't specified", "tableName");
            }

            if (string.IsNullOrEmpty(columnName))
            {
                throw new ArgumentException("Column name isn't specified", "columnName");
            }

            string filePath = DefaultConsoleHelper.GetSourceFiles(source, "*.tsd")[0];
            await logger.InfoAsync("Generating rename script for the scheme: \"{0}\"", filePath);

            var partitions = FileSchemeService.GetPartitionPaths(source);
            var service = new FileSchemeService(source, partitions, SchemeServiceOptions.ReadOnly);

            var table = service.GetTable(tableName);
            if (table == null)
            {
                throw new InvalidOperationException($"Can't find table \"{tableName}\".");
            }

            if (!table.Columns.TryGetItem(columnName, out SchemeColumn _))
            {
                throw new InvalidOperationException($"Can't find column \"{columnName}\" in table \"{tableName}\".");
            }

            string identifierColumnName = table.ContentType <= SchemeTableContentType.Entries
                ? Names.Table_ID_ID
                : Names.Table_RowID;

            TextWriter fileOutput = null;

            try
            {
                IQueryBuilder builder;
                if (target == null)
                {
                    builder = new StreamQueryBuilder(stdOut, dbms);
                }
                else
                {
                    fileOutput = new StreamWriter(target);
                    builder = new StreamQueryBuilder(fileOutput, dbms);
                }

                string tempTable = "RenamingValues";

                builder
                    .CreateTempTable(ref tempTable, b => b
                         .C(identifierColumnName).Type(DbType.Guid).Q(" NOT NULL PRIMARY KEY,").N()
                         .C(columnName).Type(DbType.String).Q(" NOT NULL")).Z()

                    .InsertInto(tempTable, identifierColumnName, columnName)
                    .Values(b => b.V(Session.SystemID).V(Session.SystemName)).Z()

                    .InsertInto(tempTable, identifierColumnName, columnName)
                    .Values(b => b.V(new Guid(0x3DB19FA0, 0x228A, 0x497F, 0x87, 0x3A, 0x02, 0x50, 0xBF, 0x0A, 0x4C, 0xCB)).V("Admin")).Z()

                    .Update(tableName)
                        .C(columnName).Assign().C("r", columnName)
                        .From(tempTable, "r").NoLock()
                    .Where().C(tableName, identifierColumnName).Equals().C("r", identifierColumnName)
                        .And().C(tableName, columnName).NotEquals().C("r", columnName).Z();

                // в каждой невиртуальной таблице, в т.ч. в таблице table (для рекурсивных связей)
                foreach (var referencingTable in table.GetReferencingTables())
                {
                    if (referencingTable != table && !referencingTable.IsVirtual)
                    {
                        // для каждой комплексной колонки, которая ссылается на таблицу table, кроме системных колонок ID
                        // и в которой есть колонка вида Reference+ColumnName
                        foreach (var candidateColumn in referencingTable.Columns)
                        {
                            if (candidateColumn.IsSystem)
                                continue;

                            var complexColumn = candidateColumn as SchemeComplexColumn; // User
                            string targetTableName = referencingTable.Name;
                            string targetColumnName;                                   // UserName

                            if (complexColumn != null &&
                                complexColumn.ReferencedTable == table &&
                                complexColumn.Columns.Contains(complexColumn.Name + identifierColumnName) &&
                                complexColumn.Columns.Contains(targetColumnName = complexColumn.Name + columnName))
                            {
                                // пишем скрипт, обновляющий колонку targetColumnName в таблице t
                                builder
                                    .Update(targetTableName)
                                        .C(targetColumnName).Assign().C("r", columnName)
                                        .From(tempTable, "r").NoLock()
                                    .Where().C(targetTableName, identifierColumnName).Equals().C("r", identifierColumnName)
                                        .And().C(targetTableName, columnName).NotEquals().C("r", columnName).Z();
                            }
                        }
                    }
                }

                builder.DropTable(tempTable).Z();
            }
            finally
            {
                if (fileOutput != null)
                    fileOutput.Dispose();
            }

            return 0;
        }
    }
}
