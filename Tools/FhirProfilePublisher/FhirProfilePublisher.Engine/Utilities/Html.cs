using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FhirProfilePublisher.Engine
{
    internal static class Html
    {
        public static XElement HtmlDocument(object content)
        {
            return Element("html", content);
        }

        public static XElement Head(object content)
        {
            return Element("head", content);
        }

        public static XElement Title(object content)
        {
            return Element("title", content);
        }

        public static XElement LinkStylesheet(string url)
        {
            return Element("link", new object[]
            {
                Attribute("rel", "stylesheet"),
                Attribute("href", url)
            });
        }

        public static XElement Script(string src)
        {
            return Html.Element("script", new object[]
            {
                Html.Attribute("src", src),
                string.Empty
            });
        }

        public static XElement Script(string scriptType, string scriptContent)
        {
            return Html.Element("script", new object[]
            {
                Html.Attribute("type", scriptType),
                scriptContent
            });
        }

        public static XElement Body(object content)
        {
            return Element("body", content);
        }

        public static XElement Label(object content)
        {
            return Element("label", content);
        }

        public static XElement Input(string inputType, object content)
        {
            return Element("input", new object[]
            {
                Html.Attribute("type", "checkbox"),
                content
            });
        }

        public static XElement H1(object content)
        {
            return Element("h1", content);
        }

        public static XElement H3(object content)
        {
            return Element("h3", content);
        }

        public static XElement H4(object content)
        {
            return Element("h4", content);
        }

        public static XElement Img(string source)
        {
            return Element("img", Attribute("src", source));
        }

        public static XElement Img(string source, string cssClass, string alt, string title)
        {
            return Element("img", new object[]
            {
                Attribute("src", source),
                Attribute("class", cssClass),
                Attribute("alt", alt),
                string.IsNullOrWhiteSpace(title) ? null : Attribute("title", title)
            });
        }

        public static XElement Div(object content)
        {
            return Element("div", content);
        }

        public static XElement Div(string cssClass, object content)
        {
            return Html.Element(Html.Div, cssClass, content);
        }

        public static XElement Ul(object content)
        {
            return Element("ul", content);
        }

        public static XElement Li(object content)
        {
            return Element("li", content);
        }

        public static XAttribute Style(string value)
        {
            return Attribute("style", value);
        }

        public static XElement Span(object content)
        {
            return Element("span", content);
        }
        
        public static XElement Span(string cssClass, object content)
        {
            return Element(Html.Span, cssClass, content);
        }

        public static XElement B(object content)
        {
            return Element("b", content);
        }

        public static XAttribute Role(string role)
        {
            return Attribute("role", role);
        }

        public static XElement Table(object content)
        {
            return Element("table", content);
        }

        public static XElement THead(object content)
        {
            return Element("thead", content);
        }

        public static XElement Th(object content)
        {
            return Element("th", content);
        }

        public static XElement Th(string cssClass, object content)
        {
            return Html.Element(Html.Th, cssClass, content);
        }

        public static XElement TBody(object content)
        {
            return Element("tbody", content);
        }

        public static XElement Tr(object content)
        {
            return Element("tr", content);
        }

        public static XElement Td(object content)
        {
            return Element("td", content);
        }

        public static XElement Td(string cssClass, object content)
        {
            return Html.Element(Html.Td, cssClass, content);
        }

        public static XElement Br()
        {
            return Element("br", null);
        }

        public static XElement I(object content)
        {
            return Element("i", content);
        }

        public static XElement S(object content)
        {
            return Element("s", content);
        }

        public static XElement P(object content)
        {
            return Element("p", content);
        }

        public static XAttribute Class(string value)
        {
            return Attribute("class", value);
        }

        public static XAttribute Id(string value)
        {
            return Attribute("id", value);
        }

        public static XElement A(object content)
        {
            return Element("a", content);
        }

        public static XElement Abbr(object content)
        {
            return Element("abbr", content);
        }

        public static XElement Pre(object content)
        {
            return Element("pre", content);
        }

        public static XElement Code(object content)
        {
            return Element("code", content);
        }

        public static XElement A(Link link)
        {
            return A(link.Url, link.Display);
        }

        public static XElement A(string href, object content)
        {
            return A(new object[]
            {
                Attribute("href", href),
                content
            });
        }

        private static XElement Element(string tag, object content)
        {
            return new XElement(tag, content);
        }

        private static XElement Element(Func<object, XElement> element, string cssClass, object content)
        {
            return element(new object[]
            {
                Html.Class(cssClass),
                content
            });
        }

        public static XAttribute Attribute(string name, object value)
        {
            return new XAttribute(name, value);
        }
    }
}
