using System;
using System.IO;
using System.Threading.Tasks;
using Tessa.Platform.ConsoleApps;
using Tessa.Platform.Data;
using Tessa.Scheme;

namespace Tessa.Extensions.Default.Console.SchemeScript
{
    public static class Operation
    {
        public static async Task<int> ExecuteAsync(
            IConsoleLogger logger,
            TextWriter stdOut,
            string source,
            string target,
            Dbms dbms,
            Version dbmsv,
            bool withoutTransactions)
        {
            if (dbmsv == null)
            {
                switch (dbms)
                {
                    case Dbms.SqlServer:
                        dbmsv = new Version(10, 50, 0, 0);
                        break;

                    case Dbms.PostgreSql:
                        dbmsv = new Version(9, 6, 0, 0);
                        break;

                    default: throw new NotSupportedException();
                }
            }

            var sourcePath = DefaultConsoleHelper.GetSourceFiles(source, "*.tsd")[0];

            await logger.InfoAsync($"Generating scheme script for: {sourcePath}");

            StreamWriter streamWriter = null;

            try
            {
                if (target != null)
                    streamWriter = new StreamWriter(target);

                var partitions = FileSchemeService.GetPartitionPaths(sourcePath);
                var fileService = new FileSchemeService(sourcePath, partitions);

                var database = new SchemeDatabase(Guid.Empty, DatabaseNames.Applied, false, true);
                var fakeService = new FakeSchemeService(streamWriter ?? stdOut, !withoutTransactions, dbms, dbmsv);

                database.Refresh(fileService);
                fakeService.CreateStorage();
                database.SubmitChanges(fakeService);
            }
            finally
            {
                streamWriter?.Dispose();
            }

            return 0;
        }
    }
}