using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Engine
{
    internal abstract class ResourceFile
    {
        protected const string HtmlExtension = "html";
        protected const string XmlExtension = "xml";
        protected const string JsonExtension = "json";
        
        public string Xml { get; protected set; }
        public string Json { get; protected set; }

        public abstract OutputFileType FileType { get; }
        public abstract string Name { get; }
        public abstract string CanonicalUrl { get; }
        public abstract string OutputHtmlFilename { get; }
        public abstract string OutputXmlFilename { get; }
        public abstract string OutputJsonFilename { get; }
        public abstract ResourceMaturity Maturity { get; }

        public string[] OutputFileNames
        {
            get
            {
                return new string[]
                {
                    OutputHtmlFilename,
                    OutputXmlFilename,
                    OutputJsonFilename
                };
            }
        }

        public virtual Link Link
        {
            get
            {
                return new Link
                (
                    url: OutputHtmlFilename,
                    display: Name
                );
            }
        }
    }
}
