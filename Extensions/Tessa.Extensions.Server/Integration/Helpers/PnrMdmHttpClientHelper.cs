using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Tessa.Platform.Validation;

namespace Tessa.Extensions.Server.Integration.Helpers
{
    public static class PnrMdmHttpClientHelper
    {
        private const string LogSeparator = "====================================================";

        public static bool PostXMLData(ILogger logger, IValidationResultBuilder validationResult, string serviceUrl, string requestXml, out string responseResult)
        {
            responseResult = null;
            try
            {
                logger.Info(LogSeparator);
                logger.Info("XML-запрос.");
                logger.Info(Environment.NewLine + requestXml);

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceUrl);
                byte[] bytes;
                bytes = Encoding.UTF8.GetBytes(requestXml);
                request.ContentType = "application/xml; encoding='utf-8'";
                request.ContentLength = bytes.Length;
                request.Method = "POST";
                request.Timeout = 10000;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();

                logger.Info($"Получен следующий ответ: {response.StatusCode}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    responseResult = new StreamReader(responseStream).ReadToEnd();

                    return true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                validationResult.AddError(ex.ToString());
            }
            finally
            {
                logger.Info(LogSeparator);
            }
            return false;
        }
    }
}
