using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tessa.Applications;
using Tessa.Applications.Package;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Json;
using Tessa.Platform;
using Tessa.Platform.ConsoleApps;
using Tessa.Platform.IO;
using Tessa.Platform.Json;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;

namespace Tessa.Extensions.Default.Console.PackageApp
{
    public static class Operation
    {
        #region Private Methods

        private static async Task LoadInfoAsync(IConsoleLogger logger, Application app, string exePath, string icoPath)
        {
            await logger.InfoAsync("Packaging application: {0}", exePath);

            RuntimeHelper.GetApplicationInfoForDefaultApps(
                exePath,
                out string name,
                out string alias,
                out Version version,
                out bool knownApp);

            if (!knownApp)
            {
                await logger.InfoAsync("Application is unknown, trying to load its assembly");

                try
                {
                    Assembly appAssembly = Assembly.LoadFrom(exePath);

                    RuntimeHelper.GetApplicationInfo(
                        appAssembly,
                        out string newName,
                        out string newAlias,
                        out Version newVersion);

                    name = newName;
                    alias = newAlias;
                    version = newVersion;
                }
                catch (Exception ex)
                {
                    await logger.LogExceptionAsync("Failed to load assembly, falling back to default alias and name", ex);
                }
            }

            if (string.IsNullOrWhiteSpace(app.Alias))
            {
                if (string.IsNullOrWhiteSpace(alias))
                {
                    alias = RuntimeHelper.ApplicationDefaultAlias;
                }

                app.Alias = alias;
            }

            if (string.IsNullOrWhiteSpace(app.Name))
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = "Application";
                }

                app.Name = name;
            }

            if (string.IsNullOrWhiteSpace(app.Group))
            {
                app.Group = string.Empty;
            }

            // эти свойства точно не null, но могут содержать оконечные пробелы
            app.Alias = app.Alias.Trim();
            app.Name = app.Name.Trim();
            app.Group = app.Group.Trim();

            app.Version = version.ToString();
            app.ExeFileName = Path.GetFileName(exePath);

            string appFolder = Path.GetDirectoryName(Path.GetFullPath(exePath));
            if (string.IsNullOrEmpty(appFolder))
            {
                appFolder = Directory.GetCurrentDirectory();
            }

            await logger.InfoAsync("Packaging from folder: {0}", appFolder);
            await logger.InfoAsync("ExeFileName = {0}", app.ExeFileName);
            await logger.InfoAsync("Version = {0}", app.Version);
            await logger.InfoAsync("Name = {0}", app.Name);
            await logger.InfoAsync("Group = {0}", string.IsNullOrEmpty(app.Group) ? "(empty)" : app.Group);
            await logger.InfoAsync("Alias = {0}", app.Alias);
            await logger.InfoAsync("Admin = {0}", app.Admin);

            if (string.IsNullOrWhiteSpace(icoPath))
            {
                string iconFileNameWithoutExtension = Path.GetFileNameWithoutExtension(app.ExeFileName);
                icoPath = Path.Combine(appFolder, iconFileNameWithoutExtension + ".ico");

                if (iconFileNameWithoutExtension?.EndsWith("32", StringComparison.Ordinal) == true
                    && !File.Exists(icoPath))
                {
                    // для TessaClient32.exe - ищем иконку в TessaClient.ico, если не нашли TessaClient32.ico
                    icoPath = Path.Combine(appFolder, iconFileNameWithoutExtension.Substring(0, iconFileNameWithoutExtension.Length - 2) + ".ico");
                }
            }
            else if (!Path.IsPathRooted(icoPath))
            {
                icoPath = Path.Combine(appFolder, icoPath);
            }

            await logger.InfoAsync("Loading icon from: {0}", icoPath);

            if (File.Exists(icoPath))
            {
                try
                {
                    app.Icon = RuntimeHelper.TryGetRecommendedIconDataFromIcoFile(icoPath);

                    if (app.Icon == null)
                    {
                        await logger.InfoAsync("No suitable icon was found in file, packaging without it");
                    }
                }
                catch (Exception ex)
                {
                    await logger.LogExceptionAsync("Failed to load icon, packaging without it", ex);
                }
            }
            else
            {
                await logger.InfoAsync("No icon found, packaging without it");
            }

            var ignoredProvider = new FileSystemIgnoredFilesProvider();
            var ignoredPathList = new HashSet<string>(ignoredProvider.GetIgnoredFileNames(appFolder));

            foreach (string path in ignoredPathList)
            {
                await logger.InfoAsync("Ignored: {0}", path);
            }

            foreach (string filePath in Directory.EnumerateFiles(appFolder, "*.*", SearchOption.AllDirectories))
            {
                if (!ignoredPathList.Contains(filePath))
                {
                    var file = new ApplicationFile(filePath, appFolder);
                    app.Files.Add(file);

                    if (string.IsNullOrEmpty(file.Category))
                    {
                        await logger.InfoAsync("Added: {0}", file.Name);
                    }
                    else
                    {
                        await logger.InfoAsync("Added: {0}\\{1}", file.Category, file.Name);
                    }
                }
            }
        }

        #endregion

        #region Methods

        public static async Task<int> ExecuteAsync(
            IConsoleLogger logger,
            string exePath,
            string outputPath,
            string icoPath,
            string alias,
            string name,
            string group,
            bool admin)
        {
            if (string.IsNullOrEmpty(exePath))
            {
                await logger.ErrorAsync("Can't package app: no source executable specified.");
                return -1;
            }

            try
            {
                // если File.Exists упадёт (например, невалидные символы в пути), то его перехватит catch
                exePath = Path.GetFullPath(exePath);
                if (!File.Exists(exePath))
                {
                    await logger.ErrorAsync("Can't find source executable \"{0}\". Please, check if file exists and application has access to it.", exePath);
                    return -2;
                }

                var app = new Application { Alias = alias, Name = name, Group = group, Admin = admin };
                await LoadInfoAsync(logger, app, exePath, icoPath);

                DateTime utcNow = DateTime.UtcNow;

                var card = new Card
                {
                    ID = Guid.NewGuid(),
                    TypeID = CardHelper.ApplicationTypeID,
                    TypeName = CardHelper.ApplicationTypeName,
                    TypeCaption = CardHelper.ApplicationTypeCaption,
                    Created = utcNow,
                    CreatedByID = Session.SystemID,
                    CreatedByName = Session.SystemName,
                    Modified = utcNow,
                    ModifiedByID = Session.SystemID,
                    ModifiedByName = Session.SystemName,
                };

                StringDictionaryStorage<CardSection> sections = card.Sections;
                sections.GetOrAddTable("ApplicationRoles");

                Dictionary<string, object> fields = sections.GetOrAddEntry("Applications").RawFields;
                fields["Alias"] = app.Alias;
                fields["AppVersion"] = app.Version;
                fields["ExecutableFileName"] = app.ExeFileName;
                fields["ExtensionVersion"] = "1.0.0.0";
                fields["ForAdmin"] = BooleanBoxes.Box(app.Admin);
                fields["GroupName"] = app.Group;
                fields["Icon"] = app.Icon;
                fields["Name"] = app.Name;
                fields["PlatformVersion"] = BuildInfo.Version;

                if (app.Files.Count > 0)
                {
                    ListStorage<CardFile> cardFiles = card.Files;
                    ISignatureProvider signatureProvider = SyncSignatureProvider.Files;

                    foreach (ApplicationFile file in app.Files)
                    {
                        Guid versionRowID = Guid.NewGuid();

                        CardFile cardFile = cardFiles.Add();
                        cardFile.RowID = file.RowID;
                        cardFile.TypeID = CardHelper.FileTypeID;
                        cardFile.TypeName = CardHelper.FileTypeName;
                        cardFile.TypeCaption = CardHelper.FileTypeCaption;
                        cardFile.VersionRowID = versionRowID;
                        cardFile.Name = file.Name;
                        cardFile.CategoryCaption = file.Category;
                        cardFile.Size = file.Size;
                        cardFile.Hash = await HashHelper.CalculateHashAsync(signatureProvider, new MemoryStream(file.Content));
                        cardFile.State = CardFileState.Inserted;

                        Card fileCard = cardFile.Card;
                        fileCard.ID = file.RowID;
                        fileCard.TypeID = CardHelper.FileTypeID;
                        fileCard.TypeName = CardHelper.FileTypeName;
                        fileCard.TypeCaption = CardHelper.FileTypeCaption;
                        fileCard.Created = utcNow;
                        fileCard.CreatedByID = Session.SystemID;
                        fileCard.CreatedByName = Session.SystemName;
                        fileCard.Modified = utcNow;
                        fileCard.ModifiedByID = Session.SystemID;
                        fileCard.ModifiedByName = Session.SystemName;
                    }
                }

                var request = new CardStoreRequest { Card = card, Method = CardStoreMethod.Import };
                request.SetImportVersion(1);

                var container = new List<object> { request.GetStorage() };

                foreach (ApplicationFile file in app.Files)
                {
                    container.Add(new Dictionary<string, object>(StringComparer.Ordinal)
                    {
                        { CardComponentHelper.ContentFileIDKey, file.RowID },
                        { CardComponentHelper.ContentFileSizeKey, file.Size },
                        { CardComponentHelper.ContentFileDataKey, file.Content },
                    });
                }

                const string outputExtension = ".jcard";
                if (string.IsNullOrWhiteSpace(outputPath) || outputPath == ".")
                {
                    outputPath = app.Alias + outputExtension;
                }
                else if (outputPath.EndsWith("/", StringComparison.Ordinal)
                    || outputPath.EndsWith("\\", StringComparison.Ordinal))
                {
                    outputPath = Path.Combine(outputPath.Substring(0, outputPath.Length - 1), app.Alias + outputExtension);
                }
                else if (!outputPath.EndsWith(outputExtension, StringComparison.OrdinalIgnoreCase)
                    && Directory.Exists(outputPath))
                {
                    outputPath = Path.Combine(outputPath, app.Alias + outputExtension);
                }

                await logger.InfoAsync("Writing package to: {0}", outputPath);

                string outputFolder = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputFolder) && !Directory.Exists(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                await using FileStream targetStream = File.Create(outputPath);
                await using var writer = new StreamWriter(targetStream, Encoding.UTF8, FileHelper.DefaultAsyncBufferSize, leaveOpen: true);
                using var jsonWriter = new JsonTextWriter(writer);
                jsonWriter.Formatting = Formatting.Indented;
                TessaSerializer.JsonTyped.Serialize(jsonWriter, container);
            }
            catch (Exception ex)
            {
                await logger.LogExceptionAsync("Error packaging application", ex);
                return -1;
            }

            await logger.InfoAsync("Packaging is completed");
            return 0;
        }

        #endregion
    }
}