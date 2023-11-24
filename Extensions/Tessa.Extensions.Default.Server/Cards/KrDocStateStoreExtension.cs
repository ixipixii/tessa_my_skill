using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared;
using Tessa.Platform.Runtime;
using Tessa.Platform.Storage;
using Tessa.Platform.Validation;
using Tessa.Scheme;
using Unity;

namespace Tessa.Extensions.Default.Server.Cards
{
    /// <summary>
    /// Расширение на сохранение виртуальной карточки состояния документа.
    /// </summary>
    public sealed class KrDocStateStoreExtension :
        CardStoreExtension
    {
        #region Constructors

        public KrDocStateStoreExtension(
            IConfigurationInfoProvider configurationInfoProvider,
            [OptionalDependency] ISchemeService schemeService = null)
        {
            this.configurationInfoProvider = configurationInfoProvider ?? throw new ArgumentNullException(nameof(configurationInfoProvider));
            this.schemeService = schemeService;
        }

        #endregion

        #region Fields

        private readonly IConfigurationInfoProvider configurationInfoProvider;

        private readonly ISchemeService schemeService;

        #endregion

        #region Base Overrides

        public override Task BeforeRequest(ICardStoreExtensionContext context)
        {
            // например, ошибки валидации из-за пустого имени
            if (!context.ValidationResult.IsSuccessful())
            {
                return Task.CompletedTask;
            }

            Card card = context.Request.TryGetCard();
            StringDictionaryStorage<CardSection> sections;

            if (card is null
                || (sections = card.TryGetSections()) is null
                || !sections.TryGetValue("KrDocStateVirtual", out CardSection section))
            {
                return Task.CompletedTask;
            }

            IDictionary<string, object> fields = section.Fields;
            bool nameModified = fields.ContainsKey("Name");
            bool partitionModified = fields.ContainsKey("PartitionID");

            // идентификатор состояния у карточки перед тем, как выполнялось сохранение
            int? originalStateID;
            int? fieldStateID;
            bool cardIsNew = card.StoreMode == CardStoreMode.Insert;
            if (cardIsNew)
            {
                // при создании карточки пользователь мог поменять идентификатор, поэтому нет смысла смотреть в Info
                originalStateID = fields.Get<int?>("StateID");
                fieldStateID = originalStateID;
            }
            else
            {
                // при изменении карточки исходный идентификатор всегда должен быть в Info
                originalStateID = card.TryGetInfo()?.TryGet<int?>(DefaultExtensionHelper.StateIDKey);

                if (fields.TryGetValue("StateID", out object value))
                {
                    // если присутствует в запросе, но пустое, то здесь должен был сработать валидатор
                    fieldStateID = (int?) value;
                }
                else
                {
                    // идентификатор не изменялся
                    fieldStateID = originalStateID;
                }
            }

            if (!originalStateID.HasValue || !fieldStateID.HasValue)
            {
                return Task.CompletedTask;
            }

            bool idModified = fieldStateID.Value != originalStateID.Value;

            // полей для изменения нет
            if (!idModified && !nameModified && !partitionModified)
            {
                context.Response = new CardStoreResponse
                {
                    CardID = card.ID,
                    CardTypeID = DefaultCardTypes.KrDocStateTypeID,
                    CardVersion = 1,
                    StoreDateTime = context.StoreDateTime,
                };

                return Task.CompletedTask;
            }

            if (!context.Session.User.IsAdministrator())
            {
                ValidationSequence
                    .Begin(context.ValidationResult)
                    .SetObjectName(this)
                    .Error(CardValidationKeys.UserIsNotAdmin)
                    .End();

                return Task.CompletedTask;
            }

            this.configurationInfoProvider.CheckSealed();

            if (this.schemeService is null)
            {
                // не зарегистрирован сервис схемы, остальное API карточек на месте,
                // но в этом расширении мы ничего не можем сделать
                return Task.CompletedTask;
            }

            SchemeTable table = this.schemeService.GetTable("KrDocState");
            if (table is null)
            {
                return Task.CompletedTask;
            }

            if (cardIsNew || idModified)
            {
                SchemeRecord stateRecord = table.Records.FirstOrDefault(x => (short) x["ID"] == fieldStateID.Value);
                if (stateRecord != null)
                {
                    // мы знаем, что идентификатор был изменён, т.к. карточки новая или fieldStateID отличается от originalStateID;
                    // так что если мы нашли любую запись в коллекции старых записей с новым идентификатором, то у нас конфликт идентификаторов
                    ValidationSequence
                        .Begin(context.ValidationResult)
                        .SetObjectName(this)
                        .ErrorText("$CardTypes_Validators_IdentifierIsNotUnique")
                        .End();

                    return Task.CompletedTask;
                }
            }

            if (nameModified)
            {
                string name = fields.Get<string>("Name");

                SchemeRecord stateRecord = table.Records.FirstOrDefault(x =>
                    string.Equals((string) x["Name"], name, StringComparison.OrdinalIgnoreCase));

                if (stateRecord != null && (short) stateRecord["ID"] != originalStateID.Value)
                {
                    ValidationSequence
                        .Begin(context.ValidationResult)
                        .SetObjectName(this)
                        .ErrorText("$CardTypes_Validators_UniqueNameIsNotUnique")
                        .End();

                    return Task.CompletedTask;
                }
            }

            bool saveChanges = false;

            if (cardIsNew)
            {
                var record = new SchemeRecord(table)
                {
                    ["ID"] = (short) fieldStateID.Value,
                    ["Name"] = fields.Get<string>("Name"),
                };

                SchemePartition partition;
                Guid? partitionID = fields.Get<Guid?>("PartitionID");
                if (partitionID.HasValue
                    && (partition = this.schemeService.GetPartition(partitionID.Value)) != null)
                {
                    record.Partition = partition;
                }

                table.Records.Add(record);
                saveChanges = true;
            }
            else
            {
                SchemeRecord record = table.Records.FirstOrDefault(x => (short) x["ID"] == originalStateID.Value);
                if (record != null)
                {
                    if (idModified)
                    {
                        record["ID"] = fieldStateID.Value;
                        saveChanges = true;
                    }

                    if (nameModified)
                    {
                        record["Name"] = fields.Get<string>("Name");
                        saveChanges = true;
                    }

                    if (partitionModified)
                    {
                        SchemePartition partition;
                        Guid? partitionID = fields.Get<Guid?>("PartitionID");
                        if (partitionID.HasValue
                            && (partition = this.schemeService.GetPartition(partitionID.Value)) != null)
                        {
                            record.Partition = partition;
                            saveChanges = true;
                        }
                    }
                }
            }

            if (saveChanges)
            {
                this.schemeService.SaveTable(table);
                // кэш схемы, и за ним кэш карточек сбрасываются сами, явный вызов InvalidateCache не нужен
            }

            Guid actualCardID = cardIsNew || idModified
                ? DefaultExtensionHelper.GetKrDocStateCardID(fieldStateID.Value)
                : card.ID;

            context.Response = new CardStoreResponse
            {
                CardID = actualCardID,
                CardTypeID = DefaultCardTypes.KrDocStateTypeID,
                CardVersion = 1,
                StoreDateTime = context.StoreDateTime,
            };

            return Task.CompletedTask;
        }

        #endregion
    }
}