using System;
using System.IO;
using System.Threading.Tasks;
using Tessa.Localization;
using Tessa.Platform.ConsoleApps;
using Tessa.Platform.IO;
using Tessa.Scheme;
using Tessa.Scheme.Differences;

namespace Tessa.Extensions.Default.Console.SchemeDiff
{
    public static class Operation
    {
        #region Private Methods

        private static SchemeDatabase OpenDatabase(string source)
        {
            string[] partitions = FileSchemeService.GetPartitionPaths(source);
            FileSchemeService fileSchemeService = new FileSchemeService(source, partitions);
            SchemeDatabase tessaDatabase = new SchemeDatabase(DatabaseNames.Original);

            tessaDatabase.Refresh(fileSchemeService);

            return tessaDatabase;
        }

        private static void RestoreForegroundColor(ConsoleColor color)
        {
            System.Console.ForegroundColor = color;
        }

        private static void SetForegroundColor(ConsoleColor color, out ConsoleColor previousColor)
        {
            previousColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
        }

        private static void WriteObject(SchemeObject obj, IndentedTextWriter writer)
        {
            writer.Write(obj.GetDisplayName());

            if (obj is SchemeNamedObject named)
            {
                writer.Write(", ID: ");
                writer.Write(named.ID);
            }
        }

        private static void WritePropertyDifference(SchemePropertyDifference propDifference, IndentedTextWriter writer)
        {
            writer.WriteLine();
            writer.Write(propDifference.Property);
            writer.WriteLine(':');
            writer.Indent();

            var scalarDifference = propDifference as SchemeScalarPropertyDifference;
            ConsoleColor previousColor;

            if (scalarDifference != null)
            {
                SetForegroundColor(ConsoleColor.Yellow, out previousColor);

                writer.Write("X: ");
                writer.Write(scalarDifference.Left);
                writer.WriteLine();
                writer.Write("Y: ");
                writer.Write(scalarDifference.Right);

                RestoreForegroundColor(previousColor);
            }

            if (propDifference is SchemeCollectionDifference objCollectionDifference)
            {
                writer.Write("Added:");
                writer.Indent();

                SetForegroundColor(ConsoleColor.Green, out previousColor);

                foreach (var obj in objCollectionDifference.Added)
                {
                    writer.WriteLine();
                    WriteObject(obj, writer);
                }

                RestoreForegroundColor(previousColor);

                writer.Unindent();

                writer.WriteLine();
                writer.Write("Removed:");
                writer.Indent();

                SetForegroundColor(ConsoleColor.Red, out previousColor);

                foreach (var obj in objCollectionDifference.Removed)
                {
                    writer.WriteLine();
                    WriteObject(obj, writer);
                }

                RestoreForegroundColor(previousColor);

                writer.Unindent();

                writer.WriteLine();
                writer.Write("Modified:");
                writer.Indent();

                foreach (var objDifference in objCollectionDifference.Modified)
                {
                    writer.WriteLine();

                    SetForegroundColor(ConsoleColor.Yellow, out previousColor);
                    WriteObject(objDifference.Right, writer);
                    RestoreForegroundColor(previousColor);

                    writer.Indent();

                    foreach (var nestedDifference in objDifference.Modified)
                    {
                        WritePropertyDifference(nestedDifference, writer);
                    }

                    writer.Unindent();
                }

                writer.Unindent();
            }

            writer.Unindent();
        }

        #endregion

        #region Static Methods

        public static async Task<int> ExecuteAsync(
            IConsoleLogger logger,
            TextWriter stdOut,
            string sourceA,
            string sourceB)
        {
            string sourcePathA = DefaultConsoleHelper.GetSourceFiles(sourceA, "*.tsd")[0];
            string sourcePathB = DefaultConsoleHelper.GetSourceFiles(sourceB, "*.tsd")[0];

            await logger.InfoAsync("Evaluating difference between \"{0}\" and \"{1}\"", sourceA, sourceB);

            SchemeDatabase databaseA = OpenDatabase(sourcePathA);
            SchemeDatabase databaseB = OpenDatabase(sourcePathB);
            SchemeObjectDifference difference = SchemeObjectDifference.GetDifferenceIfExists(databaseA, databaseB, false);

            await logger.InfoAsync("Difference is evaluated, printing it to output", sourceA, sourceB);

            if (difference == null)
            {
                stdOut.WriteLine(LocalizationManager.GetString("Scheme_CLI_DatabasesAreEqual"));
            }
            else
            {
                await using IndentedTextWriter writer = new IndentedTextWriter(stdOut);
                foreach (SchemePropertyDifference propDifference in difference.Modified)
                {
                    WritePropertyDifference(propDifference, writer);
                }
            }

            return 0;
        }

        #endregion
    }
}
