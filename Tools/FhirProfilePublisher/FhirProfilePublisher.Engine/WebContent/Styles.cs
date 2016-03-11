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
    internal static class Styles
    {
        public static readonly string StylesheetResourceLocation = typeof(Styles).Assembly.GetName().Name + ".WebContent.Styles.";
        public const string StylesheetFileName = "style.css";
        public const string BootstrapStylesheetFileName = "bootstrap.css";
        public const string JQueryUIStylesheetFileName = "jquery-ui.css";

        public const string BackgroundImageCss = "background-image: url({0})";
        public const string TableLayoutFixedClassName = "table-fixed";
        public const string TableColumn20pcClassName = "table-column-20pc";
        public const string StarListClassName = "star-list";
        public const string RemovedTableRowClassName = "removed-row";
        public const string DisplayNoneStyle = "display: none; ";
        public const string StrikethroughStyle = "text-decoration: line-through; ";
        public const string ResourcesListingTableIdName = "resources-table";
        public const string ResourceListingTableNameColumnClassName = "resources-table-name-column";
        public const string ResourceListingTableMaturityColumnClassName = "resources-table-maturity-column";
        public const string ResourceListingTableIdentifierColumnClassName = "resources-table-identifier-column";
        public const string HierarchyClassName = "hierarchy";
        public const string ResourceTreeClassName = "resource-tree";
        public const string ResourceTreeNameColumnClassName = "resource-tree-name-column";
        public const string ResourceTreeFlagsColumnClassName = "resource-tree-flags-column";
        public const string ResourceTreeCardinalityColumnClassName = "resource-tree-cardinality-column";
        public const string ResourceTreeTypeColumnClassName = "resource-tree-type-column";
        public const string ResourceTreeDescriptionColumnClassName = "resource-tree-description-column";

        public static void WriteStylesToDisk(OutputPaths outputPaths)
        {
            string[] styleNames = GetStyleNames();

            foreach (string styleName in styleNames)
                ResourceHelper.WriteResourceToDisk(StylesheetResourceLocation + styleName, outputPaths.GetOutputPath(OutputFileType.Style, styleName));
        }

        private static string[] GetStyleNames()
        {
            return new string[]
            {
                BootstrapStylesheetFileName,
                JQueryUIStylesheetFileName,
                StylesheetFileName
            };
        }

        public static XElement[] GetStylesheetTags(OutputPaths outputPaths)
        {
            return GetStyleNames().Select(t => Html.LinkStylesheet(outputPaths.GetRelativePath(OutputFileType.Style, t))).ToArray();
        }

        public static string GetBackgroundImageCss(string url)
        {
            return string.Format(BackgroundImageCss, url);
        }
    }
}
