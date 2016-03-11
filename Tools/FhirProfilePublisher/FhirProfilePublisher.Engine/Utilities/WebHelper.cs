using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace FhirProfilePublisher.Engine
{
    public static class WebHelper
    {
        public static void LaunchBrowser(string filePath)
        {
            using (RegistryKey registrykey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false))
            {
                string browserPath = ((string)registrykey.GetValue(null, null)).Split('"')[1];
                Process.Start(browserPath, filePath);
            }
        }

        internal static bool IsHttpUrl(string url)
        {
            Uri uri;

            bool result = Uri.TryCreate(url, UriKind.Absolute, out uri);

            return ((result) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps));
        }
    }
}
