using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Tessa.Platform.ConsoleApps;
using Tessa.Scheme;

namespace Tessa.Extensions.Default.Console.SchemeCompact
{
    public static class Operation
    {
        public static async Task<int> ExecuteAsync(
            IConsoleLogger logger,
            TextWriter stdOut,
            string source,
            string target)
        {
            string filePath = DefaultConsoleHelper.GetSourceFiles(source, "*.tsd")[0];
            await logger.InfoAsync($"Compacting the scheme into a single file: \"{filePath}\"");

            var partitions = FileSchemeService.GetPartitionPaths(filePath);
            var service = new FileSchemeService(filePath, partitions);
            var database = new SchemeDatabase(DatabaseNames.Original);

            database.Refresh(service);

            StreamWriter streamWriter = null;
            XmlWriter xmlWriter = null;
            SchemeSerializationScope scope = null;

            try
            {
                if (target != null)
                {
                    streamWriter = new StreamWriter(target);
                }

                xmlWriter = XmlWriter.Create(streamWriter ?? stdOut, FileSchemeService.XmlWriterSettings);
                scope = new SchemeSerializationScope();

                database.WriteXml(xmlWriter);
            }
            finally
            {
                scope?.Dispose();
                xmlWriter?.Close();
                streamWriter?.Dispose();
            }

            await logger.InfoAsync("Scheme has been compacted");
            return 0;
        }
    }
}
