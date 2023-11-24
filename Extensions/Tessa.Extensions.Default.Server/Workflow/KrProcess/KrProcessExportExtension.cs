using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Extensions.Templates;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Server.Workflow.KrProcess
{
    /// <summary>
    /// При экспорте карточки документа или шаблона процесса в JSON "разворачиваем" бинарные данные каждого этапа в структуру.
    /// </summary>
    public sealed class KrProcessExportExtension :
        CardGetExtension
    {
        #region Private Methods

        private static bool FixCardWithStages(Card card)
        {
            StringDictionaryStorage<CardSection> sections = card.TryGetSections();
            if (sections == null)
            {
                return false;
            }

            bool hasSections = false;

            if (sections.TryGetValue(KrApprovalCommonInfo.Name, out CardSection approvalCommonInfo))
            {
                hasSections = true;

                FixField(Info, approvalCommonInfo.RawFields);
            }

            if (sections.TryGetValue(KrStages.Name, out CardSection stages))
            {
                hasSections = true;

                ListStorage<CardRow> rows = stages.TryGetRows();
                if (rows != null && rows.Count > 0)
                {
                    foreach (CardRow row in rows)
                    {
                        FixField(Info, row);
                        FixField(KrStages.Settings, row);
                    }
                }
            }

            return hasSections;
        }


        private static void FixField(string key, IDictionary<string, object> fieldStorage)
        {
            if (fieldStorage.TryGetValue(key, out object jsonValue)
                && jsonValue is string json
                && json.Length > 0)
            {
                Dictionary<string, object> storage = StorageHelper.DeserializeFromTypedJson(json);
                fieldStorage[key] = storage;
            }
        }

        #endregion

        #region Base Overrides

        public override Task AfterRequest(ICardGetExtensionContext context)
        {
            if (!context.RequestIsSuccessful
                || context.Request.ExportFormat != CardFileFormat.Json)
            {
                return Task.CompletedTask;
            }

            Card card = context.Response.TryGetCard();
            if (card == null)
            {
                return Task.CompletedTask;
            }

            // выполняемся для всех типов карточек, у которых есть таблица "KrStages"
            if (!FixCardWithStages(card))
            {
                // или эта таблица есть в сателлите, который сериализован в Info (верно для карточек-документов с маршрутом)
                Card satellite = CardSatelliteHelper.TryGetSatelliteCard(card, KrSatelliteInfoKey);
                if (satellite != null)
                {
                    FixCardWithStages(satellite);
                }
            }

            return Task.CompletedTask;
        }

        #endregion
    }
}
