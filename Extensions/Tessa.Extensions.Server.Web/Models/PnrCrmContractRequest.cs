namespace Tessa.Extensions.Server.Web.Models
{
    /// <summary>
    /// Договор (интеграция с CRM)
    /// </summary>
    public partial class PnrCrmContractRequest
    {
        public string Number { get; set; }
        public string Title { get; set; }
        public string Annotation { get; set; }
        public string ProjectDate { get; set; }
        public string ProjectStartDate { get; set; }
        public string CRMUrl { get; set; }
        public string NeedApprove { get; set; }
        public string CorrespondentMDMKey { get; set; }
        public string PioneerContractType { get; set; }
        public string RubricLink { get; set; }
        public string Building { get; set; }
        public string MDMKey { get; set; }
        public string Flat { get; set; }
        public string Author { get; set; }
        public string PionerLegalEntity { get; set; }
        public string ActivityStateId { get; set; }
        public string ProjectUId { get; set; }
        public string ParentProjectUId { get; set; }
        public string CRMContractStatus { get; set; }
        public string CRMContractApprove { get; set; }
        public string MigrateFilesFromUrl { get; set; }
    }
}
