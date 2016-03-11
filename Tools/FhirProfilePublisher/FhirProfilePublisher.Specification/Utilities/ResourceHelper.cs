using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Specification
{
    public class ResourceHelper
    {
        public static string LoadStringResource(string resourceName)
        {
            using (Stream stream = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                    return reader.ReadToEnd();
        }

        public static void WriteResourceToDisk(string resourceName, string filePath)
        {
            using (Stream resource = Assembly.GetCallingAssembly().GetManifestResourceStream(resourceName))
                using (Stream output = File.OpenWrite(filePath))
                    resource.CopyTo(output);
        }
    }
}
