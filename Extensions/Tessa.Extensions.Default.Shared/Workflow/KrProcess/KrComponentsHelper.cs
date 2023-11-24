using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Tessa.Cards;
using Tessa.Cards.Caching;
using Tessa.Localization;
using Tessa.Platform.Data;
using static Tessa.Extensions.Default.Shared.Workflow.KrProcess.KrConstants;

namespace Tessa.Extensions.Default.Shared.Workflow.KrProcess
{
    public static class KrComponentsHelper
    {
        #region public

        /// <summary>
        /// Оптимизированный метод на определение того, включен ли тип в типовом решении
        /// </summary>
        /// <param name="cardTypeID"></param>
        /// <param name="typesCache"></param>
        /// <returns></returns>
        public static bool HasBase(
            Guid cardTypeID,
            IKrTypesCache typesCache)
        {
            // Исключаем рекурсию
            if (cardTypeID == DefaultCardTypes.KrSettingsTypeID)
            {
                return false;
            }

            var cardTypes = typesCache?.GetCardTypesAsync().ConfigureAwait(false).GetAwaiter().GetResult(); // TODO async
            if (cardTypes == null)
            {
                return false;
            }

            foreach (var type in cardTypes)
            {
                if (type.ID == cardTypeID)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Оптимизированный метод на определение того, включен ли тип в типовом решении
        /// </summary>
        /// <param name="cardTypeID"></param>
        /// <param name="cardCache"></param>
        /// <returns></returns>
        public static bool HasBase(
            Guid cardTypeID,
            ICardCache cardCache)
        {
            // Исключаем рекурсию
            if (cardTypeID == DefaultCardTypes.KrSettingsTypeID)
            {
                return false;
            }

            IList<CardRow> rows;
            try
            {
                rows = cardCache.Cards.GetAsync("KrSettings").GetAwaiter().GetResult() // TODO async
                    .Sections["KrSettingsCardTypes"].Rows;
            }
            catch
            {
                return false;
            }

            foreach (var row in rows)
            {
                if (row.Get<Guid>("CardTypeID") == cardTypeID)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Получить KrComponents для типа карточки с использованием ICardCache
        /// Использовать для избежания циклических вызовов.
        /// </summary>
        /// <param name="cardTypeID"></param>
        /// <param name="cardCache"></param>
        /// <returns></returns>
        public static KrComponents GetKrComponents(Guid cardTypeID, ICardCache cardCache)
        {
            // Исключаем рекурсию
            if (cardTypeID == DefaultCardTypes.KrSettingsTypeID)
            {
                return KrComponents.None;
            }

            IList<CardRow> rows;
            try
            {
                rows = cardCache.Cards.GetAsync("KrSettings").GetAwaiter().GetResult() // TODO async
                    .Sections["KrSettingsCardTypes"].Rows;
            }
            catch
            {
                rows = null;
            }

            var result = KrComponents.None;
            if (rows == null)
            {
                return result;
            }

            foreach (CardRow row in rows)
            {
                if (row.Get<Guid>("CardTypeID") == cardTypeID)
                {
                    result |= KrComponents.Base;
                    if (row.Get<bool>("UseDocTypes"))
                    {
                        result |= KrComponents.DocTypes;
                        //Использование согласования/регистрации будет определяться типом документа
                        return result;
                    }
                    //Если тип указан в настройках и указано "использовать" согласование
                    if (row.Get<bool>("UseApproving"))
                    {
                        result |= KrComponents.Routes;
                    }
                    //Если тип указан в настройках и указано "использовать" регистрацию
                    if (row.Get<bool>("UseRegistration"))
                    {
                        result |= KrComponents.Registration;
                    }
                    //Если тип указан в настройках и указано "использовать" резолюции
                    if (row.Get<bool>("UseResolutions"))
                    {
                        result |= KrComponents.Resolutions;
                    }
                    return result;
                }
            }

            return KrComponents.None;
        }

        /// <summary>
        /// Получить KrComponents только для типа карточки без учета типа документа.
        /// </summary>
        /// <param name="cardTypeID"></param>
        /// <param name="typesCache"></param>
        /// <returns></returns>
        public static KrComponents GetKrComponents(
            Guid cardTypeID,
            IKrTypesCache typesCache)
        {
            // Исключаем рекурсию
            if (cardTypeID == DefaultCardTypes.KrSettingsTypeID)
            {
                return KrComponents.None;
            }

            var result = KrComponents.None;
            var type = typesCache
                ?.GetCardTypesAsync().ConfigureAwait(false).GetAwaiter().GetResult() // TODO async
                ?.FirstOrDefault(p => p.ID == cardTypeID);
            if (type == null)
            {
                return result;
            }

            result |= KrComponents.Base;
            if (type.UseDocTypes)
            {
                result |= KrComponents.DocTypes;
            }
            if (type.UseApproving)
            {
                result |= KrComponents.Routes;
            }
            if (type.UseRegistration)
            {
                result |= KrComponents.Registration;
            }
            if (type.UseResolutions)
            {
                result |= KrComponents.Resolutions;
            }
            if (type.UseForum)
            {
                result |= KrComponents.UseForum;
            }
            return result;
        }

        /// <summary>
        /// Получить KrComponents только для типа карточки по известному типу карточки и документа.
        /// </summary>
        /// <param name="cardTypeID"></param>
        /// <param name="docTypeID"></param>
        /// <param name="typesCache"></param>
        /// <returns></returns>
        public static KrComponents GetKrComponents(
            Guid cardTypeID,
            Guid? docTypeID,
            IKrTypesCache typesCache)
        {
            var result = GetKrComponents(cardTypeID, typesCache);

            if (docTypeID.HasValue
                && result.Has(KrComponents.DocTypes))
            {
                var docType = typesCache
                    .GetDocTypesAsync().ConfigureAwait(false).GetAwaiter().GetResult() // TODO async
                    ?.FirstOrDefault(x => x.ID == docTypeID);
                result = GetDocTypeComponents(docType);
            }

            return result;
        }

        /// <summary>
        /// Получить включенные компоненты типового решения для указанной карточки.
        /// </summary>
        /// <param name="card">карточка, для которой необходимо получить компоненты</param>
        /// <param name="typesCache">Кэш типов документов</param>
        /// <returns></returns>
        public static KrComponents GetKrComponents(
            Card card,
            IKrTypesCache typesCache)
        {
            var result = GetKrComponents(card.TypeID, typesCache);
            if (result == KrComponents.None)
            {
                return result;
            }

            if (result.Has(KrComponents.DocTypes)
                && card.Sections.TryGetValue(DocumentCommonInfo.Name, out var dciSec)
                && dciSec.Fields.TryGetValue(DocumentCommonInfo.DocTypeID, out var docTypeIDObj)
                && docTypeIDObj is Guid docTypeID)
            {
                var docType = typesCache
                    .GetDocTypesAsync().ConfigureAwait(false).GetAwaiter().GetResult() // TODO async
                    ?.FirstOrDefault(x => x.ID == docTypeID);
                result = GetDocTypeComponents(docType);
            }

            return result;
        }

        /// <summary>
        /// Получить включенные компоненты для карточки с учетом типов документов, обладая только ID карточки.
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="typesCache"></param>
        /// <param name="dbScope"></param>
        /// <returns></returns>
        public static KrComponents GetKrComponents(
            Guid cardID,
            IKrTypesCache typesCache,
            IDbScope dbScope)
        {
            Guid? cardTypeID;

            using (dbScope.Create())
            {
                var query = dbScope.BuilderFactory
                    .Select()
                    .C(DocumentCommonInfo.CardTypeID)
                    .From(DocumentCommonInfo.Name).NoLock()
                    .Where().C(ID).Equals().P("CardID")
                    .Build();

                cardTypeID = dbScope.Db.SetCommand(
                        query,
                        dbScope.Db.Parameter("CardID", cardID))
                    .LogCommand()
                    .Execute<Guid?>();
            }

            return cardTypeID.HasValue
                ? GetKrComponents(cardID, cardTypeID.Value, typesCache, dbScope)
                : KrComponents.None;
        }

        /// <summary>
        /// Получить включенные компоненты типового решения с учетом типа документа.
        /// Тип документа получается из бд.
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="cardTypeID"></param>
        /// <param name="typesCache"></param>
        /// <param name="dbScope"></param>
        /// <returns></returns>
        public static KrComponents GetKrComponents(
            Guid cardID,
            Guid cardTypeID,
            IKrTypesCache typesCache,
            IDbScope dbScope)
        {
            var result = GetKrComponents(cardTypeID, typesCache);
            if (result == KrComponents.None)
            {
                return result;
            }

            if (result.Has(KrComponents.DocTypes))
            {
                var docTypeID = KrProcessSharedHelper.GetDocTypeID(cardID, dbScope);
                var docType = typesCache
                    .GetDocTypesAsync().ConfigureAwait(false).GetAwaiter().GetResult() // TODO async
                    ?.FirstOrDefault(x => x.ID == docTypeID);
                result = GetDocTypeComponents(docType);
            }
            return result;
        }

        /// <summary>
        /// Проверить на наличие необходимых настроек у карточки типового решения
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="cardTypeID"></param>
        /// <param name="docTypeID"></param>
        /// <param name="dbScope"></param>
        /// <param name="typesCache"></param>
        /// <param name="required"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static bool CheckKrComponents(
            Guid cardID,
            Guid cardTypeID,
            Guid? docTypeID,
            IDbScope dbScope,
            IKrTypesCache typesCache,
            KrComponents required,
            out string errorMessage)
        {
            KrComponents used = GetKrComponents(cardTypeID, typesCache);

            if (used.Has(KrComponents.DocTypes))
            {
                if (!docTypeID.HasValue)
                {
                    docTypeID = KrProcessSharedHelper.GetDocTypeID(cardID, dbScope);
                }
                if (!docTypeID.HasValue)
                {
                    errorMessage = LocalizationManager.GetString("KrMessages_UnableToGetSpecifiedDocType");
                    return false;
                }
                var docType = typesCache
                    .GetDocTypesAsync().ConfigureAwait(false).GetAwaiter().GetResult() // TODO async
                    ?.FirstOrDefault(x => x.ID == docTypeID);
                used = GetDocTypeComponents(docType);
            }

            if (used.Has(required))
            {
                errorMessage = string.Empty;
                return true;
            }

            bool result = true;
            string lostComponent = string.Empty;

            if (required.Has(KrComponents.Base) && used.HasNot(KrComponents.Base))
            {
                lostComponent += LocalizationManager.GetString("KrMessages_StandardSolution");
                result = false;
            }
            if (required.Has(KrComponents.Routes) && used.HasNot(KrComponents.Routes))
            {
                lostComponent += LocalizationManager.GetString("KrMessages_Approving");
                result = false;
            }
            if (required.Has(KrComponents.Registration) && used.HasNot(KrComponents.Registration))
            {
                lostComponent += (string.IsNullOrEmpty(lostComponent) ? "" : ", ")
                    + LocalizationManager.GetString("KrMessages_Registration");
                result = false;
            }
            if (required.Has(KrComponents.Resolutions) && used.HasNot(KrComponents.Resolutions))
            {
                lostComponent += (string.IsNullOrEmpty(lostComponent) ? "" : ", ")
                    + LocalizationManager.GetString("KrMessages_Resolutions");
                result = false;
            }

            if (result)
            {
                errorMessage = string.Empty;
            }
            else
            {
                errorMessage = string.Format(LocalizationManager.GetString("KrMessages_TypeDoesntUse"),
                    (docTypeID.HasValue ? LocalizationManager.GetString("KrMessages_DocType") : LocalizationManager.GetString("KrMessages_CardType")),
                    lostComponent);
            }

            return result;
        }

        #endregion

        #region private

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static KrComponents GetDocTypeComponents(IKrType docType)
        {
            var result = KrComponents.None;
            if (docType != null)
            {
                // Настройки типа карточки больше не беспокоят
                // Оставляем только основное
                result = KrComponents.Base | KrComponents.DocTypes;

                if (docType.UseApproving)
                {
                    result |= KrComponents.Routes;
                }

                if (docType.UseRegistration)
                {
                    result |= KrComponents.Registration;
                }

                if (docType.UseResolutions)
                {
                    result |= KrComponents.Resolutions;
                }
                if (docType.UseForum)
                {
                    result |= KrComponents.UseForum;
                }
            }

            return result;
        }

        #endregion
    }
}