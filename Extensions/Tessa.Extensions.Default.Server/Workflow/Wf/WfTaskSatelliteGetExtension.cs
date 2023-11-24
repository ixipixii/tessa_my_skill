using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.ComponentModel;
using Tessa.Cards.Extensions;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Extensions.Default.Shared.Workflow.Wf;
using Tessa.Platform.Data;
using Tessa.Platform.Storage;
using Unity;

namespace Tessa.Extensions.Default.Server.Workflow.Wf
{
    /// <summary>
    /// Загружаем физическую или виртуальную карточку-сателлит по идентификатору задания,
    /// а также заполняем поля сателлита после успешной загрузки.
    ///
    /// Расширение должно выполняться перед <see cref="WfTasksServerGetExtension"/>,
    /// поэтому желательно использовать <see cref="ExtensionStage.BeforePlatform"/>.
    /// </summary>
    public class WfTaskSatelliteGetExtension :
        TaskSatelliteGetExtension
    {
        #region Constructors

        /// <summary>
        /// Создаёт экземпляр класса с указанием его зависимостей.
        /// </summary>
        /// <param name="extendedRepository">Репозиторий для управления карточками с расширениями и транзакцией.</param>
        /// <param name="extendedRepositoryWithoutTransaction">Репозиторий для управления карточками с расширениями, но без транзакции.</param>
        /// <param name="cardTransactionStrategy">Стратегия обеспечения блокировок для взаимодействия с основной карточкой.</param>
        /// <param name="cardGetStrategy">Стратегия низкоуровневой загрузки карточки, используемая при загрузке виртуального задания.</param>
        /// <param name="cardNewStrategy">Стратегия низкоуровневого создания структуры карточки, используемая при загрузке виртуального задания.</param>
        public WfTaskSatelliteGetExtension(
            ICardRepository extendedRepository,
            [Dependency(CardRepositoryNames.ExtendedWithoutTransaction)] ICardRepository extendedRepositoryWithoutTransaction,
            ICardTransactionStrategy cardTransactionStrategy,
            ICardGetStrategy cardGetStrategy,
            ICardNewStrategy cardNewStrategy)
            : base(
                 extendedRepository,
                 extendedRepositoryWithoutTransaction,
                 cardTransactionStrategy,
                 cardGetStrategy,
                 cardNewStrategy)
        {
        }

        #endregion

        #region Fields

        private static readonly string[] documentCommonInfoFields =
        {
            "DocTypeID",
            "DocTypeTitle",
            "Number",
            "FullNumber",
            "Sequence",
            "Subject",
            "DocDate",
            "CreationDate",
            "AuthorID",
            "AuthorName",
            "RegistratorID",
            "RegistratorName",
        };

        #endregion

        #region Base Overrides

        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="SatelliteTypeID"]'/>
        protected override Guid SatelliteTypeID => DefaultCardTypes.WfTaskCardTypeID;

        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="FileIsExternalKey"]'/>
        protected override string FileIsExternalKey => WfHelper.FileIsExternalKey;

        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="VirtualMainCardIDKey"]'/>
        protected override string VirtualMainCardIDKey => WfHelper.VirtualMainCardIDKey;

        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="VirtualSatelliteSection"]'/>
        protected override string VirtualSatelliteSection => "WfTaskCardsVirtual";

        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="MainCardDigestInVirtualSatelliteSectionFieldName"]'/>
        protected override string MainCardDigestInVirtualSatelliteSectionFieldName => "MainCardDigest";

        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="TryGetTaskSatelliteIDAsync"]'/>
        protected override Task<Guid?> TryGetTaskSatelliteIDAsync(IDbScope dbScope, Guid taskRowID, CancellationToken cancellationToken = default) =>
            WfHelper.TryGetTaskSatelliteIDAsync(dbScope, taskRowID, cancellationToken);

        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="TryGetMainCardIDByTaskRowIDAsync"]'/>
        protected override Task<Guid?> TryGetMainCardIDByTaskRowIDAsync(IDbScope dbScope, Guid taskRowID, CancellationToken cancellationToken = default) =>
            CardSatelliteHelper.TryGetMainCardIDByTaskRowIDAsync(dbScope, taskRowID, cancellationToken);


        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="SetupVirtualSatelliteAsync"]'/>
        protected override async ValueTask SetupVirtualSatelliteAsync(
            ICardGetExtensionContext context,
            Card satellite,
            Guid mainCardID,
            Guid taskRowID)
        {
            Dictionary<string, object> fields = satellite.Sections[WfHelper.TaskSatelliteSection].RawFields;
            fields[WfHelper.TaskSatelliteTaskRowIDField] = taskRowID;
            fields[WfHelper.TaskSatelliteMainCardIDField] = mainCardID;
        }


        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="SetupSatelliteFileAsync"]'/>
        protected override async ValueTask SetupSatelliteFileAsync(
            ICardGetExtensionContext context,
            CardFile file,
            bool isMainCard)
        {
            if (isMainCard)
            {
                file.CategoryID = WfHelper.MainCardCategoryID;
                file.CategoryCaption = WfHelper.MainCardCategoryCaption;
            }
        }


        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="LoadExternalCardsWithFilesListAsync"]'/>
        protected override async Task<IEnumerable<Guid>> LoadExternalCardsWithFilesListAsync(
            ICardGetExtensionContext context,
            IDbScope dbScope,
            Guid currentTaskRowID)
        {
            var db = dbScope.Db;

            return await db
                .SetCommand(
                    dbScope.BuilderFactory
                        .With("TasksCTE", b => b
                            .Select().C("th", "ParentRowID")
                            .From("TaskHistory", "th").NoLock()
                            .Where().C("th", "RowID").Equals().P("CurrentTaskRowID")
                                .And().C("th", "ParentRowID").IsNotNull()
                            .UnionAll()
                            .Select().C("th", "ParentRowID")
                            .From("TasksCTE", "t")
                            .InnerJoin("TaskHistory", "th").NoLock()
                                .On().C("th", "RowID").Equals().C("t", "RowID")
                            .Where().C("th", "ParentRowID").IsNotNull(),
                            columnNames: new[] { "RowID" },
                            recursive: true)
                        .Select().C("wf", "ID")
                        .From("TasksCTE", "t")
                        .InnerJoin("WfTaskCards", "wf").NoLock()
                            .On().C("wf", "TaskRowID").Equals().C("t", "RowID")
                        .Where()
                        .Exists(e => e
                            .Select().V(1)
                            .From("Files", "f").NoLock()
                            .Where().C("f", "ID").Equals().C("wf", "ID"))
                        .Build(),
                    db.Parameter("CurrentTaskRowID", currentTaskRowID))
                .LogCommand()
                .ExecuteListAsync<Guid>(context.CancellationToken);
        }


        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="TryGetMainCardIDAndTaskRowIDAsync"]'/>
        protected override async ValueTask<(bool result, Guid? mainCardID, Guid? taskRowID)> TryGetMainCardIDAndTaskRowIDAsync(
            ICardGetExtensionContext context,
            Card satellite)
        {
            if (!satellite.Sections.TryGetValue(WfHelper.TaskSatelliteSection, out var section))
            {
                return (false, null, null);
            }

            Guid? mainCardID = section.RawFields.TryGet<Guid?>(WfHelper.TaskSatelliteMainCardIDField);
            Guid? taskRowID = section.RawFields.TryGet<Guid?>(WfHelper.TaskSatelliteTaskRowIDField);

            return (mainCardID.HasValue && taskRowID.HasValue, mainCardID, taskRowID);
        }


        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="PrepareSatelliteAfterLoadingAndGetAdditionalInfoAsync"]'/>
        protected override async ValueTask<object> PrepareSatelliteAfterLoadingAndGetAdditionalInfoAsync(
            ICardGetExtensionContext context,
            Card satellite)
        {
            // пробрасываем токен с правами в сателлит, чтобы потом вернуть его в основную карточку
            Dictionary<string, object> requestInfo = context.Request.Info;
            KrToken token = KrToken.TryGet(requestInfo);
            if (token != null)
            {
                token.Set(satellite.Info);
            }

            // пробрасываем признак расчёта по кнопке "Редактировать" в сателлит, чтобы потом вернуть его в основную карточку
            object permissionsCalculated = requestInfo.TryGet<object>(KrPermissionsHelper.PermissionsCalculatedMark);
            if (permissionsCalculated != null)
            {
                satellite.Info[KrPermissionsHelper.PermissionsCalculatedMark] = permissionsCalculated;
            }

            return token;
        }


        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="PrepareRequestToLoadMainCardAsync"]'/>
        protected override async ValueTask PrepareRequestToLoadMainCardAsync(
            ICardGetExtensionContext context,
            CardGetRequest request,
            Guid mainCardID,
            Guid taskRowID,
            object additionalInfo)
        {
            Dictionary<string, object> getRequestInfo = request.Info;
            getRequestInfo[WfHelper.TaskSatelliteTaskRowIDField] = taskRowID;

            (additionalInfo as KrToken)?.Set(getRequestInfo);
        }


        /// <doc path='info[@type="TaskSatelliteGetExtension" and @item="PrepareSatelliteWithMainCardInfoAsync"]'/>
        protected override async ValueTask PrepareSatelliteWithMainCardInfoAsync(
            ICardGetExtensionContext context,
            Card satellite,
            Card mainCard,
            object additionalInfo)
        {
            Dictionary<string, object> virtualFields = satellite.Sections[this.VirtualSatelliteSection].RawFields;

            StringDictionaryStorage<CardSection> mainSections = mainCard.TryGetSections();
            if (mainSections != null)
            {
                if (mainSections.TryGetValue("DocumentCommonInfo", out var mainSection))
                {
                    Dictionary<string, object> fields = mainSection.RawFields;
                    for (int i = 0; i < documentCommonInfoFields.Length; i++)
                    {
                        virtualFields[documentCommonInfoFields[i]] = fields.TryGet<object>(documentCommonInfoFields[i]);
                    }
                }

                if (mainSections.TryGetValue("KrApprovalCommonInfoVirtual", out mainSection))
                {
                    Dictionary<string, object> fields = mainSection.RawFields;
                    virtualFields["StateID"] = fields.TryGet<object>("StateID");
                    virtualFields["StateName"] = fields.TryGet<object>("StateName");
                    virtualFields["StateModified"] = fields.TryGet<object>("StateChangedDateTimeUTC");
                }

                if (!(additionalInfo is KrToken))
                {
                    // при открытии карточки-сателлита по ссылке у нас нет токена в исходной карточке,
                    // но токен может быть в загруженной основной карточке
                    KrToken.TryGet(mainCard.Info)?.Set(satellite.Info);
                }
            }
        }

        #endregion
    }
}
