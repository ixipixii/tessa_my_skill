using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Server.Web.Models
{
    public class PnrFileResponse : PnrBaseResponse
    {
        public PnrFileResponse()
        {
            Value = new PnrFileResponseValue();
        }

        public PnrFileResponse(PnrBaseResponseStatusCode status, string message, PnrFileResponseValue value) : base(status, message)
        {
            Value = value;
        }

        public PnrFileResponseValue Value { get; set; }
    }

    public class PnrFileResponseValue
    {
        public string FileUrl { get; set; }
        public string FileId { get; set; }
        public string FileUId { get; set; }
    }
}
