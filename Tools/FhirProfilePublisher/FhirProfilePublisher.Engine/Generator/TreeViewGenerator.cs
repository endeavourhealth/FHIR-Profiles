using Hl7.Fhir.V101;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FhirProfilePublisher.Engine
{
    internal class TreeViewGenerator
    {
        private ResourceFileSet _resourceFileSet;
        private OutputPaths _outputPaths;
        
        public TreeViewGenerator(ResourceFileSet resourceFileSet, OutputPaths outputPaths)
        {
            _resourceFileSet = resourceFileSet;
            _outputPaths = outputPaths;
        }

        public XElement Generate(StructureDefinition structureDefinition)
        {
            StructureDefinition baseDefinition = _resourceFileSet.GetStructureDefinition(structureDefinition.@base.value);
            ElementDefinition[] elementDefinitions = Fhir.MergeElementDefinitions(baseDefinition.differential.element, structureDefinition.differential.element);

            return GenerateHtml(elementDefinitions);
        }

        private XElement GenerateHtml(ElementDefinition[] elementDefinitions)
        { 
            return Html.Table(new object[]
            {
                Html.Class(Styles.ResourceTreeClassName),
                GenerateTableHeader(),
                GenerateTableBody(elementDefinitions)
            });
        }

        private static string GetTreeViewHelpersJavaScript()
        {
            return Utilities.LoadStringResource(Scripts.TreeViewHelpersScriptFileName);
        }

        private XElement GenerateTableHeader()
        {
            return Html.THead(Html.Tr(new object[]
            {
                Html.Th(Styles.ResourceTreeNameColumnClassName, Html.A(Fhir.ResourceStructureTableHeaderUrl, "Name")),
                Html.Th(Styles.ResourceTreeFlagsColumnClassName, Html.A(Fhir.ResourceStructureTableHeaderUrl, "Flags")),
                Html.Th(Styles.ResourceTreeCardinalityColumnClassName, Html.A(Fhir.ResourceStructureTableHeaderUrl, "Card.")),
                Html.Th(Styles.ResourceTreeTypeColumnClassName, Html.A(Fhir.ResourceStructureTableHeaderUrl, "Type")),
                Html.Th(Styles.ResourceTreeDescriptionColumnClassName, Html.A(Fhir.ResourceStructureTableHeaderUrl, "Description & Constraints"))
            }));
        }

        private XElement GenerateTableBody(IEnumerable<ElementDefinition> elements)
        {
            ElementNavigator elementNavigator = new ElementNavigator(elements);
            elementNavigator.SkipExtensionSlice = true;

            List<XElement> tableRows = new List<XElement>();

            while (elementNavigator.MoveNext())
                tableRows.Add(GetTableRow(elementNavigator));

            return Html.TBody(tableRows);
        }

        private XElement GetTableRow(ElementNavigator elementNavigator)
        {
            ElementDefinition currentElement = elementNavigator.Current;
            bool hasChildren = elementNavigator.CurrentHasChildren;

            List<object> result = new List<object>()
            {
                GetNameAndImagesTableCell(elementNavigator),
                GetFlagsTableCell(currentElement),
                GetCardinalityTableCell(currentElement, null),
                GetTypeTableCell(currentElement, hasChildren),
                GetDescriptionTableCell(currentElement)
            };

            if (currentElement.IsRemoved())
            {
                //result.Add(Html.Class(Styles.StrikethroughStyle));
                result.Add(Html.Class(Styles.RemovedTableRowClassName));
                //result.Add(Html.Style(Styles.DisplayNoneStyle + Styles.StrikethroughStyle));
            }

            return Html.Tr(result.ToArray());
        }

        private XElement GetNameAndImagesTableCell(ElementNavigator elementNavigator)
        {
            ElementDefinition element = elementNavigator.Current;
            bool hasChildren = elementNavigator.CurrentHasChildren && (!elementNavigator.Current.IsRemoved());
            bool[] indents = elementNavigator.GetHierarchyImageDefinition();

            XElement td = Html.Td(new object[]
            {
                Html.Class(Styles.HierarchyClassName),
                Html.Style(Styles.GetBackgroundImageCss(GetBackgroundHierarchyImage(indents, hasChildren))),
                GetHierarchyImageElement(Images.IconTreeSpacer),
                GetHierarchyImages(element, indents),
                GetHierarchyImageElement(Images.IconTreeSpacerWide),
                GetDisplayName(elementNavigator.Current)
            });

            return td;
        }


        private XElement GetFlagsTableCell(ElementDefinition definition)
        {
            string flags = string.Empty;

            if (!definition.IsRemoved())
                flags = string.Join(" ", definition.GetFlagsSymbols());

            return Html.Td(flags);
        }

        private XElement GetCardinalityTableCell(ElementDefinition definition, ElementDefinition fallback)
        {
            return Html.Td(definition.GetCardinalityText());
        }

        private XElement GetTypeTableCell(ElementDefinition element, bool hasChildren)
        {
            ElementDefinitionType[] types = element.type;

            if (types == null)
            {
                if (hasChildren)
                    return Html.Td(GetDataTypeLink(Fhir.BackboneElement));
                else
                    return Html.Td(string.Empty);
            }

            if (types.Length == 0)
                return Html.Td(string.Empty);

            if (element.AllTypesAreReference())
                return Html.Td(GetReferenceTypeName(types));

            if (types.Length == 1)
            {
                ElementDefinitionType type = types.Single();

                string extensionUri = element.GetExtensionCanonicalUrl();

                if (!string.IsNullOrEmpty(extensionUri))
                {
                    StructureDefinition structureDefinition = _resourceFileSet.GetStructureDefinition(type.profile.First().value);
                    return Html.Td(GetDataTypeLink(structureDefinition.GetExtensionType()));
                }

                return Html.Td(GetNonReferenceTypeName(type));
            }

            if (types.Length > 1)
                return Html.Td(string.Empty);

            throw new NotSupportedException("Element type not supported.");

        }

        private XElement GetDescriptionTableCell(ElementDefinition definition)
        {
            List<object> lines = new List<object>();

            if (!definition.IsRemoved())
            {
                // short definition
                string shortDefinition = definition.@short.WhenNotNull(t => t.value);

                if (shortDefinition != null)
                    lines.Add(Html.P(shortDefinition));

                // extension details
                string extensionCanonicalUrl = definition.GetExtensionCanonicalUrl();

                if (!string.IsNullOrEmpty(extensionCanonicalUrl))
                {
                    Link link = _resourceFileSet.GetStructureDefinitionLink(extensionCanonicalUrl);
                    lines.Add(Html.P(GetLabelAndValue("Extension", Html.A(link))));
                }

                // fixed value
                if (definition.HasFixedValue())
                    lines.Add(Html.P(GetLabelAndValue("Fixed value", definition.GetFixedValue())));

                // valueset/bindings
                string valueSetUri = definition.GetValueSetUri();

                if (!string.IsNullOrWhiteSpace(valueSetUri))
                {
                    Link valuesetLink = _resourceFileSet.GetValueSetLink(valueSetUri);

                    lines.Add(Html.P(GetLabelAndValue("Valueset", new object[]
                    {
                        Html.A(valuesetLink.Url, valuesetLink.Display),
                        " (",
                        Html.A(Fhir.GetBindingStrengthUrl(definition.GetValueSetBindingStrength().Value), definition.GetValueSetBindingStrength().GetDescription()),
                        ")"
                    })));
                }

                // invariants
                string[] invariants = definition.GetInvariantText();

                if (invariants.Length > 0)
                    lines.Add(Html.P(invariants.Select(t => Html.I(t)).Intersperse(Html.Br()).ToArray()));
            }

            return Html.Td(lines.ToArray());
        }

        private string GetDisplayName(ElementDefinition element)
        {
            if (element.IsExtension())
                return element.name.WhenNotNull(t => t.value);

            return element.GetNameFromPath();
        }


        private object[] GetLabelAndValue(string labelText, object value)
        {
            return new object[]
            {
                BootstrapHtml.Label(labelText),
                " ",
                value
            };
        }


        private object GetNonReferenceTypeName(ElementDefinitionType type)
        {
            return GetDataTypeLink(type.WhenNotNull(t => t.code.value));
        }

        private XNode GetDataTypeLink(string typeName)
        {
            string typeUrl = Fhir.GetDataTypeUrl(typeName);

            if (string.IsNullOrWhiteSpace(typeUrl))
                return new XText(typeName);

            return Html.A(typeUrl, typeName);
        }


        private object[] GetReferenceTypeName(ElementDefinitionType[] types)
        {
            List<XNode> elements = new List<XNode>();

            foreach (ElementDefinitionType type in types)
                elements.Add(Html.A(_resourceFileSet.GetStructureDefinitionLink(type.profile.First().value)));

            return new object[]
            {
                GetDataTypeLink(Fhir.ReferenceTypeName),
                new XText("("),
                elements.Intersperse(new XText(" | ")).ToArray(),
                new XText(")")
            };
        }

        private string GetBackgroundHierarchyImage(bool[] indents, bool hasChildren)
        {
            string imageName = Images.GenerateBackgroundHierarchyImage(indents, hasChildren, _outputPaths);
            return _outputPaths.GetRelativePath(OutputFileType.Image, imageName);
        }

        private XElement[] GetHierarchyImages(ElementDefinition element, bool[] indents)
        {
            List<XElement> images = new List<XElement>();

            for (int i = 0; i < indents.Length - 1; i++)
                images.Add(GetHierarchyImageElement(indents[i] ? Images.IconTreeBlank : Images.IconTreeVLine));

            if (indents.Any())
                images.Add(GetHierarchyImageElement(indents[indents.Length - 1] ? Images.IconTreeVJoinEnd : Images.IconTreeVJoin));

            images.Add(GetHierarchyImageElement(Images.GetImageName(element)));

            return images.ToArray();
        }

        private XElement GetHierarchyImageElement(string imageName)
        {
            return Html.Img(_outputPaths.GetRelativePath(OutputFileType.Image, imageName), Styles.HierarchyClassName, ".", Images.GetImageTitle(imageName));
        }
    }
}
