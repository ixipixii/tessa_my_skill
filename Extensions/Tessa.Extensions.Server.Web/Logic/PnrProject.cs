using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Extensions.Server.Web.Models;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;
using Tessa.Extensions.Server.DataHelpers;

namespace Tessa.Extensions.Server.Web.Models
{
    public partial class PnrProjectRequest : PnrBaseRequest
    {
        public override async Task<Guid?> GetCardTypeID(IDbScope dbScope) => PnrCardTypes.PnrProjectTypeID;

        public override async Task<Guid?> GetCardIDAsync(IDbScope dbScope)
        {
            return await TryGetCardIDFromMdmKeyAsync(dbScope, this.Body?.classData?.MDM_Key, "PnrProjects");
        }

        public override string GetDescription()
        {
            return GetTrimmedString(this.Body?.classData?.Наименование);
        }

        public override async Task FillCardDataAsync(IDbScope dbScope, Card card, IValidationResultBuilder validationResult)
        {
            if (this.Body?.classData == null)
            {
                validationResult.AddError("Элемент \"classData\" не содержит значений.");
                return;
            }
            var project = this.Body?.classData;

            if (card.Sections.TryGetValue("PnrProjects", out var mainSection))
            {
                // MDM-Key
                mainSection.Fields["MDMKey"] = project.MDM_Key;

                // Код
                mainSection.Fields["Code"] = GetTrimmedString(project.Код);

                // Название
                mainSection.Fields["Name"] = GetTrimmedString(project.Наименование);

                // Описание
                mainSection.Fields["Description"] = GetTrimmedString(project.Комментарий);

                // Дата окончания
                mainSection.Fields["EndDate"] = GetDateFromString(project.ДатаОкончания);

                // Родительский проект
                if (!string.IsNullOrEmpty(project.Родитель))
                {
                    var parentProject = await PnrProjectHelper.GetProjectByMDM(dbScope, project.Родитель);
                    if (parentProject != null)
                    {
                        mainSection.Fields["ParentProjectID"] = parentProject.ID;
                        mainSection.Fields["ParentProjectName"] = parentProject.Name;
                    }
                    else
                    {
                        validationResult.AddError($"Не удалось найти проект. MDM-Key={project.Родитель}.");
                    }
                }
            }
        }
    }
}
