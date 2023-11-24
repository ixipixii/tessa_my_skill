using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Platform;
using Tessa.Platform.ConsoleApps;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Default.Console.ImportCards
{
    public sealed class Operation : ConsoleOperation<OperationContext>
    {
        #region FailedImportingResult Private Class

        private sealed class FailedImportingResult
        {
            #region Constructors

            public FailedImportingResult(
                CardStoreResponse response,
                string cardName,
                string fileName,
                bool isSingleton)
            {
                this.Response = response;
                this.CardName = cardName;
                this.FileName = fileName;
                this.IsSingleton = isSingleton;
            }

            public CardStoreResponse Response { get; }

            public string CardName { get; }

            public string FileName { get; }

            public bool IsSingleton { get; }

            #endregion
        }

        #endregion

        #region Constructors

        public Operation(
            ConsoleSessionManager sessionManager,
            IConsoleLogger logger,
            ICardManager cardManager,
            ICardRepository cardRepository)
            : base(logger, sessionManager, extendedInitialization: true)
        {
            this.cardManager = cardManager;
            this.cardRepository = cardRepository;
        }

        #endregion

        #region Fields

        private readonly ICardManager cardManager;

        private readonly ICardRepository cardRepository;

        #endregion

        #region Private Constants

        private const string ImportingAddOperationPrefix = "UI_Cards_ImportingAddOperationPrefix";

        private const string DeletingAddOperationPrefix = "UI_Cards_DeletingAddOperationPrefix";

        #endregion

        #region Private Methods

        private async Task<bool> ImportFilesAsync(
            bool canDeleteExistentCards,
            bool ignoreExistentCards,
            bool ignoreRepairMessages,
            CancellationToken cancellationToken,
            params string[] fileNames)
        {
            IValidationResultBuilder validationResult = new ValidationResultBuilder();
            var successfulCardNames = new List<string>(fileNames.Length);

            var failedResults = new List<FailedImportingResult>();
            var nextFileNames = new List<string>(fileNames);

            try
            {
                while (nextFileNames.Count > 0)
                {
                    failedResults.Clear();

                    foreach (string fileName in nextFileNames)
                    {
                        string cardName = GetImportingCardName(fileName);
                        CardStoreResponse response = await this.ImportCardCoreAsync(fileName, cancellationToken);

                        ValidationResult result = response.ValidationResult.Build();
                        if (!canDeleteExistentCards || !AddFailedImportingResult(result, failedResults, response, cardName, fileName))
                        {
                            DefaultConsoleHelper.AddOperationToValidationResult(
                                ImportingAddOperationPrefix,
                                cardName,
                                result,
                                validationResult,
                                ignoreExistentCards,
                                ignoreRepairMessages);

                            if (result.IsSuccessful)
                            {
                                successfulCardNames.Add(cardName);
                                await this.Logger.InfoAsync("Card is imported: {0}", cardName);
                            }
                            else
                            {
                                await this.Logger.InfoAsync("Card is failed to import: {0}", cardName);
                            }
                        }
                    }

                    nextFileNames.Clear();

                    if (failedResults.Count > 0)
                    {
                        failedResults.Reverse();

                        foreach (FailedImportingResult importingResult in failedResults)
                        {
                            ValidationResult result = await this.DeleteCardCoreAsync(importingResult, cancellationToken);

                            DefaultConsoleHelper.AddOperationToValidationResult(
                                DeletingAddOperationPrefix,
                                importingResult.CardName,
                                result,
                                validationResult,
                                false,
                                ignoreRepairMessages);

                            if (result.IsSuccessful)
                            {
                                nextFileNames.Add(importingResult.FileName);
                                await this.Logger.InfoAsync("Card is deleted: {0}", importingResult.CardName);
                            }
                            else
                            {
                                await this.Logger.InfoAsync("Card is failed to delete: {0}", importingResult.CardName);
                            }
                        }

                        nextFileNames.Reverse();
                    }
                }
            }
            catch (Exception ex)
            {
                validationResult.AddException(this, ex);
            }

            if (successfulCardNames.Count != 0)
            {
                validationResult = GetImportingResultWithPreamble(validationResult, successfulCardNames);
            }

            ValidationResult totalResult = validationResult.Build();
            await this.Logger.LogResultAsync(totalResult);

            return totalResult.IsSuccessful;
        }


        private static string GetImportingCardName(string fileName)
        {
            // в качестве имени выводим полностью относительный путь к файлу, чтобы было подробно залогировано
            // return Path.GetFileNameWithoutExtension(fileName);

            return fileName;
        }


        private static IValidationResultBuilder GetImportingResultWithPreamble(
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
                    "UI_Cards_CardImported",
                    "UI_Cards_MultipleCardsImported",
                    successfulCardNames)
                .ToString();

            return new ValidationResultBuilder()
                .AddInfo(typeof(Operation), infoText)
                .Add(validationResult);
        }


        private static bool AddFailedImportingResult(
            ValidationResult result,
            List<FailedImportingResult> failedResults,
            CardStoreResponse response,
            string cardName,
            string fileName)
        {
            // response может прийти фейковый, в котором есть только ValidationResult с каким-то исключением
            // и обращение к другим его свойствам приведёт к ошибке

            foreach (ValidationResultItem item in result.Items)
            {
                if (CardValidationKeys.IsCardExists(item.Key))
                {
                    failedResults.Add(
                        new FailedImportingResult(
                            response,
                            cardName,
                            fileName,
                            isSingleton: item.Key == CardValidationKeys.SingletonExists));

                    return true;
                }
            }

            return false;
        }


        private async Task<CardStoreResponse> ImportCardCoreAsync(string source, CancellationToken cancellationToken = default)
        {
            CardStoreResponse response;

            try
            {
                string extension = Path.GetExtension(source);
                CardFileFormat format = CardHelper.TryParseCardFileFormatFromExtension(extension) ?? CardFileFormat.Json;

                await using FileStream fileStream = File.OpenRead(source);
                response = await this.cardManager.ImportAsync(fileStream, format: format, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                response = new CardStoreResponse();
                response.ValidationResult.AddException(this, ex);
            }

            return response;
        }


        private async Task<ValidationResult> DeleteCardCoreAsync(
            FailedImportingResult importingResult,
            CancellationToken cancellationToken = default)
        {
            CardStoreResponse response = importingResult.Response;

            if (importingResult.IsSingleton)
            {
                // удаляется любая карточка-синглтон, а не только та, которую пытаемся импортировать
                return (await this.DeleteCardCoreAsync(null, response.CardTypeID, cancellationToken)).Build();
            }

            Guid[] deletedCards = response.TryGetDeletedCards();

            if (deletedCards == null)
            {
                // удаляется обычная карточка
                return (await this.DeleteCardCoreAsync(response.CardID, response.CardTypeID, cancellationToken)).Build();
            }

            if (deletedCards.Length == 1)
            {
                // удаляется единственная удалённая карточка
                return (await this.DeleteCardCoreAsync(deletedCards[0], CardHelper.DeletedTypeID, cancellationToken)).Build();
            }

            // удаляются все удалённые карточки
            IValidationResultBuilder result = new ValidationResultBuilder();
            foreach (Guid deletedID in deletedCards)
            {
                result.Add(
                    await this.DeleteCardCoreAsync(deletedID, CardHelper.DeletedTypeID, cancellationToken));
            }

            return result.Build();
        }


        private async Task<IValidationResultBuilder> DeleteCardCoreAsync(
            Guid? cardID,
            Guid cardTypeID,
            CancellationToken cancellationToken = default)
        {
            var request = new CardDeleteRequest
            {
                DeletionMode = CardDeletionMode.WithoutBackup,
                CardID = cardID,
                CardTypeID = cardTypeID,
            };

            CardDeleteResponse response = await this.cardRepository.DeleteAsync(request, cancellationToken);
            return response.ValidationResult;
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
                // если указана папка, то находим первый файл с подходящим расширением
                bool hasCardLibraries = false;
                foreach (string source in DefaultConsoleHelper.GetSourceFiles(context.Source, "*.cardlib", throwIfNotFound: false, checkPatternMatch: true))
                {
                    hasCardLibraries = true;
                    await this.Logger.InfoAsync("Reading card library from: \"{0}\"", source);

                    var library = new CardLibrary();
                    await using (FileStream fileStream = File.OpenRead(source))
                    {
                        library.DeserializeFromXml(fileStream);
                    }

                    if (library.Items.Count == 0)
                    {
                        await this.Logger.InfoAsync("There are no files in the card library");
                        continue;
                    }

                    string folder = Path.GetDirectoryName(source);
                    if (string.IsNullOrEmpty(folder))
                    {
                        folder = Directory.GetCurrentDirectory();
                    }

                    string[] fileNames = library.Items
                        .Select(relativePath => Path.Combine(folder, relativePath.Path).NormalizePathOnCurrentPlatform())
                        .ToArray();

                    await this.Logger.InfoAsync(
                        "Importing cards ({0}) in the card library:{1}{2}",
                        fileNames.Length,
                        Environment.NewLine,
                        string.Join(Environment.NewLine, fileNames.Select(x => "\"" + x + "\"")));

                    if (!await this.ImportFilesAsync(
                        context.CanDeleteExistentCards,
                        context.IgnoreExistentCards,
                        context.IgnoreRepairMessages,
                        cancellationToken,
                        fileNames))
                    {
                        return -1;
                    }
                }

                if (!hasCardLibraries)
                {
                    bool foundFiles = false;
                    foreach (string source in DefaultConsoleHelper.GetSourceFiles(context.Source, "*.*", throwIfNotFound: false))
                    {
                        string extension = Path.GetExtension(source);
                        CardFileFormat? format = CardHelper.TryParseCardFileFormatFromExtension(extension);

                        if (format.HasValue)
                        {
                            foundFiles = true;

                            await this.Logger.InfoAsync("Importing card from: \"{0}\"", source);

                            if (!await this.ImportFilesAsync(
                                context.CanDeleteExistentCards,
                                context.IgnoreExistentCards,
                                context.IgnoreRepairMessages,
                                cancellationToken,
                                source))
                            {
                                return -1;
                            }
                        }
                    }

                    if (!foundFiles)
                    {
                        throw new FileNotFoundException($"Couldn't locate *.cardlib, *.jcard or *.card files in \"{context.Source}\"", context.Source);
                    }
                }
            }
            catch (Exception e)
            {
                await this.Logger.LogExceptionAsync("Error importing cards", e);
                return -1;
            }

            await this.Logger.InfoAsync("Cards are imported successfully");
            return 0;
        }

        #endregion
    }
}
