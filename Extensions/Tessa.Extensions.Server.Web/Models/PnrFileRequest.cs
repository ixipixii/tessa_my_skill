using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tessa.Extensions.Server.Web.Models
{
    public class PnrFileRequest
    {
        public string FileName { get; set; }
        public string FolderUId { get; set; }
        public string UniqueId { get; set; }
        public string Id { get; set; }
        public string User { get; set; }
        public string DocumentBody { get; set; }

        public Guid? GetCardID()
        {
            if (Guid.TryParse(this.FolderUId, out var result))
            {
                return result;
            }
            return null;
        }

        public Guid? GetExternalFileID()
        {
            if (Guid.TryParse(this.UniqueId, out var result))
            {
                return result;
            }
            return null;
        }

        public virtual PnrFileResponse GetSuccessResult(Logger logger, string message, Guid fileID, string fileUrl)
        {
            if (!string.IsNullOrEmpty(message))
            {
                logger.Info(message);
            }
            var fileResponse = new PnrFileResponseValue()
            {
                FileId = fileID.ToString(),
                FileUId = fileID.ToString(),
                FileUrl = fileUrl
            };
            return new PnrFileResponse(PnrBaseResponseStatusCode.Success, message, fileResponse);
        }

        public virtual PnrFileResponse GetErrorResult(Logger logger, string message, string prefix = null)
        {
            if (!string.IsNullOrEmpty(message))
            {
                logger.Error(prefix + message);
            }
            return new PnrFileResponse(PnrBaseResponseStatusCode.Error, message, new PnrFileResponseValue());
        }
    }
}
