using System;
using System.Net;
using Microsoft.Exchange.WebServices.Data;

namespace Tessa.Extensions.Default.Chronos.Notices
{
    public static class ExchangeServiceHelper
    {
        public static ExchangeService CreateExchangeService(ExchangeServiceSettings settings)
        {
            ExchangeService service = new ExchangeService(settings.Version);
            
            if (string.IsNullOrEmpty(settings.Password))
            {
                service.UseDefaultCredentials = true;
            }
            else
            {
                service.Credentials = new NetworkCredential(settings.User, settings.Password);
            }

            if (string.IsNullOrEmpty(settings.Server))
            {
                service.AutodiscoverUrl(settings.User, RedirectionUrlValidationCallback);
            }
            else
            {
                service.Url = new Uri(settings.Server);
            }

            if (settings.ProxyAddress != null)
            {
                service.WebProxy = 
                    new WebProxy(
                        settings.ProxyAddress, 
                        false,              // false - не обходить прокси для списка локальных адресов
                        null,               // null - список локальных адресов для обхода
                        new NetworkCredential(
                            string.IsNullOrWhiteSpace(settings.ProxyUser) ? null : settings.ProxyUser, 
                            string.IsNullOrWhiteSpace(settings.ProxyPassword) ? null : settings.ProxyPassword));
            }

            return service;
        }

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
                result = true;
            return result;
        }
    }
}
