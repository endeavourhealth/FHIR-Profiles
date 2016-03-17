using Hl7.Fhir.V102;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FhirProfilePublisher.Specification;

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
            SDTreeBuilder builder = new SDTreeBuilder();
            SDTreeNode rootNode = builder.GenerateTree(structureDefinition, _resourceFileSet, false);

            return GenerateHtml(rootNode);
        }

        private XElement GenerateHtml(SDTreeNode rootNode)
        { 
            return Html.Table(new object[]
            {
                Html.Class(Styles.ResourceTreeClassName),
                GenerateTableHeader(),
                GenerateTableBody(rootNode)
            });
        }

        private static string GetTreeViewHelpersJavaScript()
        {
            return ResourceHelper.LoadStringResource(Scripts.TreeViewHelpersScriptFileName);
        }

        private XElement GenerateTableHeader()
        {
            return Html.THead(Html.Tr(new object[]
            {
                Html.Th(Styles.ResourceTreeNameColumnClassName, Html.A(FhirConstants.ResourceStructureTableHeaderUrl, "Name")),
                Html.Th(Styles.ResourceTreeCardinalityColumnClassName, Html.A(FhirConstants.ResourceStructureTableHeaderUrl, "Card.")),
                Html.Th(Styles.ResourceTreeTypeColumnClassName, Html.A(FhirConstants.ResourceStructureTableHeaderUrl, "Type")),
                Html.Th(Styles.ResourceTreeDescriptionColumnClassName, Html.A(FhirConstants.ResourceStructureTableHeaderUrl, "Description & Constraints"))
            }));
        }

        private XElement GenerateTableBody(SDTreeNode rootNode)
        {
            List<XElement> tableRows = new List<XElement>();

            SDTreeNodeNavigator nodeNavigator = new SDTreeNodeNavigator(rootNode);

            while (nodeNavigator.MoveNext())
                tableRows.Add(GetTableRow(nodeNavigator.CurrentNode));

            return Html.TBody(tableRows);
        }

        private XElement GetTableRow(SDTreeNode treeNode)
        {
            ElementDefinition currentElement = treeNode.Element;

            List<object> result = new List<object>()
            {
                GetNameAndImagesTableCell(treeNode),
                Html.Td(currentElement.GetCardinalityText()),
                GetTypeTableCell(treeNode.Element.WhenNotNull(t => t.type)),
                GetDescriptionTableCell(currentElement)
            };

            if (treeNode.HasZeroMaxCardinality())
                result.Add(Html.Class(Styles.RemovedTableRowClassName));

            return Html.Tr(result.ToArray());
        }

        private XElement GetNameAndImagesTableCell(SDTreeNode treeNode)
        {
            bool[] indents = GetHierarchyImageDefinition(treeNode);

            XElement td = Html.Td(new object[]
            {
                Html.Class(Styles.HierarchyClassName),
                Html.Style(Styles.GetBackgroundImageCss(GetBackgroundHierarchyImage(indents, treeNode.HasChildren))),
                GetHierarchyImageElement(Images.IconTreeSpacer),
                GetHierarchyImages(treeNode, indents),
                GetHierarchyImageElement(Images.IconTreeSpacerWide),
                treeNode.GetDisplayName()
            });

            return td;
        }

        private XElement GetTypeTableCell(ElementDefinitionType[] types)
        {
            string result = string.Empty;

            if (types == null)
                return Html.Td(string.Empty);

            if (types.Length == 0)
            {
                return Html.Td(string.Empty);
            }
            else if (types.Length == 1)
            {
                ElementDefinitionType type = types.Single();

                if (type.IsReference())
                {
                    return Html.Td(GetReferenceTypeName(new ElementDefinitionType[] { type }));
                }
                else if (type.IsExtension())
                {
                    uri profileUri = type.profile.WhenNotNull(t => t.FirstOrDefault());
                    
                    if (profileUri != null)
                    {
                        StructureDefinition structureDefinition = _resourceFileSet.GetStructureDefinition(profileUri.value);
                        ElementDefinitionType[] elementDefinitionTypes = structureDefinition.GetSimpleExtensionType();

                        return GetTypeTableCell(elementDefinitionTypes);
                    }
                    else
                    {
                        return Html.Td(string.Empty);
                    }                 
                }
                else
                {
                    return Html.Td(GetNonReferenceTypeName(type));
                }
            }
            else
            {
                if (types.All(t => t.IsReference()))
                    return Html.Td(GetReferenceTypeName(types));

                return Html.Td(types.Select(t => GetNonReferenceTypeName(t)).Intersperse(" | "));
            }
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
                        Html.A((definition.GetValueSetBindingStrength().GetUrl()), definition.GetValueSetBindingStrength().GetDescription()),
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
            string typeUrl = FhirData.Instance.GetDataTypeUrl(typeName);

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
                GetDataTypeLink(FhirConstants.ReferenceTypeName),
                new XText("("),
                elements.Intersperse(new XText(" | ")).ToArray(),
                new XText(")")
            };
        }

        public bool[] GetHierarchyImageDefinition(SDTreeNode treeNode)
        {
            Stack<SDTreeNode> stack = new Stack<SDTreeNode>();

            SDTreeNode current = treeNode;

            while (current.Parent != null)
            {
                stack.Push(current);
                current = current.Parent;
            }

            List<bool> result = new List<bool>();

            while (stack.Any())
            {
                SDTreeNode node = stack.Pop();
                result.Add(node.IsLastChild());
            }

            return result.ToArray();
        }

        private string GetBackgroundHierarchyImage(bool[] indents, bool hasChildren)
        {
            string imageName = Images.GenerateBackgroundHierarchyImage(indents, hasChildren, _outputPaths);
            return _outputPaths.GetRelativePath(OutputFileType.Image, imageName);
        }

        private XElement[] GetHierarchyImages(SDTreeNode treeNode, bool[] indents)
        {
            List<XElement> images = new List<XElement>();

            for (int i = 0; i < indents.Length - 1; i++)
                images.Add(GetHierarchyImageElement(indents[i] ? Images.IconTreeBlank : Images.IconTreeVLine));

            if (indents.Any())
                images.Add(GetHierarchyImageElement(indents[indents.Length - 1] ? Images.IconTreeVJoinEnd : Images.IconTreeVJoin));

            images.Add(GetHierarchyImageElement(Images.GetImageName(treeNode)));

            return images.ToArray();
        }

        private XElement GetHierarchyImageElement(string imageName)
        {
            return Html.Img(_outputPaths.GetRelativePath(OutputFileType.Image, imageName), Styles.HierarchyClassName, ".", Images.GetImageTitle(imageName));
        }
    }
}
