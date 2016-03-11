using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FhirProfilePublisher.Specification;

namespace FhirProfilePublisher.Engine
{
    internal class Templates
    {
        private static readonly string TemplatesResourceLocation = typeof(Styles).Assembly.GetName().Name + ".WebContent.Templates.";
        private static readonly string TemplatePageFileName = TemplatesResourceLocation + "Page.html";
        private static readonly string TemplateRedirectPageFileName = TemplatesResourceLocation + "RedirectPage.html";
        
        private static Templates _instance = null;

        public static Templates Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Templates();

                return _instance;
            }
        }

        private string _templatePage;
        private string _templateRedirectPage;

        public string PageHeader { get; set; }
        public string PageTitleSuffix { get; set; }

        private Templates()
        {
            _templatePage = ResourceHelper.LoadStringResource(TemplatePageFileName);
            _templateRedirectPage = ResourceHelper.LoadStringResource(TemplateRedirectPageFileName);
            PageHeader = "FHIR Implementation Guide (Draft)";
            PageTitleSuffix = string.Empty;
        }

        public string GetPage(string title, string content, string version, DateTime dateGenerated)
        {
            return _templatePage
                    .Replace("%PAGE_HEADER%", PageHeader)
                    .Replace("%TITLE%", title + PageTitleSuffix)
                    .Replace("%CONTENT%", content)
                    .Replace("%VERSION%", version)
                    .Replace("%DATE_GENERATED%", dateGenerated.ToString("dd-MMM-yyyy"));
        }

        public string GetRedirectPage(string url)
        {
            return _templateRedirectPage
                .Replace("%URL%", url);
        }
    }
}
