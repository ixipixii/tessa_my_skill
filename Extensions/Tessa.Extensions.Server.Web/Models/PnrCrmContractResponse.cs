using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Server.Web.Models
{
    public class PnrCrmContractResponse : PnrBaseResponse
    {
        public PnrCrmContractResponse()
        {
            Value = new PnrCrmContractResponseValue();
        }

        public PnrCrmContractResponse(PnrBaseResponseStatusCode status, string message, PnrCrmContractResponseValue value) : base(status, message)
        {
            Value = value;
        }

        public PnrCrmContractResponseValue Value { get; set; }
    }

    public class PnrCrmContractResponseValue
    {
        public string ProjectUrl { get; set; }
        public string ProjectDisplayFormUrl { get; set; }
        public string ProjectId { get; set; }
        public string ProjectUId { get; set; }
        public string ProjectFilesFolderUId { get; set; }
    }
}
