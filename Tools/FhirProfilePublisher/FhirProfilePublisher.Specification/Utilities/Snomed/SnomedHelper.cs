using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Engine
{
    public static class SnomedHelper
    {
        public static bool IsSnomedSystemUri(string uri)
        {
            return uri.ToLower().Contains("snomed.info/sct");
        }

        public static string GetBrowserUrl()
        {
            return GetBrowserUrl("138875005");
        }

        public static string GetBrowserUrl(string conceptId)
        {
            return string.Format(@"http://browser.ihtsdotools.org/?perspective=full&conceptId1={0}&acceptLicense=true&edition=uk-edition&release=v20151001&server=https://browser-aws-1.ihtsdotools.org/&langRefset=999001251000000103", conceptId);
        }
    }
}
