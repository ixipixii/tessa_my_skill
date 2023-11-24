﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Default.Shared.Workflow.KrPermissions;
using Tessa.Localization;
using Tessa.Platform.Collections;
using Tessa.UI;
using Tessa.UI.Cards;
using Tessa.UI.Cards.Tasks;

namespace Tessa.Extensions.Default.Client.Cards
{
    public sealed class KrPermissionsMandatoryStoreExtension : CardStoreExtension
    {
        #region Base Overrides

        public override async Task AfterRequest(ICardStoreExtensionContext context)
        {
            if (!context.RequestIsSuccessful
                && UIContext.Current.CardEditor != null
                && context.Response.Info.TryGetValue(KrPermissionsHelper.FailedMandatoryRulesKey, out var rulesObj)
                && rulesObj is List<object> rules)
            {
                var cardModel = UIContext.Current.CardEditor.CardModel;
                var failRules = rules.Select(x => new KrPermissionMandatoryRuleStorage((Dictionary<string, object>)x)).ToList();

                ValidateFromRules(cardModel, failRules);

                var requestTasks = context.Request.Card.Tasks;
                var taskItems = cardModel.TryGetTaskItems();
                if (requestTasks.Count > 0
                    && taskItems != null
                    && taskItems.Count > 0)
                {
                    foreach(var task in requestTasks)
                    {
                        if (task.Action == CardTaskAction.Complete
                            && taskItems.TryFirst(x => x is TaskViewModel taskVM && taskVM.TaskModel.CardTask.RowID == task.RowID, out var taskItem))
                        {
                            ValidateFromRules(((TaskViewModel)taskItem).TaskModel, failRules);
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private void ValidateFromRules(ICardModel cardModel, List<KrPermissionMandatoryRuleStorage> failRules)
        {
            foreach (var controlViewModel in cardModel.ControlBag)
            {
                var sourceInfo = controlViewModel.CardTypeControl.GetSourceInfo();
                if (sourceInfo != null
                    && failRules.Any(x =>
                        x.SectionID == sourceInfo.SectionID
                        && (x.ColumnIDs.Count == 0
                            || sourceInfo.ColumnIDs.Any(y => x.ColumnIDs.Contains(y)))))
                {
                    controlViewModel.ValidationFunc = (c) =>
                    {
                        if (c.HasEmptyValue())
                        {
                            return LocalizationManager.Format("$KrPermissions_MandatoryControlTemplate", controlViewModel.Caption);
                        }

                        return null;
                    };
                    controlViewModel.NotifyUpdateValidation();
                }
            }
        }

        #endregion
    }
}
