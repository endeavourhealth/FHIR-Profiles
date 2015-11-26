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
        public static string ReadInputFile(string inputFilename)
        {
            if (!File.Exists(inputFilename))
                throw new FileNotFoundException(string.Format("Could not find {0}", inputFilename));

            return File.ReadAllText(inputFilename);
        }

        public static string LoadStringResource(string resourceName)
        {
            using (Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                    return reader.ReadToEnd();
        }

        public static void WriteUtf8Text(string path, string fileContents)
        {
            File.WriteAllText(path, fileContents, Encoding.UTF8);
        }

        public static void WriteText(string path, string fileContents)
        {
            File.WriteAllText(path, fileContents);
        }

        public static void WriteResourceToDisk(string resourceName, string filePath)
        {
            using (Stream resource = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName))
                using (Stream output = File.OpenWrite(filePath))
                    resource.CopyTo(output);
        }

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

        public static string UpperCaseFirstCharacter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        internal static void EnsureDirectory(string outputDirectory)
        {
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);
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
