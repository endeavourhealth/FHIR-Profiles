using Hl7.Fhir.V102;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FhirProfilePublisher.Engine
{
    internal class ResourceListingHtmlGenerator
    {
        private OutputPaths _outputPaths;

        public ResourceListingHtmlGenerator(OutputPaths outputPaths)
        {
            _outputPaths = outputPaths;
        }

        internal void GenerateStructureDefinitionListing(string fileName, ResourceFileSet resourceFileSet)
        {
            List<object> result = new List<object>();

            foreach (string w5Group in resourceFileSet.StructureDefinitionsByW5Group.Keys.OrderBy(t => t))
            {
                StructureDefinitionFile[] structureDefinitionFiles = resourceFileSet
                    .StructureDefinitionsByW5Group[w5Group]
                    .OrderBy(t => t.Name)
                    .ToArray();

                XElement groupItemList = GenerateItemList(structureDefinitionFiles);

                result.Add(Html.H4(w5Group));
                result.Add(groupItemList);
            }
            
            XElement extensionContent = GenerateItemList(resourceFileSet
                .StructureDefinitionExtensionFiles
                .OrderBy(t => t.Name)
                .ToArray());

            result.AddRange(new object[]
            {
                Html.H3("Extensions"),
                Html.P("The structures above refer to the following extensions:"),
                extensionContent
            });

            WritePage(fileName, "Resources", Html.Div(result.ToArray()));
        }

        internal void GenerateValueSetListing(string fileName, ResourceFileSet resourceFileSet)
        {
            ValueSetFile[] items = resourceFileSet.ValueSetFiles
                .OrderBy(t => t.Name)
                .ToArray();

            XElement content = GenerateItemList(items);

            WritePage(fileName, "Value sets", content);
        }

        private void WritePage(string fileName, string itemTypeName, XElement content)
        {
            string contentHtml = Html.Div(new object[]
            {
                Html.H3(itemTypeName),
                content
            })
            .ToString(SaveOptions.DisableFormatting);

            string html = Templates.Instance.GetPage(itemTypeName, contentHtml, "0.1", DateTime.Now);

            _outputPaths.WriteUtf8File(OutputFileType.Html, fileName, html);
        }

        private XElement GenerateItemList(ResourceFile[] items)
        {
            return Html.Table(new object[]
            {
                Html.Id(Styles.ResourcesListingTableIdName),
                Html.Class("table table-hover table-condensed"),
                Html.THead(new object[]
                {
                    Html.Th(Styles.ResourceListingTableNameColumnClassName, "Name"),
                    Html.Th(Styles.ResourceListingTableMaturityColumnClassName, GetMaturityColumnWithImage("Maturity", Images.IconBlank)),
                    Html.Th(Styles.ResourceListingTableIdentifierColumnClassName, "Identifier")
                }),
                Html.TBody
                (
                    items.Select(t => Html.Tr(new object[] 
                    {
                        Html.Td(Styles.ResourceListingTableNameColumnClassName, Html.A(t.OutputHtmlFilename, t.Name)), 
                        Html.Td(Styles.ResourceListingTableMaturityColumnClassName, GetMaturityColumnWithImage(t.Maturity.GetDescription(), t.Maturity.GetAssociatedIcon())),
                        Html.Td(Styles.ResourceListingTableIdentifierColumnClassName, t.CanonicalUrl)
                    })).ToArray()
                )
            });
        }

        private object[] GetMaturityColumnWithImage(string description, string iconName)
        {
            return new object[]
            {
                Html.Img(_outputPaths.GetRelativePath(OutputFileType.Image, iconName)),
                " ",
                description
            };
        }
    }
}
