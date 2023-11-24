﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Cards.Extensions.Templates;
using Tessa.Platform.Storage;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    /// <summary>
    /// Расширение <c>KrProcessExportExtension</c> при экспорте в формате <see cref="CardFileFormat.Json"/>
    /// заменит сериализованную в виде строк настройки этапов в их структуру в JSON. Это расширение обратно сериализует их в строки.
    /// </summary>
    public sealed class KrProcessRepairExtension :
        CardRepairExtension
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
            if (fieldStorage.TryGetValue(key, out object settingsValue)
                && settingsValue is Dictionary<string, object> storage)
            {
                string json = StorageHelper.SerializeToTypedJson(storage);
                fieldStorage[key] = json;
            }
        }

        #endregion

        #region Base Overrides

        public override Task AfterRequest(ICardRepairExtensionContext context)
        {
            if (!context.RequestIsSuccessful)
            {
                return Task.CompletedTask;
            }

            // выполняемся для всех типов карточек, у которых есть таблица "KrStages"
            if (!FixCardWithStages(context.Card))
            {
                // или эта таблица есть в сателлите, который сериализован в Info (верно для карточек-документов с маршрутом)
                Card satellite = CardSatelliteHelper.TryGetSatelliteCard(context.Card, KrSatelliteInfoKey);
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
