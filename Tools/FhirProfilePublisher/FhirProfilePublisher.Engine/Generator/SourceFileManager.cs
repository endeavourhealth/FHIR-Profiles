using FhirProfilePublisher.Specification;
using Hl7.Fhir.V102;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Engine
{
    internal class SourceFileManager
    {
        private OutputPaths _outputPaths;
        private ResourceFileSet _profileSet;

        public SourceFileManager(OutputPaths outputPaths, ResourceFileSet profileSet)
        {
            _outputPaths = outputPaths;
            _profileSet = profileSet;
        }
        
        public void CopyXml()
        {
            foreach (ResourceFile file in _profileSet.Files)
                _outputPaths.WriteUtf8File(file.FileType, file.OutputXmlFilename, file.Xml);
        }

        public void GenerateJson()
        {
            foreach (ResourceFile file in _profileSet.Files)
                _outputPaths.WriteUtf8File(file.FileType, file.OutputJsonFilename, file.Json);
        }

        public void CreateRedirectsForProfileUrls()
        {
            foreach (ResourceFile file in _profileSet.Files)
            {
                Uri uri = new Uri(file.CanonicalUrl);

                string filename = System.IO.Path.GetFileName(uri.LocalPath);

                if (!string.IsNullOrWhiteSpace(filename))
                {
                    if (!file.OutputFileNames.Contains(filename))
                    {
                        // calculate relative path properly
                        string redirectHtml = Templates.Instance.GetRedirectPage("../" + file.OutputHtmlFilename);
                        FileHelper.WriteUtf8Text(_outputPaths.GetOutputPath(file.FileType, filename), redirectHtml);
                    }
                }
            }
        }
    }
}
