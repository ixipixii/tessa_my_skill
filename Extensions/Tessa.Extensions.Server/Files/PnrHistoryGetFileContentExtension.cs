using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tessa.Cards;
using Tessa.Cards.Extensions;
using Tessa.Extensions.Shared.Files;
using Tessa.Extensions.Shared.PnrCards;
using Tessa.Platform;
using Tessa.Platform.Data;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.Files
{
    public sealed class PnrHistoryGetFileContentExtension : CardGetFileContentExtension
    {
        private readonly ICardRepository cardRepository;
        private readonly IDbScope dbScope;

        public static readonly Guid FileHistory = PnrFiles.VirtualFileHistory;
        public static readonly Guid VersionFileHistory = PnrFiles.VersionVirtualFileHistory;

        public PnrHistoryGetFileContentExtension(
            IDbScope dbScope,
            ICardRepository cardRepository
            )
        {
            this.cardRepository = cardRepository;
            this.dbScope = dbScope;
        }

        public override async Task BeforeRequest(ICardGetFileContentExtensionContext context)

        {
            CardGetFileContentRequest request = context.Request;
            Guid? cardID;
            if (context.CardType == null
                || request.FileID != FileHistory
                || !(cardID = request.CardID).HasValue)
            {
                return;
            }

            if (request.VersionRowID == VersionFileHistory)
            {
                CardGetRequest getRequest = new CardGetRequest { CardID = cardID };
                CardGetResponse getResponse = await this.cardRepository.GetAsync(getRequest, context.CancellationToken);
                if (!getResponse.ValidationResult.IsSuccessful())
                {
                    context.ValidationResult.Add(getResponse.ValidationResult.Build());
                    return;
                }

                context.Response = new CardGetFileContentResponse();

                try
                {
                    var pnrProcesses = GetProcesses(context);

                    var dataSet = new System.Data.DataSet();

                    foreach (var process in pnrProcesses)
                    {
                        dataSet.Tables.Add(GetProcessStages(context, process));
                    }

                    var html = await CreateHTMLFile(dataSet, context);

                    string result;

                    result = html.ToString();

                    byte[] data = Encoding.UTF8.GetBytes(result);
                    context.ContentFuncAsync = ct => Task.FromResult((Stream)new MemoryStream(data));
                    // context.Response.Size = data.Length;
                    context.Response = new CardGetFileContentResponse { Size = data.Length }; ;
                }
                catch (Exception e)
                {
                    context.ValidationResult.Add(ValidationKey.Unknown, ValidationResultType.Error, e.Message, "", "", "", e.HelpLink + e.Source + e.StackTrace);
                }
            }
        }



        private List<PnrProcess> GetProcesses(ICardGetFileContentExtensionContext context)
        {
            var cardID = context.Request.CardID;

            List<PnrProcess> pnrProcesses = new List<PnrProcess>();

            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                pnrProcesses = db.SetCommand(
                    "declare @CardID uniqueidentifier; "
                    + "declare @ProccessID uniqueidentifier; "
                    + "set @CardID = @ContextCardID "
                    + "set @ProccessID = (select fsc.ID from FdSatelliteCommonInfo fsc where MainCardId = @CardID) "
                    + "select distinct ID, BasedOnProcessTemplateID as ProcessTemplateID, BasedOnProcessTemplateName as Name from FdProcessInstances "
                    + "where ID = @ProccessID "
                    + "and BasedOnProcessTemplateName <> 'Обработка тендерной документации'"
                    + "and BasedOnProcessTemplateName <> 'Ознакомление'",
                db.Parameter("ContextCardID", cardID))
                            .LogCommand()
                            .ExecuteList<PnrProcess>();
            }

            return pnrProcesses;
        }

        private System.Data.DataTable GetProcessStages(ICardGetFileContentExtensionContext context, PnrProcess pnrProcess)
        {
            System.Data.DataTable processStages = new System.Data.DataTable();

            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                processStages.Load(db.SetCommand(@"
                    SELECT 
                    Статус, 
                    Сотрудник, Роль, [Тип задания], 
                    --[В&nbsp;работе&nbsp;у], [В&nbsp;работе&nbsp;с], 
                    Решение, 
                    --Получено, 
                     Комментарий, [Дата получения], [Дата завершения]
                    --, [Дата завершения]
                    FROM
                    (
	                    SELECT
		                    fsi.StateName AS 'Статус',
		                    NULL AS 'Сотрудник',
		                    CASE
			                    WHEN dbo.FdGetRoleUsers(t.RoleID, 5) IS NULL 
				                    THEN t.RoleName
			                    ELSE
				                    CONCAT(t.RoleName, ' (', dbo.FdGetRoleUsers(t.RoleID, 5), ')')
			                    END
                    		AS 'Роль',
		                    fsi.BasedOnStageTemplateName AS 'Тип задания',
		                    --t.UserName AS 'В&nbsp;работе&nbsp;у',
		                    --t.InProgress AS 'В&nbsp;работе&nbsp;с',
		                    NULL AS 'Решение',
		                   -- NULL AS 'Получено',
		                    
		                    NULL AS 'Комментарий',
		                    t.Created,
		                    FORMAT(DATEADD(MINUTE, t.TimeZoneUtcOffsetMinutes, t.Created), 'dd.MM.yyyy HH:mm') AS 'Дата получения',
		                   NULL AS 'Дата завершения'
	                    FROM Tasks t -- активные задачи
	                    LEFT JOIN FdActiveTasks fat ON t.RowID = fat.TaskID -- fd активные задачи
	                    LEFT JOIN FdProcessInstances fdi ON fdi.RowID = fat.StageRowID  -- процесс fd активной задачи
	                    LEFT JOIN FdStageInstances fsi ON fsi.RowID = fat.StageRowID -- этап fd активной задачи
	                    WHERE fsi.id = @ProccessID
                    	
	                    UNION ALL
                    	
	                    SELECT
		                    fsi.StateName AS 'Статус',
		                    th.UserName AS 'Сотрудник',
		                    th.RoleName AS 'Роль',
		                    fsi.BasedOnStageTemplateName AS 'Тип задания',
		                   -- NULL AS 'В&nbsp;работе&nbsp;у',
		                    --NULL AS 'В&nbsp;работе&nbsp;с',
		                    th.OptionCaption AS 'Решение',
		                    --FORMAT(th.InProgress, 'dd.MM.yyyy HH:mm') AS 'Получено',
		                    th.Result AS 'Комментарий',
		                    th.Created,
		                    FORMAT(DATEADD(MINUTE, th.TimeZoneUtcOffsetMinutes, th.Created), 'dd.MM.yyyy HH:mm') AS 'Дата получения',
		                    FORMAT(DATEADD(MINUTE, th.TimeZoneUtcOffsetMinutes, th.Completed), 'dd.MM.yyyy HH:mm') AS 'Дата завершения'
                    	FROM TaskHistory th
	                    --FROM FdApprovalHistory fah
	                    --JOIN TaskHistory th ON th.RowID = fah.HistoryRecord -- все задачи
	                    JOIN FdCompletedTasks fct ON th.RowID = fct.TaskID -- fd завершенные задачи
	                    LEFT JOIN FdProcessInstances fdi ON fdi.RowID = fct.StageRowID  -- процесс fd завершенной задачи
	                    LEFT JOIN FdStageInstances fsi ON fsi.RowID = fct.StageRowID -- этап fd завершенной задачи
	                    WHERE fsi.id = @ProccessID
                    ) AS HistoryList
                    ORDER BY Created ",
                    db.Parameter("@ProccessID", pnrProcess.ID),
                    db.Parameter("@ProcessTemplateID", pnrProcess.ProcessTemplateID))
                    .LogCommand()
                    .ExecuteReader());
            }

            processStages.TableName = pnrProcess.Name;

            return processStages;
        }

        public class PnrProcess
        {
            public Guid? ID { get; set; }
            public Guid? ProcessTemplateID { get; set; }
            public string Name { get; set; }
        }

        public static string ConvertDataTableToHTML(System.Data.DataTable dataTable)
        {
            string html = "<br><b><font color=\"#36638e\">" + dataTable.TableName + "</font></b><br><br>"
                + "<table width=\"100%\" border=\"1px\" cellspacing=\"0\" bordercolor=\"#B9C4DA\" style=\"font-size: 13.5px;\">";

            html += "<tr>";
            for (int i = 0; i < dataTable.Columns.Count; i++)
                html += "<td align=\"center\">&nbsp;&nbsp;&nbsp;<b>" + dataTable.Columns[i].ColumnName + "</b>&nbsp;&nbsp;&nbsp;</td>";
            html += "</tr>";

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                html += "<tr>";
                for (int j = 0; j < dataTable.Columns.Count; j++)
                    html += "<td align=\"center\">&nbsp;&nbsp;&nbsp;" + dataTable.Rows[i][j].ToString() + "&nbsp;&nbsp;&nbsp;</td>";
                html += "</tr>";
            }
            html += "</table>";
            return html;
        }

        public static async Task<StringBuilder> CreateHTMLFile(System.Data.DataSet dataSet, ICardGetFileContentExtensionContext context)
        {
            string tab = "\t";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html>");
            sb.AppendLine("<head><meta http-equiv=Content-Type content='text/html; charset=utf-8'></head>");
            sb.AppendLine(tab + "<body>");

            sb.AppendLine(
                "<p align=\"right\">Подготовлено в системе Тесса ГК Пионер</p>"
                + "<hr color=\"#B9C4DA\" size=10>"
                + "<p align=\"right\">"
                + $"Дата печати: {DateTime.Now.ToString("dd.MM.yyyy")}"
                + "</p>");

            var headTable = await GetHeadTable(context);

            sb.AppendLine(
                "<table border=\"1px\" width=\"100%\" height=\"30\" bordercolor=\"#B9C4DA\" bgcolor=\"#B9C4DA\" style=\"font-size: 13.5px;\">" +
                "<tbody><tr><th colspan=\"4\">Общие сведения:</th></tr></table>" +
                "<table cellspacing=10 width=\"100%\" cellspacing=\"0\" bordercolor=\"#B9C4DA\" style=\"font-size: 13.5px;\">" +
                headTable.ToString() +
                "</table>" +
                "<table border=\"1px\" width=\"100%\" height=\"30\" bordercolor=\"#B9C4DA\" bgcolor=\"#B9C4DA\">" +
                "<tbody><tr><th colspan=\"4\">Журнал согласования:</th></tr></table>");

            foreach (System.Data.DataTable dt in dataSet.Tables)
            {
                sb.AppendLine(ConvertDataTableToHTML(dt));
            }

            sb.AppendLine(tab + "</body>");
            sb.AppendLine("</html>");

            return sb;
        }

        public static async Task<StringBuilder> GetHeadTable(ICardGetFileContentExtensionContext context)
        {
            var cardTypeID = context.CardType.ID;

            if (cardTypeID == PnrCardTypes.PnrTenderTypeID)
            {
                var tenderField = new TenderField();

                tenderField = GetTenderFields(context);

                StringBuilder headTableTender = new StringBuilder();

                if (tenderField != null)
                {
                    headTableTender.AppendLine(
                        $"<tr><td>Тип документа:</td><td><b>{tenderField.CardTypeCaption}</b></td><td>Статус:</td><td><b>{tenderField.StateName}</b></td></tr>" +
                        $"<tr><td>Тип договора:</td><td><b>{tenderField.ContractTypeName}</b></td><td>Стадия реализации:</td><td><b>{tenderField.StageName}</b></td></tr>" +
                        $"<tr><td>Номер:</td><td><b>{tenderField.FullNumber}</b></td><td>Проект:</td><td><b>{tenderField.ProjectName}</b></td></tr>" +
                        $"<tr><td>Дата документа:</td><td><b>{tenderField.CreationDate}</b></td><td>ЦФО:</td><td><b>{tenderField.CFOName}</b></td></tr>" +
                        $"<tr><td>Заголовок:</td><td><b>{tenderField.Subject}</b></td><td>Победитель тендера:</td><td><b>{tenderField.WinnerName}</b></td></tr>" +
                        $"<tr><td>Участники тендера:</td><td><b>{tenderField.PartnerName}</b></td><td>Протокол:</td><td><b>{tenderField.ProtocolNo}</b></td></tr>" +
                        $"<tr><td>Инициатор:</td><td><b>{tenderField.InitiatorName}</b></td><td>Статус тендера:</td><td><b>{tenderField.StatusName}</b></td></tr>");

                    return headTableTender;
                }
            }

            if (cardTypeID == PnrCardTypes.PnrOutgoingTypeID)
            {
                var outgoingField = new OutgoingField();

                outgoingField = GetOutgoingFields(context);

                StringBuilder headTableOutgoing = new StringBuilder();

                if (outgoingField != null)
                {
                    headTableOutgoing.AppendLine(
                        $"<tr><td>Тип документа:</td><td><b>{outgoingField.CardTypeCaption}</b></td><td>Статус:</td><td><b>{outgoingField.StateName}</b></td></tr>" +
                        $"<tr><td>Вид документа:</td><td><b>{outgoingField.DocumentKindName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Номер документа:</td><td><b>{outgoingField.FullNumber}</b></td><td>Дата документа:</td><td><b>{outgoingField.RegistrationDate}</b></td></tr>" +
                        $"<tr><td>Заголовок:</td><td><b>{outgoingField.Subject}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Организация ГК Пионер:</td><td><b>{outgoingField.OrganizationName}</b></td><td>Адресат:</td><td><b>{outgoingField.DestinationName}</b></td></tr>" +
                        $"<tr><td>Автор:</td><td><b>{outgoingField.AuthorName}</b></td><td>Подразделение:</td><td><b>{outgoingField.DepartmentName}</b></td></tr>" +
                        $"<tr><td>Подписант:</td><td><b>{outgoingField.SignatoryName}</b></td><td></td><td><b></b></td></tr>");

                    return headTableOutgoing;
                }
            }

            if (cardTypeID == PnrCardTypes.PnrContractTypeID)
            {
                var contractField = new ContractField();

                contractField = GetContractFields(context);

                StringBuilder headTableContract = new StringBuilder();

                if (contractField != null)
                {
                    headTableContract.AppendLine(
                        $"<tr><td>Тип документа:</td><td><b>{contractField.CardTypeCaption}</b></td><td>Статус:</td><td><b>{contractField.StateName}</b></td></tr>" +
                        $"<tr><td>Вид документа:</td><td><b>{contractField.KindName}</b></td><td>Стадия реализации:</td><td><b>{contractField.ImplementationStageName}</b></td></tr>" +
                        $"<tr><td>Номер:</td><td><b>{contractField.FullNumber}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Дата документа:</td><td><b>{contractField.ProjectDate}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Наименование:</td><td><b>{contractField.Subject}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Организация ГК Пионер:</td><td><b>{contractField.OrganizationName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Инициатор:</td><td><b>{contractField.AuthorName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Подписант:</td><td><b>{contractField.SignatoryName}</b></td><td></td><td><b></b></td></tr>");

                    return headTableContract;
                }
            }

            if (cardTypeID == PnrCardTypes.PnrRegulationTypeID)
            {
                var regulationField = new RegulationField();

                regulationField = GetRegulationFields(context);

                StringBuilder headTableRegulation = new StringBuilder();

                if (regulationField != null)
                {
                    headTableRegulation.AppendLine(
                        $"<tr><td>Тип документа:</td><td><b>{regulationField.CardTypeCaption}</b></td><td>Статус:</td><td><b>{regulationField.StateName}</b></td></tr>" +
                        $"<tr><td>Тема:</td><td><b>{regulationField.Subject}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Номер:</td><td><b>{regulationField.FullNumber}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Дата документа:</td><td><b>{regulationField.RegistrationDate}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Инициатор:</td><td><b>{regulationField.AuthorName}</b></td><td></td><td><b></b></td></tr>");

                    return headTableRegulation;
                }
            }

            if (cardTypeID == PnrCardTypes.PnrPowerAttorneyTypeID)
            {
                var powerAttorneyField = new PowerAttorneyField();

                powerAttorneyField = GetPowerAttorneyFields(context);

                StringBuilder headTablePowerAttorney = new StringBuilder();

                if (powerAttorneyField != null)
                {
                    headTablePowerAttorney.AppendLine(
                        $"<tr><td>Тип документа:</td><td><b>{powerAttorneyField.CardTypeCaption}</b></td><td>Статус:</td><td><b>{powerAttorneyField.StateName}</b></td></tr>" +
                        $"<tr><td>Номер документа:</td><td><b>{powerAttorneyField.FullNumber}</b></td><td>Дата документа:</td><td><b>{powerAttorneyField.ProjectDate}</b></td></tr>" +
                        $"<tr><td>Заголовок:</td><td><b>{powerAttorneyField.Subject}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Организация ГК Пионер:</td><td><b>{powerAttorneyField.OrganizationName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Инициатор:</td><td><b>{powerAttorneyField.AuthorName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Доверенное лицо:</td><td><b>{powerAttorneyField.PerformerName}</b></td><td></td><td><b></b></td></tr>");

                    return headTablePowerAttorney;
                }
            }

            if (cardTypeID == PnrCardTypes.PnrSupplementaryAgreementTypeID)
            {
                var supplementaryAgreementField = new SupplementaryAgreementsField();

                supplementaryAgreementField = GetSupplementaryAgreementsFields(context);

                StringBuilder headTableSupplementaryAgreements = new StringBuilder();

                if (supplementaryAgreementField != null)
                {
                    headTableSupplementaryAgreements.AppendLine(
                        $"<tr><td>Тип документа:</td><td><b>{supplementaryAgreementField.CardTypeCaption}</b></td><td>Статус:</td><td><b>{supplementaryAgreementField.StateName}</b></td></tr>" +
                        $"<tr><td>Вид документа:</td><td><b>{supplementaryAgreementField.KindName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Внутренний номер:</td><td><b>{supplementaryAgreementField.FullNumber}</b></td><td>Внешний номер:</td><td><b>{supplementaryAgreementField.ExternalNumber}</b></td></tr>" +
                        $"<tr><td>Дата документа:</td><td><b>{supplementaryAgreementField.ProjectDate}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Заголовок:</td><td><b>{supplementaryAgreementField.Subject}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Организация ГК Пионер:</td><td><b>{supplementaryAgreementField.OrganizationName}</b></td><td>Контрагент:</td><td><b>{supplementaryAgreementField.PartnerName}</b></td></tr>" +
                        $"<tr><td>Инициатор:</td><td><b>{supplementaryAgreementField.AuthorName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Подписант:</td><td><b>{supplementaryAgreementField.SignatoryName}</b></td><td></td><td><b></b></td></tr>");

                    return headTableSupplementaryAgreements;
                }
            }

            if (cardTypeID == PnrCardTypes.PnrActTypeID)
            {
                var actField = new ActsField();

                actField = GetActsFields(context);

                StringBuilder headTableActs = new StringBuilder();

                if (actField != null)
                {
                    headTableActs.AppendLine(
                        $"<tr><td>Тип документа:</td><td><b>{actField.CardTypeCaption}</b></td><td>Статус:</td><td><b>{actField.StateName}</b></td></tr>" +
                        $"<tr><td>Вид документа:</td><td><b>{actField.TypeName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Номер:</td><td><b>{actField.FullNumber}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Дата документа:</td><td><b>{actField.ProjectDate}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Заголовок:</td><td><b>{actField.Subject}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Организация ГК Пионер:</td><td><b>{actField.OrganizationName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Инициатор:</td><td><b>{actField.AuthorName}</b></td><td></td><td><b></b></td></tr>");

                    return headTableActs;
                }
            }

            if (cardTypeID == PnrCardTypes.PnrOrderTypeID)
            {
                var orderField = new OrderField();

                orderField = GetOrderFields(context);

                StringBuilder headTableOrder = new StringBuilder();

                if (orderField != null)
                {
                    headTableOrder.AppendLine(
                        $"<tr><td>Тип документа:</td><td><b>{orderField.CardTypeCaption}</b></td><td>Статус:</td><td><b>{orderField.StateName}</b></td></tr>" +
                        $"<tr><td>Вид документа:</td><td><b>{orderField.DocumentKindName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Внутренний Номер:</td><td><b>{orderField.FullNumber}</b></td><td>Дата документа:</td><td><b>{orderField.RegistrationDate}</b></td></tr>" +
                        $"<tr><td>Заголовок:</td><td><b>{orderField.Subject}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Организация ГК Пионер:</td><td><b>{orderField.OrganizationName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Инициатор:</td><td><b>{orderField.AuthorName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Подразделение:</td><td><b>{orderField.DepartmentName}</b></td><td></td><td><b></b></td></tr>");

                    return headTableOrder;
                }
            }

            if (cardTypeID == PnrCardTypes.PnrServiceNoteTypeID)
            {
                var serviceNoteField = new ServiceNoteField();

                serviceNoteField = GetServiceNoteFields(context);

                StringBuilder headTableServiceNote = new StringBuilder();

                if (serviceNoteField != null)
                {
                    headTableServiceNote.AppendLine(
                        $"<tr><td>Тип документа:</td><td><b>{serviceNoteField.CardTypeCaption}</b></td><td>Статус:</td><td><b>{serviceNoteField.StateName}</b></td></tr>" +
                        $"<tr><td>Вид документа:</td><td><b>{serviceNoteField.ServiceNoteTypeName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Внутренний Номер:</td><td><b>{serviceNoteField.FullNumber}</b></td><td>Дата документа:</td><td><b>{serviceNoteField.ProjectDate}</b></td></tr>" +
                        $"<tr><td>Заголовок:</td><td><b>{serviceNoteField.Subject}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Организация ГК Пионер:</td><td><b>{serviceNoteField.OrganizationName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Инициатор:</td><td><b>{serviceNoteField.AuthorName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Подразделение:</td><td><b>{serviceNoteField.DepartmentName}</b></td><td></td><td><b></b></td></tr>");

                    return headTableServiceNote;
                }
            }

            if (cardTypeID == PnrCardTypes.PnrTemplateTypeID)
            {
                var templatesField = new TemplatesField();

                templatesField = GetTemplatesFields(context);

                StringBuilder headTableTemplates = new StringBuilder();

                if (templatesField != null)
                {
                    headTableTemplates.AppendLine(
                        $"<tr><td>Тип документа:</td><td><b>{templatesField.CardTypeCaption}</b></td><td>Статус:</td><td><b>{templatesField.StateName}</b></td></tr>" +
                        $"<tr><td>Шаблон:</td><td><b>{templatesField.TemplateName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Внутренний Номер:</td><td><b>{templatesField.FullNumber}</b></td><td>Дата документа:</td><td><b>{templatesField.ProjectDate}</b></td></tr>" +
                        $"<tr><td>Заголовок:</td><td><b>{templatesField.Subject}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Организация ГК Пионер:</td><td><b>{templatesField.OrganizationName}</b></td><td></td><td><b></b></td></tr>" +
                        $"<tr><td>Инициатор:</td><td><b>{templatesField.AuthorName}</b></td><td></td><td><b></b></td></tr>");

                    return headTableTemplates;
                }
            }

            return new StringBuilder();
        }

        public class TenderField
        {
            public string CardTypeCaption { get; set; }
            public string StateName { get; set; }
            public string ContractTypeName { get; set; }
            public string StageName { get; set; }
            public string FullNumber { get; set; }
            public string ProjectName { get; set; }
            public DateTime? CreationDate { get; set; }
            public string CFOName { get; set; }
            public string Subject { get; set; }
            public string WinnerName { get; set; }
            public string PartnerName { get; set; }
            public string ProtocolNo { get; set; }
            public string InitiatorName { get; set; }
            public string StatusName { get; set; }
        }

        private static TenderField GetTenderFields(ICardGetFileContentExtensionContext context)
        {
            var cardID = context.Request.CardID;

            TenderField tenderField = new TenderField();

            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                tenderField = db.SetCommand(@"SELECT
                    dci.CardTypeCaption,
                    fsco.StateName,
                    pt.ContractTypeName,
                    pt.StageName,
                    dci.FullNumber,
                    pt.ProjectName,
                    dci.CreationDate,
                    pt.CFOName,
                    dci.Subject,
                    pt.WinnerName,
                    (SELECT STUFF(
                    (SELECT ', ' + PartnerName
                    FROM PnrTenderBidders
                    WHERE ID = pt.ID
                    FOR XML PATH ('')), 1, 1, '')) AS PartnerName,
                    pt.ProtocolNo,
                    pt.InitiatorName,
                    pt.StatusName
                    FROM PnrTenders AS pt 
                    JOIN DocumentCommonInfo dci ON dci.ID = pt.ID
                    LEFT JOIN FdSatelliteCommonInfo AS fsco ON fsco.MainCardId = pt.ID
                    WHERE pt.ID = @cardID",
                    db.Parameter("@cardID", cardID))
                    .LogCommand()
                    .Execute<TenderField>();
            }

            return tenderField;
        }

        public class OutgoingField
        {
            public string CardTypeCaption { get; set; }
            public string StateName { get; set; }
            public string DocumentKindName { get; set; }
            public string FullNumber { get; set; }
            public DateTime? RegistrationDate { get; set; }
            public string Subject { get; set; }
            public string OrganizationName { get; set; }
            public string DestinationName { get; set; }
            public string AuthorName { get; set; }
            public string DepartmentName { get; set; }
            public string SignatoryName { get; set; }
        }

        private static OutgoingField GetOutgoingFields(ICardGetFileContentExtensionContext context)
        {
            var cardID = context.Request.CardID;

            OutgoingField outgoingField = new OutgoingField();

            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                outgoingField = db.SetCommand(@"
                    SELECT
                    dci.CardTypeCaption,
                    fsco.StateName,
                    po.DocumentKindName,
                    dci.FullNumber,
                    po.RegistrationDate,
                    dci.Subject,
                    (SELECT STUFF(
                    (SELECT ', ' + OrganizationName
                    FROM PnrOutgoingOrganizations
                    WHERE ID = po.ID
                    FOR XML PATH ('')), 1, 1, '')) AS OrganizationName,
                    po.DestinationName,
                    dci.AuthorName,
                    po.DepartmentName,
                    po.SignatoryName
                    FROM PnrOutgoing AS po 
                    JOIN DocumentCommonInfo AS dci ON dci.ID = po.ID
                    JOIN FdSatelliteCommonInfo AS fsco ON fsco.MainCardId = po.ID
                    WHERE po.ID = @cardID",
                    db.Parameter("@cardID", cardID))
                    .LogCommand()
                    .Execute<OutgoingField>();
            }

            return outgoingField;
        }

        public class ContractField
        {
            public string CardTypeCaption { get; set; }
            public string StateName { get; set; }
            public string KindName { get; set; }
            public string ImplementationStageName { get; set; }
            public string FullNumber { get; set; }
            public DateTime? ProjectDate { get; set; }
            public string Subject { get; set; }
            public string OrganizationName { get; set; }
            public string AuthorName { get; set; }
            public string SignatoryName { get; set; }
        }

        private static ContractField GetContractFields(ICardGetFileContentExtensionContext context)
        {
            var cardID = context.Request.CardID;

            ContractField contractField = new ContractField();

            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                contractField = db.SetCommand(@"
                    SELECT
                    dci.CardTypeCaption,
                    fsco.StateName,
                    pc.KindName,
					pc.ImplementationStageName,
					dci.FullNumber,
                    pc.ProjectDate,
                    pc.Subject,
					pc.OrganizationName,
					dci.AuthorName,
					pc.SignatoryName
					FROM PnrContracts AS pc 
                    JOIN DocumentCommonInfo AS dci ON dci.ID = pc.ID
                    JOIN FdSatelliteCommonInfo AS fsco ON fsco.MainCardId = pc.ID
                    WHERE pc.ID = @cardID",
                    db.Parameter("@cardID", cardID))
                    .LogCommand()
                    .Execute<ContractField>();
            }

            return contractField;
        }

        public class RegulationField
        {
            public string CardTypeCaption { get; set; }
            public string StateName { get; set; }
            public string Subject { get; set; }
            public string FullNumber { get; set; }
            public DateTime? RegistrationDate { get; set; }
            public string AuthorName { get; set; }
        }

        private static RegulationField GetRegulationFields(ICardGetFileContentExtensionContext context)
        {
            var cardID = context.Request.CardID;

            RegulationField regulationField = new RegulationField();

            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                regulationField = db.SetCommand(@"
                    SELECT
                        dci.CardTypeCaption,
                        fsco.StateName,
                        dci.Subject,
                        dci.FullNumber,
                        pr.RegistrationDate,
                        dci.AuthorName
                    FROM DocumentCommonInfo AS dci
                    JOIN PnrRegulations AS pr 
                        ON dci.ID = pr.ID
                    LEFT JOIN FdSatelliteCommonInfo AS fsco
                        ON fsco.MainCardId = pr.ID
                    WHERE pr.ID = @cardID",
                    db.Parameter("@cardID", cardID))
                    .LogCommand()
                    .Execute<RegulationField>();
            }

            return regulationField;
        }

        public class PowerAttorneyField
        {
            public string CardTypeCaption { get; set; }
            public string StateName { get; set; }
            public string FullNumber { get; set; }
            public DateTime? ProjectDate { get; set; }
            public string Subject { get; set; }
            public string OrganizationName { get; set; }
            public string AuthorName { get; set; }
            public string PerformerName { get; set; }
        }

        private static PowerAttorneyField GetPowerAttorneyFields(ICardGetFileContentExtensionContext context)
        {
            var cardID = context.Request.CardID;

            PowerAttorneyField powerAttorneyField = new PowerAttorneyField();

            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                powerAttorneyField = db.SetCommand(@"
                    SELECT
                        dci.CardTypeCaption,
                        fsco.StateName,
                        dci.FullNumber,
                        pa.ProjectDate,
                        dci.Subject,
                        pa.OrganizationName,
                        dci.AuthorName,
                        (SELECT STUFF(
                            (SELECT ', ' + UserName
                            FROM Performers
                            WHERE ID = pa.ID
                            FOR XML PATH ('')), 1, 1, '')) AS PerformerName
                        FROM
                            DocumentCommonInfo AS dci
                        JOIN PnrPowerAttorney AS pa ON dci.ID = pa.ID
                            LEFT JOIN FdSatelliteCommonInfo AS fsco ON fsco.MainCardId = pa.ID
                    WHERE pa.ID = @cardID",
                    db.Parameter("@cardID", cardID))
                    .LogCommand()
                    .Execute<PowerAttorneyField>();
            }

            return powerAttorneyField;
        }

        public class SupplementaryAgreementsField
        {
            public string CardTypeCaption { get; set; }
            public string StateName { get; set; }
            public string KindName { get; set; }
            public string FullNumber { get; set; }
            public string ExternalNumber { get; set; }
            public DateTime? ProjectDate { get; set; }
            public string Subject { get; set; }
            public string OrganizationName { get; set; }
            public string PartnerName { get; set; }
            public string AuthorName { get; set; }
            public string SignatoryName { get; set; }
        }

        private static SupplementaryAgreementsField GetSupplementaryAgreementsFields(ICardGetFileContentExtensionContext context)
        {
            var cardID = context.Request.CardID;

            var supplementaryAgreementsield = new SupplementaryAgreementsField();

            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                supplementaryAgreementsield = db.SetCommand(@"
                    SELECT
                        dci.CardTypeCaption,
                        fsco.StateName,
                        psa.KindName,
                        dci.FullNumber,
                        psa.ExternalNumber,
                        psa.ProjectDate,
                        dci.Subject,
                        psa.OrganizationName,
                        psa.PartnerName,
                        dci.AuthorName,
                        psa.SignatoryName
                    FROM
                        DocumentCommonInfo AS dci
                    JOIN PnrSupplementaryAgreements AS psa 
                        ON dci.ID = psa.ID
                    LEFT JOIN FdSatelliteCommonInfo AS fsco 
                        ON fsco.MainCardId = psa.ID
                    WHERE psa.ID = @cardID",
                    db.Parameter("@cardID", cardID))
                    .LogCommand()
                    .Execute<SupplementaryAgreementsField>();
            }

            return supplementaryAgreementsield;
        }

        public class ActsField
        {
            public string CardTypeCaption { get; set; }
            public string StateName { get; set; }
            public string TypeName { get; set; }
            public string FullNumber { get; set; }
            public DateTime? ProjectDate { get; set; }
            public string Subject { get; set; }
            public string OrganizationName { get; set; }
            public string AuthorName { get; set; }
        }

        private static ActsField GetActsFields(ICardGetFileContentExtensionContext context)
        {
            var cardID = context.Request.CardID;

            var actsField = new ActsField();

            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                actsField = db.SetCommand(@"
                    SELECT
                        dci.CardTypeCaption,
                        fsco.StateName,
                        pa.TypeName,
                        dci.FullNumber,
                        pa.ProjectDate,
                        dci.Subject,
                        pa.OrganizationName,
                        dci.AuthorName
                    FROM
                        DocumentCommonInfo AS dci
                    JOIN PnrActs AS pa 
                        ON dci.ID = pa.ID
                    LEFT JOIN FdSatelliteCommonInfo AS fsco 
                        ON fsco.MainCardId = pa.ID
                    WHERE pa.ID = @cardID",
                    db.Parameter("@cardID", cardID))
                    .LogCommand()
                    .Execute<ActsField>();
            }

            return actsField;
        }

        public class OrderField
        {
            public string CardTypeCaption { get; set; }
            public string StateName { get; set; }
            public string DocumentKindName { get; set; }
            public string FullNumber { get; set; }
            public DateTime? RegistrationDate { get; set; }
            public string Subject { get; set; }
            public string OrganizationName { get; set; }
            public string AuthorName { get; set; }
            public string DepartmentName { get; set; }
        }

        private static OrderField GetOrderFields(ICardGetFileContentExtensionContext context)
        {
            var cardID = context.Request.CardID;

            var orderField = new OrderField();

            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                orderField = db.SetCommand(@"
                    SELECT
                        dci.CardTypeCaption,
                        fsco.StateName,
                        po.DocumentKindName,
                        dci.FullNumber,
                        po.RegistrationDate,
                        dci.Subject,
                        po.OrganizationName,
                        dci.AuthorName,
                        po.DepartmentName
                    FROM
                        DocumentCommonInfo AS dci
                    JOIN PnrOrder AS po 
                        ON dci.ID = po.ID
                    LEFT JOIN FdSatelliteCommonInfo AS fsco 
                        ON fsco.MainCardId = po.ID
                    WHERE po.ID = @cardID",
                    db.Parameter("@cardID", cardID))
                    .LogCommand()
                    .Execute<OrderField>();
            }

            return orderField;
        }

        public class ServiceNoteField
        {
            public string CardTypeCaption { get; set; }
            public string StateName { get; set; }
            public string ServiceNoteTypeName { get; set; }
            public string FullNumber { get; set; }
            public DateTime? ProjectDate { get; set; }
            public string Subject { get; set; }
            public string OrganizationName { get; set; }
            public string AuthorName { get; set; }
            public string DepartmentName { get; set; }
        }

        private static ServiceNoteField GetServiceNoteFields(ICardGetFileContentExtensionContext context)
        {
            var cardID = context.Request.CardID;

            var serviceNoteField = new ServiceNoteField();

            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                serviceNoteField = db.SetCommand(@"
                    SELECT
                        dci.CardTypeCaption,
                        fsco.StateName,
                        psn.ServiceNoteTypeName,
                        dci.FullNumber,
                        psn.ProjectDate,
                        dci.Subject,
                        psn.OrganizationName,
                        dci.AuthorName,
                        psn.DepartmentName
                    FROM
                        DocumentCommonInfo AS dci
                    JOIN PnrServiceNote AS psn 
                        ON dci.ID = psn.ID
                    LEFT JOIN FdSatelliteCommonInfo AS fsco
                        ON fsco.MainCardId = psn.ID
                    WHERE psn.ID = @cardID",
                    db.Parameter("@cardID", cardID))
                    .LogCommand()
                    .Execute<ServiceNoteField>();
            }

            return serviceNoteField;
        }

        public class TemplatesField
        {
            public string CardTypeCaption { get; set; }
            public string StateName { get; set; }
            public string TemplateName { get; set; }
            public string FullNumber { get; set; }
            public DateTime? ProjectDate { get; set; }
            public string Subject { get; set; }
            public string OrganizationName { get; set; }
            public string AuthorName { get; set; }
        }

        private static TemplatesField GetTemplatesFields(ICardGetFileContentExtensionContext context)
        {
            var cardID = context.Request.CardID;

            var templatesField = new TemplatesField();

            using (context.DbScope.Create())
            {
                var db = context.DbScope.Db;
                templatesField = db.SetCommand(@"
                    SELECT
                        dci.CardTypeCaption,
                        fsco.StateName,
                        pt.TemplateName,
                        dci.FullNumber,
                        pt.ProjectDate,
                        dci.Subject,
                        pt.OrganizationName,
                        dci.AuthorName
                    FROM
                        DocumentCommonInfo AS dci
                    JOIN PnrTemplates AS pt 
                        ON dci.ID = pt.ID
                    LEFT JOIN FdSatelliteCommonInfo AS fsco
                        ON fsco.MainCardId = pt.ID
                    WHERE pt.ID = @cardID",
                    db.Parameter("@cardID", cardID))
                    .LogCommand()
                    .Execute<TemplatesField>();
            }

            return templatesField;
        }
    }
}
