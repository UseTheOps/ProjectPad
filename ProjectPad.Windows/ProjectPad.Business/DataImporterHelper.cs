using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProjectPad.Business
{
    public class DataImporterHelper
    {
        public enum KnowUrlKind
        {
            Unknown,
            AzureDevOpsWorkItem
        }

        public class UrlDataContent
        {
            public KnowUrlKind UrlKind { get; set; }
            public Uri Uri { get; set; }
        }

        public static async Task<UrlDataContent> InspectUrl(Uri uri)
        {
            UrlDataContent retVal = new UrlDataContent() { Uri = uri, UrlKind = KnowUrlKind.Unknown };
            string host = uri.Host.ToLowerInvariant();

            if (host.Equals("dev.azure.com"))
                retVal.UrlKind = KnowUrlKind.AzureDevOpsWorkItem;


            return retVal;
        }

    }
}
