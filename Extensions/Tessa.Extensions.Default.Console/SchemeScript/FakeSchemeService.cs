using System;
using System.Collections.Generic;
using System.IO;
using Tessa.Platform.Data;
using Tessa.Platform.Data.Fake;
using Tessa.Platform.Data.Fake.Npgsql;
using Tessa.Platform.Data.Fake.SqlClient;
using Tessa.Scheme;

namespace Tessa.Extensions.Default.Console.SchemeScript
{
    public sealed class FakeSchemeService : SystemSchemeService
    {
        #region Fields

        private readonly TextWriter output;
        private readonly Version dbmsVersion;
        private readonly DatabaseSchemeService service;
        private DatabaseSchemeService.KnownTables tables;

        #endregion

        #region Constructors

        public FakeSchemeService(TextWriter output, bool outputTransactions, Dbms dbms, Version dbmsVersion)
            : base(SchemeServiceOptions.None)
        {
            this.output = output;
            this.dbmsVersion = dbmsVersion;
            FakeConnection connection = CreateConnection(dbms, output, outputTransactions, executor => executor
                .Query(DatabaseSchemeService.GetDbmsVersionQuery(dbms), this.GetDbmsVersion)
                .Query(DatabaseSchemeService.TableExistsQuery(dbms), this.TableExists)
                .Execute(DatabaseSchemeService.SaveTableQuery(dbms, false), this.SaveTable)
                .Execute(DatabaseSchemeService.RemoveTableQuery(dbms), this.RemoveTable));
            this.service = new DatabaseSchemeService(() => connection, null, () => new DateTime(2014, 08, 21));
        }

        private FakeDataReader GetDbmsVersion(FakeCommand command) =>
            new FakeDataReader(new object[,] { { this.dbmsVersion.ToString() } });

        private FakeDataReader TableExists(FakeCommand command)
        {
            var table = ToTable((string) command.Parameters[0].Value);
            return new FakeDataReader(new object[,] { { (this.tables & table) == table } });
        }

        private int SaveTable(FakeCommand command)
        {
            this.tables |= ToTable((Guid) command.Parameters[0].Value);
            return 1;
        }

        private int RemoveTable(FakeCommand command)
        {
            this.tables &= ~ToTable((Guid) command.Parameters[0].Value);
            return 1;
        }

        #endregion

        #region SystemSchemeService Overrides

        #region Database Members

        protected override void SaveDatabasePropertiesOverride(SchemeDatabaseProperties properties)
        {
            WriteComment(this.output, properties.Name);
            this.service.SaveDatabaseProperties(properties);
        }

        protected override SchemeDatabaseProperties GetDatabasePropertiesOverride()
        {
            return this.service.GetDatabaseProperties();
        }

        #endregion

        #region Partition Members

        protected override SchemePartition GetPartitionOverride(Guid id)
        {
            return this.service.GetPartition(id);
        }

        protected override SchemePartition GetPartitionOverride(string name)
        {
            return this.service.GetPartition(name);
        }

        protected override IEnumerable<SchemePartition> GetPartitionsOverride()
        {
            return this.service.GetPartitions();
        }

        protected override void SavePartitionOverride(SchemePartition partition)
        {
            WriteComment(this.output, partition.Name);
            this.service.SavePartition(partition);
        }

        protected override bool RemovePartitionOverride(SchemePartition partition)
        {
            WriteComment(this.output, partition.Name);
            return this.service.RemovePartition(partition);
        }

        #endregion

        #region Table Members

        protected override SchemeTable GetTableOverride(Guid id)
        {
            return this.service.GetTable(id);
        }

        protected override SchemeTable GetTableOverride(string name)
        {
            return this.service.GetTable(name);
        }

        protected override IEnumerable<SchemeTable> GetTablesOverride()
        {
            return this.service.GetTables();
        }

        protected override void SaveTableOverride(SchemeTable table)
        {
            WriteComment(this.output, table.Name);
            this.tables |= ToTable(table.ID);
            this.service.SaveTable(table);
        }

        protected override bool RemoveTableOverride(SchemeTable table)
        {
            WriteComment(this.output, table.Name);
            this.tables &= ~ToTable(table.ID);
            return this.service.RemoveTable(table);
        }

        #endregion

        #region Procedure Members

        protected override SchemeProcedure GetProcedureOverride(Guid id)
        {
            return this.service.GetProcedure(id);
        }

        protected override SchemeProcedure GetProcedureOverride(string name)
        {
            return this.service.GetProcedure(name);
        }

        protected override IEnumerable<SchemeProcedure> GetProceduresOverride()
        {
            return this.service.GetProcedures();
        }

        protected override void SaveProcedureOverride(SchemeProcedure procedure)
        {
            WriteComment(this.output, procedure.Name);
            this.service.SaveProcedure(procedure);
        }

        protected override bool RemoveProcedureOverride(SchemeProcedure procedure)
        {
            WriteComment(this.output, procedure.Name);
            return this.service.RemoveProcedure(procedure);
        }

        #endregion

        #region Function Members

        protected override SchemeFunction GetFunctionOverride(Guid id)
        {
            return this.service.GetFunction(id);
        }

        protected override SchemeFunction GetFunctionOverride(string name)
        {
            return this.service.GetFunction(name);
        }

        protected override IEnumerable<SchemeFunction> GetFunctionsOverride()
        {
            return this.service.GetFunctions();
        }

        protected override void SaveFunctionOverride(SchemeFunction function)
        {
            WriteComment(this.output, function.Name);
            this.service.SaveFunction(function);
        }

        protected override bool RemoveFunctionOverride(SchemeFunction function)
        {
            WriteComment(this.output, function.Name);
            return this.service.RemoveFunction(function);
        }

        #endregion

        #region Migration Members

        protected override SchemeMigration GetMigrationOverride(Guid id)
        {
            return this.service.GetMigration(id);
        }

        protected override SchemeMigration GetMigrationOverride(string name)
        {
            return this.service.GetMigration(name);
        }

        protected override IEnumerable<SchemeMigration> GetMigrationsOverride()
        {
            return this.service.GetMigrations();
        }

        protected override void SaveMigrationOverride(SchemeMigration migration)
        {
            WriteComment(this.output, migration.Name);
            this.service.SaveMigration(migration);
        }

        protected override bool RemoveMigrationOverride(SchemeMigration migration)
        {
            WriteComment(this.output, migration.Name);
            return this.service.RemoveMigration(migration);
        }

        #endregion

        #region Storage Members

        public override bool IsStorageExists => this.service.IsStorageExists;

        protected override void CreateStorageOverride() => this.service.CreateStorage();

        public override Version StorageVersion => this.service.StorageVersion;

        protected override void UpdateStorageOverride() => this.service.UpdateStorage();

        protected override void InitializeOverride()
        {
        }

        protected override SchemeOperationScope CreateOperationScopeOverride(string operation, SchemeObject obj) => null;

        #endregion

        #region Cache Members

        public override void InvalidateCache() => this.service.InvalidateCache();

        public override bool InvalidateCacheIfChanged() => this.service.InvalidateCacheIfChanged();

        #endregion

        #endregion

        #region Methods

        private static FakeConnection CreateConnection(Dbms dbms, TextWriter output, bool outputTransactions, Action<FakeCommandExecutor> defineCommands)
        {
            switch (dbms)
            {
                case Dbms.SqlServer:
                    return new FakeSqlConnection(defineCommands, output) { OutputTransactions = outputTransactions };

                case Dbms.PostgreSql:
                    return new FakeNpgsqlConnection(defineCommands, output) { OutputTransactions = outputTransactions };

                default:
                    throw new NotSupportedException();
            }
        }

        private static DatabaseSchemeService.KnownTables ToTable(string name)
        {
            switch (name)
            {
                case Names.Configuration: return DatabaseSchemeService.KnownTables.Configuration;
                case Names.Scheme: return DatabaseSchemeService.KnownTables.Scheme;
                case Names.Partitions: return DatabaseSchemeService.KnownTables.Partitions;
                case Names.Tables: return DatabaseSchemeService.KnownTables.Tables;
                case Names.Procedures: return DatabaseSchemeService.KnownTables.Procedures;
                case Names.Functions: return DatabaseSchemeService.KnownTables.Functions;
                case Names.Migrations: return DatabaseSchemeService.KnownTables.Migrations;
                default: return DatabaseSchemeService.KnownTables.None;
            }
        }

        private static DatabaseSchemeService.KnownTables ToTable(Guid id)
        {
            if (id == SchemeGuids.Configuration) return DatabaseSchemeService.KnownTables.Configuration;
            if (id == SchemeGuids.Scheme) return DatabaseSchemeService.KnownTables.Scheme;
            if (id == SchemeGuids.Partitions) return DatabaseSchemeService.KnownTables.Partitions;
            if (id == SchemeGuids.Tables) return DatabaseSchemeService.KnownTables.Tables;
            if (id == SchemeGuids.Procedures) return DatabaseSchemeService.KnownTables.Procedures;
            if (id == SchemeGuids.Functions) return DatabaseSchemeService.KnownTables.Functions;
            if (id == SchemeGuids.Migrations) return DatabaseSchemeService.KnownTables.Migrations;
            return DatabaseSchemeService.KnownTables.None;
        }

        private static void WriteComment(TextWriter textWriter, string comment)
        {
            for (int i = 0; i < 80; i++)
                textWriter.Write('-');

            textWriter.WriteLine();
            textWriter.Write('-');
            textWriter.Write('-');
            textWriter.Write(' ');
            textWriter.WriteLine(comment);

            for (int i = 0; i < 80; i++)
                textWriter.Write('-');

            textWriter.WriteLine();
        }

        #endregion
    }
}