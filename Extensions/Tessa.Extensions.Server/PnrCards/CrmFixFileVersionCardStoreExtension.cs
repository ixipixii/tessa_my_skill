using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.FileConverters;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.PnrCards
{
    /// <summary>
    /// Фикс RowID версий файлов в договорах с покупателями для поддержки работы ссылок в CRM.
    /// </summary>
    class CrmFixFileVersionCardStoreExtension : CardStoreExtension
    {
        private readonly ILogger logger = LogManager.GetLogger("CrmFixFileVersionLogger");
        private readonly ICardRepository defaultCardRepositoryWT;
        private readonly ICardRepository extendedCardRepositoryWT;
        private readonly ITransactionStrategy transactionStrategy;
        private readonly ICardTransactionStrategy cardTransactionStrategy;
        private readonly ICardGetStrategy cardGetStrategy;
        private readonly ITessaServerSettings tessaServerSettings;

        public CrmFixFileVersionCardStoreExtension(ICardRepository defaultCardRepositoryWT, ICardRepository extendedCardRepositoryWT, ITransactionStrategy transactionStrategy, ICardTransactionStrategy cardTransactionStrategy, ICardGetStrategy cardGetStrategy, ITessaServerSettings tessaServerSettings)
        {
            this.defaultCardRepositoryWT = defaultCardRepositoryWT;
            this.extendedCardRepositoryWT = extendedCardRepositoryWT;
            this.transactionStrategy = transactionStrategy;
            this.cardTransactionStrategy = cardTransactionStrategy;
            this.cardGetStrategy = cardGetStrategy;
            this.tessaServerSettings = tessaServerSettings;
        }

        public override async Task BeforeRequest(ICardStoreExtensionContext context)
        {
            // если изменилсь определенные файлы, подпишемся на завершение их загрузки и обновим RowID версий
            await TryFixFileVersionForCRM(context);
        }

        /// <summary>
        /// RowID оригинальной версии файла
        /// </summary>
        private async Task<Guid?> GetOriginalVersionRowIDAsync(IDbScope dbScope, Guid fileRowID)
        {
            using (dbScope.Create())
            {
                var db = dbScope.Db;

                return await db
                    .SetCommand(@"select top 1 OriginalVersionRowID
                                from Files with(nolock)
                                where RowID = @fileRowID",
                                db.Parameter("@fileRowID", fileRowID))
                    .ExecuteAsync<Guid?>();
            }
        }

        /// <summary>
        /// RowID всех версий файлов карточки, которые целиком хранятся в БД (FileContent)
        /// </summary>
        private async Task<List<Guid>> GetCardAllValidFileVersionRowIDsAsync(IDbScope dbScope, Guid cardID)
        {
            using (dbScope.Create())
            {
                var db = dbScope.Db;

                return await db
                    .SetCommand(@"select distinct fc.VersionRowID
                                from Files f with(nolock)
                                join FileVersions fv with(nolock) on f.RowID = fv.ID
                                join FileContent fc with(nolock) on fc.VersionRowID = fv.RowID
                                where f.ID = @cardID",
                                db.Parameter("@cardID", cardID))
                    .ExecuteListAsync<Guid>();
            }
        }

        private async Task TryFixFileVersionForCRM(ICardStoreExtensionContext context)
        {
            var card = context.Request.Card;

            if (card == null 
                || card.Files.Count == 0
                // если сохранение под системной учеткой, то не выполняем обновление версий
                || context.Session.User.ID == Session.SystemID)
            {
                return;
            }

            Guid? kindID = null;

            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;

                kindID = await db
                    .SetCommand(@"select top 1 coalesce(c.KindID, sa.KindID) as KindID
                                from DocumentCommonInfo dci with(nolock)
                                left join PnrContracts c with(nolock) on c.ID = dci.ID
                                left join PnrSupplementaryAgreements sa with(nolock) on sa.ID = dci.ID
                                where dci.ID = @cardID",
                                db.Parameter("@cardID", card.ID))
                    .ExecuteAsync<Guid?>();
            }

            // убедимся, что это договор с покупателями
            if (kindID != PnrContractKinds.PnrContractWithBuyersID)
            {
                return;
            }

            // проверим, изменялись ли файлы, которые пришли изначально через интеграцию из CRM (или миграции скриптом)
            var replacedFiles = card.Files
                .Where(x =>
                    // файл был создан системной учеткой
                    x.Card.CreatedByID == Session.SystemID
                    // содержимое файла было изменено
                    && (x.State == CardFileState.Replaced || x.State == CardFileState.ModifiedAndReplaced));


            // если есть такие файлы, подпишемся на событие окончания закгрузки контента
            if (replacedFiles.Any())
            {
                context.ContentStoreCompleted += async (s, e) =>
                {
                    using (e.Defer())
                    {
                        if (e.Success)
                        {
                            // обновим RowID у версий файлов
                            await UpdateFileVersionsRowIDs(context, card, replacedFiles);
                        }
                    }
                };
            }
        }

        /// <summary>
        /// Удаление кэш файлов указанной версии файла.
        /// </summary>
        private async Task TryClearFileCacheAsync(ICardStoreExtensionContext context, Guid fileVersionRowID)
        {
            List<Guid> cachedRowIDs = new List<Guid>();

            // найдем кэш файлы указанной версии
            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;

                cachedRowIDs = await db
                    .SetCommand(@"select RowID
                                from FileConverterCache with(nolock)
                                where VersionID = @fileVersionRowID",
                                db.Parameter("@fileVersionRowID", fileVersionRowID))
                    .ExecuteListAsync<Guid>();
            }

            if (cachedRowIDs.Count == 0)
            {
                return;
            }

            logger.Info($"Для версии {fileVersionRowID} найдены кэш файлы: {string.Join(", ", cachedRowIDs)}.");

            ValidationResultBuilder validationResult = new ValidationResultBuilder();

            // код внутри будет выполняться от имени пользователя System
            await using (SessionContext.Create(Session.CreateSystemToken(SessionType.Server, tessaServerSettings)))
            {
                await this.cardTransactionStrategy
                .ExecuteInReaderLockAsync(
                    FileConverterHelper.CacheCardID,
                    validationResult,
                    async p =>
                    {
                        // загрузим карточку кэш файлов
                        CardGetContext cardGetContext = await this.cardGetStrategy.TryLoadCardInstanceAsync(
                            FileConverterHelper.CacheCardID,
                            p.DbScope.Db,
                            context.CardMetadata,
                            p.ValidationResult,
                            CardNewMode.Default,
                            p.CancellationToken).ConfigureAwait(false);

                        if (!cardGetContext.ValidationResult.IsSuccessful())
                        {
                            p.ReportError = true;
                            return;
                        }

                        var fileCacheCard = cardGetContext.Card;

                        // загрузим связанные с версией кэшированные файлы
                        var cardGetFilesContext = this.cardGetStrategy.TryLoadFileInstancesAsync(
                            FileConverterHelper.CacheCardID,
                            fileCacheCard,
                            p.DbScope.Db,
                            context.CardMetadata,
                            p.ValidationResult,
                            CardNewMode.Default,
                            cachedRowIDs,
                            p.CancellationToken).ConfigureAwait(false).GetAwaiter();

                        if (cardGetFilesContext.GetResult().Count == 0)
                        {
                            p.ReportError = true;
                            return;
                        }

                        ListStorage<CardRow> rows = fileCacheCard.Sections.GetOrAddTable(nameof(FileConverterCache), CardTableType.Collection).Rows;

                        // отметим кэш файлы для удаления
                        foreach (var rowID in cachedRowIDs)
                        {
                            CardRow cardRow = rows.Add();
                            cardRow.RowID = rowID;
                            cardRow.State = CardRowState.Deleted;
                        }

                        foreach (CardFile file in fileCacheCard.Files)
                        {
                            file.State = CardFileState.Deleted;
                        }

                        fileCacheCard.RemoveAllButChanged();

                        // сохраним карточку кэш файлов
                        var fileCacheCardStoreResponse = await extendedCardRepositoryWT.StoreAsync(new CardStoreRequest()
                        {
                            Card = fileCacheCard,
                            DoesNotAffectVersion = true
                        });

                        if (!fileCacheCardStoreResponse.ValidationResult.IsSuccessful())
                        {
                            p.ValidationResult.Add(fileCacheCardStoreResponse.ValidationResult);
                            p.ReportError = true;
                            return;
                        }

                        logger.Info($"Кэш файлы для версии {fileVersionRowID} успешно удалены.");
                    },
                    context.CancellationToken);
            }


            if (!validationResult.IsSuccessful())
            {
                logger.Error($"ОШИБКА удаления кэш файлов для версии {fileVersionRowID}: " + validationResult.Build().ToString());
            }
        }

        private async Task UpdateFileVersionsRowIDs(ICardStoreExtensionContext context, Card card, IEnumerable<CardFile> replacedFiles)
        {
            // обновим версию каждого файла из тех, что были определены ранее
            // для этого перезагрузим карточку, чтобы получить актуальный список файлов
            var reloadedCardResponse = await defaultCardRepositoryWT.GetAsync(new CardGetRequest()
            {
                CardID = card.ID,
                CardTypeID = card.TypeID
            });

            if (!reloadedCardResponse.ValidationResult.IsSuccessful())
            {
                logger.Error($"Не удалось загрузить карточку {card.ID}: " + reloadedCardResponse.ValidationResult.Build().ToString());
                return;
            }

            var reloadedCard = reloadedCardResponse.Card;

            foreach (var file in replacedFiles)
            {
                // определим RowID оригинальной версии файла (о заполнении этого поля мы позаботились заранее при интеграции/миграции файлов из CRM)
                var originalVersionRowID = await GetOriginalVersionRowIDAsync(context.DbScope, file.RowID);

                // найдем только что пересохраненный файл с новой версией
                var newlyFile = reloadedCard.Files.FirstOrDefault(x => x.RowID == file.RowID);

                // актуальная версия файла
                var lastVersion = newlyFile?.LastVersion;

                // пропустим, если:
                // не нашли RowID оригинальной версии
                if (originalVersionRowID == null
                    // не нашли файл с новой версией
                    || newlyFile == null
                    || lastVersion == null
                    // версия файла не увеличилась
                    || newlyFile.VersionNumber <= file.VersionNumber
                    // RowID новой версии уже равен оригинальной (вдруг во время сохранения сработал скрипт исправления версий)
                    || lastVersion.RowID == originalVersionRowID)
                {
                    continue;
                }

                string fileLogInfo =
                    $"============================================================" +
                    $"{Environment.NewLine}{Environment.NewLine}" +
                    $"  ID карточки: {card.ID}{Environment.NewLine}" +
                    $"  Файл: {newlyFile.Name}{Environment.NewLine}" +
                    $"  RowID файла: {newlyFile.RowID}{Environment.NewLine}" +
                    $"  OriginalVersionRowID: {originalVersionRowID.Value}{Environment.NewLine}" +
                    $"  LastVersionRowID: {lastVersion.RowID}{Environment.NewLine}" +
                    $"  LastVersionNumber: {lastVersion.Number}{Environment.NewLine}" +
                    $"  LastVersionAuthor: {lastVersion.CreatedByName}";

                logger.Info(fileLogInfo);

                var allValidFileVersionRowIDs = await GetCardAllValidFileVersionRowIDsAsync(context.DbScope, card.ID);

                // дополнительно проверим, что найденные версии файла целиком содержатся в БД (вдруг однажды контент в другой таблице или на диске окажется)
                if (!allValidFileVersionRowIDs.Contains(originalVersionRowID.Value)
                    || !allValidFileVersionRowIDs.Contains(lastVersion.RowID))
                {
                    logger.Error("ОШИБКА: Не удалось найти данные файла во всех зависимых таблицах БД!" + Environment.NewLine);
                    continue;
                }

                var validationResult = new ValidationResultBuilder();

                // все выполняем в транзакции
                await this.transactionStrategy.ExecuteInTransactionAsync(
                    validationResult,
                    async p =>
                    {
                        var db = p.DbScope.Db;

                        // оригинальной версии файла и контенту заданим новый RowID
                        Guid newVersionRowID = Guid.NewGuid();

                        await db
                            .SetCommand(@"update fc
	                                    set fc.VersionRowID = @newVersionRowID
	                                    from FileContent fc
	                                    where fc.VersionRowID = @originalVersionRowID

                                        update fv
	                                    set fv.RowID = @newVersionRowID
	                                    from FileVersions fv
	                                    where fv.RowID = @originalVersionRowID",
                                        db.Parameter("@originalVersionRowID", originalVersionRowID.Value),
                                        db.Parameter("@newVersionRowID", newVersionRowID))
                            .ExecuteNonQueryAsync(p.CancellationToken);

                        // версии, контенту и актуальному файлу зададим оригинальный RowID
                        await db
                            .SetCommand(@"update fc
	                                    set fc.VersionRowID = @originalVersionRowID
	                                    from FileContent fc
	                                    where fc.VersionRowID = @lastVersionRowID

                                        update fv
	                                    set fv.RowID = @originalVersionRowID
	                                    from FileVersions fv
	                                    where fv.RowID = @lastVersionRowID

                                        update f
	                                    set f.VersionRowID = @originalVersionRowID
	                                    from Files f
	                                    where f.RowID = @fileRowID",
                                        db.Parameter("@fileRowID", newlyFile.RowID),
                                        db.Parameter("@lastVersionRowID", lastVersion.RowID),
                                        db.Parameter("@originalVersionRowID", originalVersionRowID.Value))
                            .ExecuteNonQueryAsync(p.CancellationToken);

                        if (!p.ValidationResult.IsSuccessful())
                        {
                            p.ReportError = true;
                            return;
                        }
                    },
                    context.CancellationToken);

                if (!validationResult.IsSuccessful())
                {
                    logger.Error("ОШИБКА: " + validationResult.Build().ToString());
                    continue;
                }

                logger.Info($"Версия файла успешно обновлена!");

                // очистим кэш по файлу, иначе конвертация PDF будет брать старую версию файла
                await TryClearFileCacheAsync(context, originalVersionRowID.Value);
            }
        }
    }
}
