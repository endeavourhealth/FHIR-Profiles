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
    public static class Utilities
    {
        public static void LaunchBrowser(string filePath)
        {
            using (RegistryKey registrykey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false))
            {
                string browserPath = ((string)registrykey.GetValue(null, null)).Split('"')[1];
                Process.Start(browserPath, filePath);
            }
        }

        public static string GetRelativePath(string path1, string path2)
        {
            Uri pathUri = new Uri(path1);
            
            if (!path2.EndsWith(Path.DirectorySeparatorChar.ToString()))
                path2 += Path.DirectorySeparatorChar;

            Uri folderUri = new Uri(path2);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        internal static bool IsHttpUrl(string url)
        {
            Uri uri;

            bool result = Uri.TryCreate(url, UriKind.Absolute, out uri);

            return ((result) && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps));
        }

        internal static int ParseInt(string value, int valueInError)
        {
            int result;

            if (int.TryParse(value, out result))
                return result;

            return valueInError;
        }

        internal static string GetEnumDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            if (field == null)
                return null;

            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            if (attribute == null) 
                value.ToString();
            
            return attribute.Description;
        }
    }
}
