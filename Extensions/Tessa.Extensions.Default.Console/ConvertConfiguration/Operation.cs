using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Json;
using Tessa.Platform.ConsoleApps;
using Tessa.Platform.IO;
using Tessa.Platform.Json;

namespace Tessa.Extensions.Default.Console.ConvertConfiguration
{
    public sealed class Operation :
        ConsoleOperation<OperationContext>
    {
        #region ConversionItem Private Class

        private sealed class ConversionItem
        {
            public ConversionItem(string oldPath, string newPath, object obj)
            {
                this.OldPath = oldPath;
                this.NewPath = newPath;
                this.Object = obj;
            }

            public string OldPath { get; }

            public string NewPath { get; }

            public object Object { get; }
        }

        #endregion

        #region Constructors

        public Operation(
            ConsoleSessionManager sessionManager,
            IConsoleLogger logger)
            : base(logger, sessionManager)
        {
        }

        #endregion

        #region Private Methods

        private static async Task ConvertCardAsync(string oldPath, string newPath, CancellationToken cancellationToken = default)
        {
            await using FileStream sourceStream = File.OpenRead(oldPath);
            await using FileStream targetStream = File.Create(newPath);
            var cardReader = new CardReader(sourceStream);
            CardHeader header = await cardReader.ReadHeaderAsync(cancellationToken);
            CardStoreRequest request = await cardReader.ReadCardStoreRequestAsync(cancellationToken);

            var result = new List<object> { request.GetStorage() };

            foreach (CardHeaderFile headerFile in header.GetOrderedFiles())
            {
                await using SubStream contentStream = cardReader.ReadStream(headerFile.Size);
                byte[] bytes = await contentStream.ReadAllBytesAsync(cancellationToken);

                result.Add(
                    new Dictionary<string, object>(StringComparer.Ordinal)
                    {
                        { CardComponentHelper.ContentFileIDKey, headerFile.ID },
                        { CardComponentHelper.ContentFileSizeKey, headerFile.Size },
                        { CardComponentHelper.ContentFileDataKey, bytes },
                    });
            }

            await using var writer = new StreamWriter(targetStream, Encoding.UTF8, FileHelper.DefaultAsyncBufferSize, leaveOpen: true);
            using var jsonWriter = new JsonTextWriter(writer);
            jsonWriter.Formatting = Formatting.Indented;
            TessaSerializer.JsonTyped.Serialize(jsonWriter, result);
        }


        private static string ChangeExtension(string path, string oldExtension, string newExtension) =>
            path?.EndsWith(oldExtension, StringComparison.OrdinalIgnoreCase) == true
                ? path.Substring(0, path.Length - oldExtension.Length) + newExtension
                : path;


        private static void ConvertCardLibraryItems(CardLibrary cardLibrary)
        {
            foreach (CardLibraryItem item in cardLibrary.Items)
            {
                item.Path = ChangeExtension(item.Path, ".card", ".jcard");
            }
        }

        #endregion

        #region Base Overrides

        public override async Task<int> ExecuteAsync(OperationContext context, CancellationToken cancellationToken = default)
        {
            await this.Logger.InfoAsync("Converting configuration from: \"{0}\"", context.Source);

            try
            {
                var items = new List<ConversionItem>();
                foreach (string filePath in DefaultConsoleHelper.GetSourceFiles(context.Source, "*.*", throwIfNotFound: false))
                {
                    string extension = Path.GetExtension(filePath)?.ToLowerInvariant();
                    ConversionItem item = null;

                    try
                    {
                        switch (extension)
                        {
                            case ".tct":
                                await this.Logger.InfoAsync("Reading type from: \"{0}\"", filePath);

                                var cardType = new CardType();
                                await using (FileStream fileStream = File.OpenRead(filePath))
                                {
                                    cardType.DeserializeFromXml(fileStream);
                                }

                                string newTypeFilePath = ChangeExtension(filePath, ".tct", ".jtype");
                                item = new ConversionItem(filePath, newTypeFilePath, cardType);
                                break;

                            case ".cardlib":
                                await this.Logger.InfoAsync("Reading card library from: \"{0}\"", filePath);

                                var cardLibrary = new CardLibrary();
                                await using (FileStream fileStream = File.OpenRead(filePath))
                                {
                                    cardLibrary.DeserializeFromXml(fileStream);
                                }

                                item = new ConversionItem(filePath, filePath, cardLibrary);
                                break;

                            case ".card":
                                await this.Logger.InfoAsync("Card is pending to convert: \"{0}\"", filePath);
                                string newCardTypePath = ChangeExtension(filePath, ".card", ".jcard");
                                item = new ConversionItem(filePath, newCardTypePath, new Card());
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        await this.Logger.LogExceptionAsync($"Error when loading file \"{filePath}\"", ex);
                        item = null;
                    }

                    if (item != null)
                    {
                        items.Add(item);
                    }
                }

                if (items.Count > 0)
                {
                    await this.Logger.InfoAsync("Converting configuration files ({0})", items.Count);

                    foreach (ConversionItem item in items)
                    {
                        try
                        {
                            switch (item.Object)
                            {
                                case CardType cardType:
                                    string typeText = cardType.SerializeToJson(indented: true);
                                    await File.WriteAllTextAsync(item.NewPath, typeText, Encoding.UTF8, cancellationToken);
                                    await this.Logger.InfoAsync("Type is converted: \"{0}\"", item.NewPath);
                                    break;

                                case CardLibrary cardLibrary:
                                    ConvertCardLibraryItems(cardLibrary);
                                    string libraryText = cardLibrary.SerializeToXml();
                                    await File.WriteAllTextAsync(item.NewPath, libraryText, Encoding.UTF8, cancellationToken);
                                    await this.Logger.InfoAsync("Card library is converted: \"{0}\"", item.NewPath);
                                    break;

                                case Card _:
                                    await ConvertCardAsync(item.OldPath, item.NewPath, cancellationToken);
                                    await this.Logger.InfoAsync("Card is converted: \"{0}\"", item.NewPath);
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            await this.Logger.LogExceptionAsync($"Error when converting file \"{item.OldPath}\"", ex);
                        }

                        if (item.NewPath != item.OldPath)
                        {
                            FileHelper.DeleteFileSafe(item.OldPath);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                await this.Logger.LogExceptionAsync("Error converting configuration", e);
                return -1;
            }

            await this.Logger.InfoAsync("Configuration is converted successfully");
            return 0;
        }

        #endregion
    }
}
