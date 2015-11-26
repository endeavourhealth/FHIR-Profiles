using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FhirProfilePublisher.Engine
{
    internal static class BootstrapHtml
    {
        public static XElement Panel(object content)
        {
            return Html.Div("panel panel-default", Html.Div("panel-body", content));
        }

        public static XElement Checkbox(object content, bool isChecked)
        {
            return Html.Div("checkbox", Html.Label(Html.Input("checkbox", new object[]
            {
                isChecked ? Html.Attribute("checked", "checked") : null,
                content                
            })));
        }

        public static XElement Label(object content)
        {
            return Html.Span("label label-default", content);
        }

        public static XElement Abbreviation(string abbreviation, object content)
        {
            return Html.Abbr(new object[]
            {
                Html.Attribute("title", abbreviation),
                content
            });
        }

        public static XElement ListGroup(object[] items)
        {
            return Html.Ul(new object[]
            {
                Html.Class("list-group"),
                items.Select(t => Html.Li(new object[] { Html.Class("list-group-item"), t })).ToArray()
            });
        }

        public static XElement Table(string additionalCssClass, object content)
        {
            return Html.Table(new object[]
            {
                Html.Class("table table-condensed " + additionalCssClass),
                content
            });
        }

        public static XElement GetTabs(Dictionary<string, object> tabNamesAndContent)
        {
            return Html.Div(new object[]
            {
                Html.Ul(new object[]
                {
                    Html.Class("nav nav-tabs"),
                    Html.Attribute("role", "tablist"),
                    GetTabHeaders(tabNamesAndContent.Keys.Select(t => t).ToArray())
                }),
                GetTabContents(tabNamesAndContent)
            });
        }

        private static XElement[] GetTabHeaders(string[] tabNames)
        {
            return tabNames.Select((t, index) => GetTabHeader(t, (index == 0))).ToArray();
        }
        
        private static XElement GetTabHeader(string tabName, bool active)
        {
            return Html.Li(new object[]
            {
                Html.Attribute("role", "presentation"),
                active ? Html.Class("active") : null,
                Html.A(new object[]
                {
                    Html.Attribute("href", "#" + tabName),
                    Html.Attribute("aria-controls", tabName),
                    Html.Role("tab"),
                    Html.Attribute("data-toggle", "tab"),
                    Html.B(tabName)
                })
            });
        }

        private static XElement GetTabContents(Dictionary<string, object> tabNamesAndContent)
        {
            return Html.Div("tab-content", 
                tabNamesAndContent
                    .Keys
                    .Select((t, index) => GetTabContent(t, tabNamesAndContent[t], (index == 0)))
                    .ToArray());
        }
        
        private static XElement GetTabContent(string tabName, object tabContent, bool active)
        {
            return Html.Div(new object[]
            {
                Html.Role("tabpanel"),
                active ? Html.Class("tab-pane active") : Html.Class("tab-pane"),
                Html.Id(tabName),
                //Html.P(Html.B(tabName)),
                tabContent
            });
        }
    }
}
