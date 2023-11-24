using System;
using Microsoft.Exchange.WebServices.Data;

namespace Tessa.Extensions.Default.Chronos.Notices
{
    public class ExchangeServiceSettings
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public Uri ProxyAddress { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }
        public ExchangeVersion Version { get; set; }
    }
}
