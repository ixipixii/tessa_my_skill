using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Localization;
using Tessa.Platform;
using Tessa.Platform.ConsoleApps;
using Tessa.Platform.IO;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Console.ExportCards
{
    public sealed class Operation :
        ConsoleOperation<OperationContext>
    {
        #region Constructors

        public Operation(
            ConsoleSessionManager sessionManager,
            IConsoleLogger logger,
            ICardRepository cardRepository,
            ICardManager cardManager)
            : base(logger, sessionManager, extendedInitialization: true)
        {
            this.cardRepository = cardRepository;
            this.cardManager = cardManager;
        }

        #endregion

        #region Fields

        private readonly ICardRepository cardRepository;

        private readonly ICardManager cardManager;

        #endregion

        #region Private Constants

        private const string ExportingAddOperationPrefix = "UI_Cards_ExportingAddOperationPrefix";

        #endregion

        #region Private Methods

        private async Task<(bool result, List<string> successfulFileNames)> ExportCardsAsync(
            List<CardInfo> cardInfoList,
            string outputFolder,
            bool binaryFormat,
            CancellationToken cancellationToken = default)
        {
            CardFileFormat format = binaryFormat ? CardFileFormat.Binary : CardFileFormat.Json;

            IValidationResultBuilder validationResult = new ValidationResultBuilder();
            var successfulCardNames = new List<string>(cardInfoList.Count);
            var successfulFileNames = new List<string>(cardInfoList.Count);

            try
            {
                Guid?[] cardTypeIDList = await this.cardRepository.GetTypeIDListAsync(
                    cardInfoList.Select(x => x.CardID).ToArray(), cancellationToken: cancellationToken);

                for (int i = 0; i < cardInfoList.Count; i++)
                {
                    CardInfo cardInfo = cardInfoList[i];
                    Guid? cardTypeID = cardTypeIDList[i];

                    string cardName = cardInfo.CardName;
                    string fileName = CardHelper.GetCardFileNameWithoutExtension(cardName) + (binaryFormat ? ".card" : ".jcard");
                    string filePath = fileName;

                    if (!string.IsNullOrEmpty(outputFolder))
                    {
                        filePath = Path.Combine(outputFolder, fileName);
                    }

                    CardGetResponse response = await this.ExportCardCoreAsync(filePath, cardInfo.CardID, cardTypeID, format, cancellationToken);

                    ValidationResult result = response.ValidationResult.Build();
                    if (result.IsSuccessful)
                    {
                        successfulCardNames.Add(cardName);
                        successfulFileNames.Add(fileName);
                    }
                    else
                    {
                        FileHelper.DeleteFileSafe(filePath);

                        DefaultConsoleHelper.AddOperationToValidationResult(
                            ExportingAddOperationPrefix,
                            cardName,
                            result,
                            validationResult);
                    }
                }
            }
            catch (Exception ex)
            {
                validationResult.AddException(this, ex);
            }

            if (successfulCardNames.Count != 0)
            {
                validationResult = GetExportingResultWithPreamble(validationResult, successfulCardNames);
            }

            ValidationResult totalResult = validationResult.Build();
            await this.Logger.LogResultAsync(totalResult);

            return (totalResult.IsSuccessful, successfulFileNames);
        }


        private static IValidationResultBuilder GetExportingResultWithPreamble(
            IValidationResultBuilder validationResult,
            ICollection<string> successfulCardNames)
        {
            if (successfulCardNames.Count == 0)
            {
                return validationResult;
            }

            string infoText =
                DefaultConsoleHelper.GetQuotedItemsText(
                        new StringBuilder(),
                        "UI_Cards_CardExported",
                        "UI_Cards_MultipleCardsExported",
                        successfulCardNames)
                    .ToString();

            return new ValidationResultBuilder()
                .AddInfo(typeof(Operation), infoText)
                .Add(validationResult);
        }


        private async Task<CardGetResponse> ExportCardCoreAsync(
            string fileName,
            Guid cardID,
            Guid? cardTypeID,
            CardFileFormat format,
            CancellationToken cancellationToken = default)
        {
            CardGetResponse response;

            try
            {
                var request = new CardGetRequest
                {
                    CardID = cardID,
                    CardTypeID = cardTypeID,
                    CompressionMode = CardCompressionMode.Full,
                    RestrictionFlags = CardGetRestrictionValues.ExportAll,
                };

                await using FileStream fileStream = File.Create(fileName);
                response = await this.cardManager.ExportAsync(request, fileStream, format: format, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                response = new CardGetResponse();
                response.ValidationResult.AddException(this, ex);
            }

            return response;
        }


        private static void CreateFoldersAndGetPaths(
            OperationContext context,
            out string subfolderPartPathInLibrary,
            out string cardFilesOutputFolder,
            out string libraryFilePath)
        {
            string outputFolder = context.OutputFolder?.Trim().NormalizePathOnCurrentPlatform();
            string cardLibraryPath = context.CardLibraryPath?.Trim().NormalizePathOnCurrentPlatform();

            libraryFilePath = cardLibraryPath;
            if (!string.IsNullOrEmpty(libraryFilePath))
            {
                if (!Path.GetFileName(libraryFilePath).Contains("."))
                {
                    // если есть любое расширение, кроме cardlib, или просто точка на конце - сохраняем расширение
                    libraryFilePath += ".cardlib";
                }
            }

            string cardLibraryDirectory = cardLibraryPath;
            if (!string.IsNullOrEmpty(cardLibraryDirectory))
            {
                cardLibraryDirectory = cardLibraryDirectory == "."
                    ? null
                    : Path.GetDirectoryName(cardLibraryDirectory);
            }

            // в библиотеках карточек путь прописывается с обратным слэшом \ независимо от ОС
            subfolderPartPathInLibrary = outputFolder == "." ? null : outputFolder?.Replace('/', '\\');

            cardFilesOutputFolder = string.IsNullOrEmpty(cardLibraryDirectory)
                ? outputFolder
                : string.IsNullOrEmpty(outputFolder)
                    ? cardLibraryDirectory
                    : Path.Combine(cardLibraryDirectory, outputFolder);

            if (cardFilesOutputFolder == ".")
            {
                cardFilesOutputFolder = null;
            }
            else if (!string.IsNullOrEmpty(cardFilesOutputFolder)
                && !Directory.Exists(cardFilesOutputFolder))
            {
                Directory.CreateDirectory(cardFilesOutputFolder);
            }
        }

        #endregion

        #region Base Overrides

        /// <inheritdoc />
        public override async Task<int> ExecuteAsync(OperationContext context, CancellationToken cancellationToken = default)
        {
            if (!this.SessionManager.IsOpened)
            {
                return -1;
            }

            try
            {
                if (context.CardInfoList.Count == 0)
                {
                    await this.Logger.InfoAsync("No cards to export");
                    return 0;
                }

                DefaultConsoleHelper.RemoveCardInfoListDuplicates(context.CardInfoList);

                if (context.LocalizationCulture != null)
                {
                    foreach (CardInfo cardInfo in context.CardInfoList)
                    {
                        cardInfo.CardName = LocalizationManager.Format(cardInfo.CardName, context.LocalizationCulture);
                    }
                }

                CreateFoldersAndGetPaths(
                    context,
                    out string subfolderPartPathInLibrary,
                    out string cardFilesOutputFolder,
                    out string libraryFilePath);

                await this.Logger.InfoAsync(
                    "Exporting cards ({0}):{1}{2}",
                    context.CardInfoList.Count,
                    Environment.NewLine,
                    string.Join(Environment.NewLine, context.CardInfoList.Select(x => "\"" + x.CardName + "\"")));

                (bool result, List<string> successfulFileNames) = await this.ExportCardsAsync(
                    context.CardInfoList, cardFilesOutputFolder, context.BinaryFormat, cancellationToken);

                if (!result)
                {
                    return -1;
                }

                if (successfulFileNames.Count > 0 && !string.IsNullOrEmpty(libraryFilePath))
                {
                    var library = new CardLibrary();

                    if (File.Exists(libraryFilePath))
                    {
                        await this.Logger.InfoAsync("Appending to existent card library: \"{0}\"", libraryFilePath);

                        await using FileStream fileStream = File.OpenRead(libraryFilePath);
                        library.DeserializeFromXml(fileStream);
                    }
                    else
                    {
                        await this.Logger.InfoAsync("Creating card library: \"{0}\"", libraryFilePath);
                    }

                    bool hasChanges = false;
                    foreach (string fileName in successfulFileNames)
                    {
                        // в библиотеках карточек путь прописывается с обратным слэшом \ независимо от ОС
                        string filePath = string.IsNullOrEmpty(subfolderPartPathInLibrary)
                            ? fileName
                            : subfolderPartPathInLibrary + "\\" + fileName;

                        if (library.Items.Any(x => x.Path == filePath))
                        {
                            await this.Logger.InfoAsync("Skipping card: \"{0}\"", filePath);
                        }
                        else
                        {
                            await this.Logger.InfoAsync("Adding card: \"{0}\"", filePath);
                            library.Items.Add(new CardLibraryItem(filePath));
                            hasChanges = true;
                        }
                    }

                    if (hasChanges)
                    {
                        await this.Logger.InfoAsync("Saving card library");

                        // чтобы при ошибке сериализации случайно не перезаписать файл с библиотекой - сериализуем в памяти
                        string xmlText = library.SerializeToXml();

                        await File.WriteAllTextAsync(libraryFilePath, xmlText, Encoding.UTF8, cancellationToken);
                    }
                    else
                    {
                        await this.Logger.InfoAsync("Card library isn't changed");
                    }
                }
            }
            catch (Exception e)
            {
                await this.Logger.LogExceptionAsync("Error exporting cards", e);
                return -1;
            }

            await this.Logger.InfoAsync("Cards are exported successfully");
            return 0;
        }

        #endregion
    }
}