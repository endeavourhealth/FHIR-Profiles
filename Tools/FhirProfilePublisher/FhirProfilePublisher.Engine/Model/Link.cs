using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FhirProfilePublisher.Engine
{
    internal class Link
    {
        public Link(string url, string display)
        {
            Url = url;
            Display = display;
        }

        public string Url { get; private set; }
        public string Display { get; private set; }

        public XElement GetAsXElement()
        {
            return Html.A(Url, Display);
        }
    }
}
