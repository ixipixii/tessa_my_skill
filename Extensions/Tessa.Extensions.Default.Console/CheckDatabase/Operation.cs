using System.Threading;
using System.Threading.Tasks;
using Tessa.Platform.ConsoleApps;
using Tessa.Platform.Data;

namespace Tessa.Extensions.Default.Console.CheckDatabase
{
    public static class Operation
    {
        public static async Task<int> ExecuteAsync(
            IConsoleLogger logger,
            string configurationString,
            int timeoutSeconds,
            bool outputDbms,
            bool outputConnectionString,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(configurationString))
            {
                await logger.InfoAsync("Checking database connection using default configuration string");
            }
            else
            {
                await logger.InfoAsync("Checking database connection using configuration string \"{0}\"", configurationString);
            }

            if (timeoutSeconds < 0)
            {
                await logger.InfoAsync("Connecting with timeout specified in a connection string");
            }
            else
            {
                await logger.InfoAsync("Connecting with timeout {0} second(s)", timeoutSeconds);
            }

            Dbms dbms = await DefaultConsoleHelper.CheckConnectionAsync(logger, configurationString, timeoutSeconds, outputConnectionString, cancellationToken);

            if (outputDbms)
            {
                switch (dbms)
                {
                    case Dbms.SqlServer:
                        await logger.WriteLineAsync("ms");
                        break;

                    case Dbms.PostgreSql:
                        await logger.WriteLineAsync("pg");
                        break;
                }
            }

            await logger.InfoAsync("Database connection check is successful");
            return 0;
        }
    }
}