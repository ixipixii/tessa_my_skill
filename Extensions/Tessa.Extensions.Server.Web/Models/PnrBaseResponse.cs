using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Server.Web.Models
{
    public enum PnrBaseResponseStatusCode
    {
        Default = 0,
        Success = 100,
        Warning = 200,
        Error = 300
    }

    public class PnrBaseResponse
    {
        public PnrBaseResponse()
        {
        }

        public PnrBaseResponse(PnrBaseResponseStatusCode status, string message)
        {
            Status = status;
            Message = message;
        }

        public PnrBaseResponseStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}
