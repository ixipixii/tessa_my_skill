using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions.Templates;
using Tessa.Extensions.Default.Server.Workflow.KrObjectModel;
using Tessa.Extensions.Default.Shared;
using Tessa.Extensions.Default.Shared.Workflow.KrProcess;
using Tessa.Json;
using Tessa.Json.Serialization;
using Tessa.Platform;
using Tessa.Platform.Collections;
using Tessa.Platform.Data;
using Tessa.Platform.Json;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess.Workflow
{
    public static class KrProcessHelper
    {
        #region Nested Types

        private sealed class SerializerEqualityComparer : IEqualityComparer
        {
            public static readonly IEqualityComparer Instance = new SerializerEqualityComparer();

            private SerializerEqualityComparer()
            {
            }

            /// <inheritdoc />
            bool IEqualityComparer.Equals(
                object x,
                object y)
            {
                if (x is WorkflowProcess && y is WorkflowProcess)
                {
                    return ReferenceEquals(x, y);
                }

                return x == y;
            }

            /// <inheritdoc />
            int IEqualityComparer.GetHashCode(
                object obj) => obj.GetHashCode();
        }

        private sealed class ContractResolverWithPrivates : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);

                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }

                return prop;
            }
        }

        #endregion

        #region Fields

        private static readonly ThreadLocal<JsonSerializer> workflowProcessSerializer = new ThreadLocal<JsonSerializer>(
            () => TessaSerializer.CreateTyped(new JsonSerializerSettings
            {
                EqualityComparer = SerializerEqualityComparer.Instance,
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
                ContractResolver = new ContractResolverWithPrivates(),
            }));

        #endregion

        #region Methods

        /// <summary>
        /// Проверка того, что в текущем соединении в dbScope открыта транзакция
        /// </summary>
        /// <param name="dbScope"></param>
        /// <returns></returns>
        public static bool IsTransactionOpened(IDbScope dbScope)
        {
            try
            {
                return dbScope?.Db?.Transaction != null;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        /// <summary>
        /// Определение, поддерживает ли карточка процесс маршрутов
        /// </summary>
        /// <param name="card"></param>
        /// <param name="dbScope"></param>
        /// <param name="typesCache"></param>
        /// <returns></returns>
        public static bool CardSupportsRoutes(
            Card card,
            IDbScope dbScope,
            IKrTypesCache typesCache)
        {
            if (card.TypeID == DefaultCardTypes.KrSettingsTypeID)
            {
                return false;
            }

            var krCardType = typesCache.GetCardTypesAsync().GetAwaiter().GetResult() // TODO async
                .FirstOrDefault(p => p.ID == card.TypeID);

            if (krCardType == null)
            {
                return false;
            }
            if (!krCardType.UseDocTypes)
            {
                return krCardType.UseApproving;
            }

            var docTypeIDClosure = KrProcessSharedHelper.GetDocTypeID(card, dbScope);
            var docType = typesCache.GetDocTypesAsync().GetAwaiter().GetResult() // TODO async
                .FirstOrDefault(p => p.ID == docTypeIDClosure);

            return docType?.UseApproving == true;
        }


        /// <summary>
        /// Установить стандартные значения для строк с этапами согласования.
        /// </summary>
        /// <param name="responseWithCard"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetStageDefaultValues(CardValueResponseBase responseWithCard)
        {
            var sectionRows = responseWithCard.TryGetSectionRows();
            if (sectionRows != null
                && sectionRows.TryGetValue(KrStages.Virtual, out var stageRow))
            {
                stageRow[KrStages.StageStateID] = (int)KrStageState.Inactive;
                stageRow[KrStages.StageStateName] = KrStageState.Inactive.TryGetDefaultName();
            }
        }

        /// <summary>
        /// Установить всем этапам согласования состояние "Inactive".
        /// </summary>
        /// <param name="card"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInactiveStateToStages(Card card)
        {
            if (card.TryGetStagesSection(out var stagesSec))
            {
                foreach (var row in stagesSec.Rows)
                {
                    row[KrStages.StageStateID] = (int)KrStageState.Inactive;
                    row[KrStages.StageStateName] = KrStageState.Inactive.TryGetDefaultName();
                }
            }
        }

        /// <summary>
        /// Проверка, существует ли карточка по записи в Instances.
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="dbScope"></param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        public static async Task<bool> CardExistsAsync(Guid cardID, IDbScope dbScope, CancellationToken cancellationToken = default)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                        dbScope.BuilderFactory
                            .Select()
                            .V(1)
                            .From("Instances").NoLock()
                            .Build(),
                        db.Parameter("id", cardID))
                    .LogCommand()
                    .ExecuteAsync<bool>(cancellationToken);
            }
        }

        /// <summary>
        /// Проверка, существует ли основной сателлит Kr процесса.
        /// </summary>
        /// <param name="mainCardID"></param>
        /// <param name="dbScope"></param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        public static async Task<bool> SatelliteExistsAsync(Guid mainCardID, IDbScope dbScope, CancellationToken cancellationToken = default)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                        dbScope.BuilderFactory
                            .Select()
                                .V(1)
                            .From(KrApprovalCommonInfo.Name).NoLock()
                            .Where().C(KrProcessCommonInfo.MainCardID).Equals().P("id")
                            .Build(),
                        db.Parameter("id", mainCardID))
                    .LogCommand()
                    .ExecuteAsync<bool>(cancellationToken);
            }
        }

        /// <summary>
        /// Загрузить список идентификаторов карточек вторичных сателлитов.
        /// </summary>
        /// <param name="mainCardID"></param>
        /// <param name="dbScope"></param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        public static async Task<List<Guid>> GetSecondarySatellitesIDsAsync(Guid mainCardID, IDbScope dbScope, CancellationToken cancellationToken = default)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                return await db
                    .SetCommand(
                        dbScope.BuilderFactory
                            .Select()
                            .C("ID")
                            .From(KrSecondaryProcessCommonInfo.Name).NoLock()
                            .Where().C(KrProcessCommonInfo.MainCardID).Equals().P("ID")
                            .Build(),
                        db.Parameter("ID", mainCardID))
                    .LogCommand()
                    .ExecuteListAsync<Guid>(cancellationToken);
            }
        }

        /// <summary>
        /// Загрузить список данных о карточках сателлитов диалогах.
        /// В информации содержится идентификатор и тип.
        /// </summary>
        /// <param name="mainCardID"></param>
        /// <param name="dbScope"></param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns></returns>
        public static async Task<List<SatelliteInfo>> GetDialogSatelliteInfosAsync(
            Guid mainCardID,
            IDbScope dbScope,
            CancellationToken cancellationToken = default)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                var result = new List<SatelliteInfo>();
                db.SetCommand(
                        dbScope.BuilderFactory
                            .Select()
                            .C(ID)
                            .C(KrDialogSatellite.TypeID)
                            .From(KrDialogSatellite.Name).NoLock()
                            .Where().C(KrDialogSatellite.MainCardID).Equals().P(ID)
                            .Build(),
                        db.Parameter(ID, mainCardID))
                    .LogCommand();

                await using (var reader = await db.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        result.Add(new SatelliteInfo(
                            reader.GetGuid(0),
                            reader.GetGuid(1),
                            default
                            ));
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Создать информацию о сателлите.
        /// </summary>
        /// <param name="satelliteCard"></param>
        /// <returns></returns>
        public static SatelliteInfo CreateSatelliteInfo(Card satelliteCard)
        {
            var cardID = satelliteCard.ID;
            return new SatelliteInfo(cardID, satelliteCard.TypeID, cardID, EmptyHolder<Guid>.Collection);
        }

        /// <summary>
        /// Загрузить явно из БД информацию о вторичных сателлита.
        /// </summary>
        /// <param name="mainCardID"></param>
        /// <param name="dbScope"></param>
        /// <param name="satelliteTypeID"></param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        public static async Task<List<SatelliteInfo>> TryGetSecondarySatelliteInfoListAsync(
            Guid mainCardID,
            IDbScope dbScope,
            Guid satelliteTypeID,
            CancellationToken cancellationToken = default)
        {
            return (await GetSecondarySatellitesIDsAsync(mainCardID, dbScope, cancellationToken))
                .Select(id => new SatelliteInfo(id, satelliteTypeID, id, EmptyHolder<Guid>.Collection))
                .ToList();
        }

        /// <summary>
        /// Загрузить тип карточки шаблона.
        /// </summary>
        /// <param name="templateID">Идентификатор карточки шаблона</param>
        /// <param name="dbScope"></param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        public static async Task<Guid?> GetTemplateCardTypeAsync(
            Guid templateID,
            IDbScope dbScope,
            CancellationToken cancellationToken = default)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                var query = dbScope.BuilderFactory
                    .Select()
                    .C("TypeID")
                    .From("Templates").NoLock()
                    .Where().C("ID").Equals().P("TemplateID")
                    .Build();
                return await db
                    .SetCommand(query, db.Parameter("TemplateID", templateID))
                    .LogCommand()
                    .ExecuteAsync<Guid?>(cancellationToken);
            }
        }

        /// <summary>
        /// Загрузить тип документа или тип карточки(если тип документа отсутствует) из карточки шаблона.
        /// </summary>
        /// <param name="templateID">Идентификатор карточки шаблона</param>
        /// <param name="dbScope"></param>
        /// <param name="cancellationToken">Объект, посредством которого можно отменить асинхронную задачу.</param>
        /// <returns>Асинхронная задача.</returns>
        public static async Task<Guid?> GetTemplateDocTypeAsync(
            Guid templateID,
            IDbScope dbScope,
            CancellationToken cancellationToken = default)
        {
            await using (dbScope.Create())
            {
                var db = dbScope.Db;
                var query = dbScope.BuilderFactory
                    .Select()
                    .C("TypeID")
                    .C("Card")
                    .From("Templates").NoLock()
                    .Where().C("ID").Equals().P("TemplateID")
                    .Build();
                db
                    .SetCommand(query, db.Parameter("TemplateID", templateID))
                    .LogCommand();

                Guid cardTypeID;
                byte[] cardSerialized;
                await using (var reader = await db.ExecuteReaderAsync(cancellationToken))
                {
                    if (!await reader.ReadAsync(cancellationToken))
                    {
                        return null;
                    }

                    cardTypeID = reader.GetGuid(0);
                    cardSerialized = reader.GetNullableBytes(1);
                }

                var cardStorage = cardSerialized?.ToSerializable()?.GetStorage();
                if (cardStorage != null)
                {
                    var card = new Card(cardStorage);
                    StringDictionaryStorage<CardSection> sections;
                    if ((sections = card.TryGetSections()) != null
                        && sections.TryGetValue(DocumentCommonInfo.Name, out var dciSec)
                        && dciSec.Fields.TryGetValue(DocumentCommonInfo.DocTypeID, out var dtidObj)
                        && dtidObj is Guid docTypeID)
                    {
                        return docTypeID;
                    }
                }

                return cardTypeID;
            }
        }

        public static string SerializeWorkflowProcess(
            WorkflowProcess workflowProcess) =>
            StorageHelper.SerializeToJson(workflowProcess, workflowProcessSerializer.Value);

        public static WorkflowProcess DeserializeWorkflowProcess(
            string json) =>
            StorageHelper.DeserializeFromJson<WorkflowProcess>(json, workflowProcessSerializer.Value);

        public static byte[] SignWorkflowProcess(
            string serializedWorkflowProcess,
            Guid? cardID,
            Guid processID,
            ISignatureProvider signatureProvider)
        {
            var processBytes = ConcatWorkflowProcessToByteArray(serializedWorkflowProcess, cardID, processID);
            return signatureProvider.Sign(processBytes);
        }

        public static bool VerifyWorkflowProcess(
            KrProcessInstance instance,
            ISignatureProvider signatureProvider)
        {
            var processBytes = ConcatWorkflowProcessToByteArray(instance.SerializedProcess, instance.CardID, instance.ProcessID);
            return signatureProvider.Verify(processBytes, instance.SerializedProcessSignature);
        }

        #endregion

        #region Private

        private static byte[] ConcatWorkflowProcessToByteArray(
            string serializedWorkflowProcess,
            Guid? cardID,
            Guid processID)
        {
            var processBytes = Encoding.UTF8.GetBytes(serializedWorkflowProcess);
            var processBytesOriginalSize = processBytes.Length;
            const int guidSize = 16;
            Array.Resize(ref processBytes, processBytesOriginalSize + 2 * guidSize); // Два гуида
            Array.Copy((cardID ?? default).ToByteArray(), 0, processBytes, processBytesOriginalSize, guidSize);
            Array.Copy(processID.ToByteArray(), 0, processBytes, processBytesOriginalSize + guidSize, guidSize);

            return processBytes;
        }

        #endregion

    }
}