using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Engine
{
    public class TextContent
    {
        public string HeaderText { get; set; }
        public string PageTitleSuffix { get; set; }
        public string FooterText { get; set; }
        public string IndexPageHtml { get; set; }
    }
}
