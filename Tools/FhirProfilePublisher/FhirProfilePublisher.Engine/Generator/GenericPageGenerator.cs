using FhirProfilePublisher.Specification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Engine
{
    internal class GenericPageGenerator
    {
        private OutputPaths _outputPaths;

        public GenericPageGenerator(OutputPaths outputPaths)
        {
            _outputPaths = outputPaths;
        }

        internal void Generate(string fileName, string title, string content)
        {
            string html = GenerateHtml(title, content);
            FileHelper.WriteUtf8Text(_outputPaths.GetOutputPath(OutputFileType.Html, fileName), html);
        }

        private string GenerateHtml(string title, string content)
        {
            return Templates.Instance.GetPage(title, content, "0.1", DateTime.Now);
        }
    }
}
