using FhirProfilePublisher.Specification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Engine
{
    internal class OutputPaths
    {
        private Dictionary<OutputFileType, string> _relativePaths = new Dictionary<OutputFileType, string>();
        private string _outputDirectory;

        public OutputPaths(string outputDirectory, string stylesRelativePath, string imagesRelativePath, string scriptsRelativePath, string structureDefinitionPath, string valueSetPath)
        {
            _outputDirectory = outputDirectory;
            _relativePaths.Add(OutputFileType.Html, string.Empty);
            _relativePaths.Add(OutputFileType.Image, imagesRelativePath);
            _relativePaths.Add(OutputFileType.Script, scriptsRelativePath);
            _relativePaths.Add(OutputFileType.StructureDefinition, structureDefinitionPath);
            _relativePaths.Add(OutputFileType.Style, stylesRelativePath);
            _relativePaths.Add(OutputFileType.ValueSet, valueSetPath);
        }

        private string GetRelativePath(OutputFileType fileType)
        {
            return _relativePaths[fileType];
        }

        public string GetRelativePath(OutputFileType fileType, string fileName)
        {
            return GetRelativePath(fileType) + "/" + fileName;
        }

        public string GetOutputPath(OutputFileType fileType)
        {
            string path = Path.Combine(_outputDirectory, GetRelativePath(fileType));

            FileHelper.EnsureDirectory(path);

            return path;
        }
        
        public string GetOutputPath(OutputFileType fileType, string fileName)
        {
            return Path.Combine(GetOutputPath(fileType), fileName);
        }

        public void WriteUtf8File(OutputFileType fileType, string fileName, string fileContents)
        {
            FileHelper.WriteUtf8Text(GetOutputPath(fileType, fileName), fileContents);
        }
    }
}
