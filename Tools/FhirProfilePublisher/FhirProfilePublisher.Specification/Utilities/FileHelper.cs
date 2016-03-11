using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Specification
{
    public class FileHelper
    {
        public static string ReadInputFile(string inputFilename)
        {
            if (!File.Exists(inputFilename))
                throw new FileNotFoundException(string.Format("Could not find {0}", inputFilename));

            return File.ReadAllText(inputFilename);
        }

        public static void WriteUtf8Text(string path, string fileContents)
        {
            File.WriteAllText(path, fileContents, Encoding.UTF8);
        }

        public static void WriteText(string path, string fileContents)
        {
            File.WriteAllText(path, fileContents);
        }

        public static void EnsureDirectory(string outputDirectory)
        {
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);
        }
    }
}
