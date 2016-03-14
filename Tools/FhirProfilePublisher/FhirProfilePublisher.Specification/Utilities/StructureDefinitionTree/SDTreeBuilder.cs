using Hl7.Fhir.V102;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FhirProfilePublisher.Specification
{
    public class SDTreeBuilder
    {
        public SDTreeBuilder()
        {
        }

        public SDTreeNode GenerateTree(StructureDefinition structureDefinition, IStructureDefinitionResolver locator, bool includeNodesWithZeroMaxCardinality = true)
        {
            // "index" slices to create unique ElementDefinition.path values
            IndexSlices(structureDefinition.differential.element);

            // Merge differential and the direct base StructureDefinition's differential. -- this needs expanding to include all ancestor base StructureDefinitions
            ElementDefinition[] elements = CreateSnapshot(structureDefinition, locator);

            // Build flat ElementDefinition list into tree
            SDTreeNode rootNode = GenerateTree(elements);

            // Expand out data types
            rootNode.DepthFirstTreeWalk(t => AddMissingComplexDataTypeElements(t));

            // group slices under the slice "setup" node (except extension slices)
            rootNode.DepthFirstTreeWalk(t => GroupSlices(t));

            // remove 0..0 nodes and their children
            if (!includeNodesWithZeroMaxCardinality)
                rootNode.DepthFirstTreeWalk(t => RemoveZeroMaxCardinalityNodes(t));

            // remove setup extension "setup" slice nodes
            rootNode.DepthFirstTreeWalk(t => RemoveExtensionSetupSlices(t));

            return rootNode;
        }

        private static void RemoveExtensionSetupSlices(SDTreeNode node)
        {
            if (node.IsSetupSliceForExtension)
                node.Parent.RemoveChild(node);
        }

        private static void RemoveZeroMaxCardinalityNodes(SDTreeNode node)
        {
            if (node.HasZeroMaxCardinality())
                if (node.Parent != null)
                    node.Parent.RemoveChild(node);
        }

        private static void AddMissingComplexDataTypeElements(SDTreeNode node)
        {
            if (node.Element.type.WhenNotNull(t => t.Count()) == 1)
            {
                ElementDefinitionType elementType = node.Element.type.First();

                if (elementType.IsComplexType())
                {
                    StructureDefinition dataTypeStructureDefinition = FhirData.Instance.FindDataTypeStructureDefinition(elementType.TypeName);

                    ElementDefinition dataTypeRootElement = dataTypeStructureDefinition.differential.element.GetRootElement();
                    ElementDefinition[] dataTypeElements = dataTypeStructureDefinition.differential.element.GetChildren(dataTypeRootElement).ToArray();

                    SDTreeNode[] existingChildren = node.Children;
                    int existingChildNodeIndex = 0;

                    List<SDTreeNode> newChildren = new List<SDTreeNode>();

                    foreach (ElementDefinition dataTypeElement in dataTypeElements)
                    {
                        SDTreeNode existingChild = null;

                        if (existingChildNodeIndex < existingChildren.Length)
                            existingChild = existingChildren[existingChildNodeIndex++];

                        string newPath = dataTypeElement.path.value.Substring(dataTypeRootElement.path.value.Length + 1);

                        if ((existingChild != null) && (existingChild.LastPathElement == newPath))
                        {
                            newChildren.Add(existingChild);
                        }
                        else
                        {
                            newChildren.Add(new SDTreeNode(dataTypeElement));
                        }


                    }

                    node.RemoveAllChildren();
                    node.AddChildren(newChildren.ToArray());

                }
            }
        }

        private static void GroupSlices(SDTreeNode node)
        {
            SDTreeNode[] childSetupSlices = node.Children.Where(t => t.IsSetupSlice && (!t.IsSetupSliceForExtension)).ToArray();

            if (childSetupSlices.Any())
            {
                foreach (SDTreeNode childSetupSlice in childSetupSlices)
                {
                    SDTreeNode[] childSlices = node.Children.Where(t => t.Path.StartsWith(childSetupSlice.Path + "#")).ToArray();

                    foreach (SDTreeNode childSlice in childSlices)
                    {
                        childSlice.IsSlice = true;
                        node.RemoveChild(childSlice);
                        childSetupSlice.AddChild(childSlice);
                    }
                }
            }
        }

        private static void IndexSlices(ElementDefinition[] elements)
        {
            ElementDefinition[] slices = elements.Where(t => t.slicing != null).ToArray();

            foreach (ElementDefinition slice in slices)
            {
                string slicePath = slice.path.value;
                int sliceCount = 0;
                bool isInitialSlice = false;   // need boolean and integer in case root slice is not first in list

                foreach (ElementDefinition element in elements)
                {
                    string path = element.path.value;

                    if (path == slicePath)
                    {
                        if (element == slice)
                        {
                            isInitialSlice = true;
                        }
                        else
                        {
                            isInitialSlice = false;
                            sliceCount++;
                        }
                    }

                    if (path.StartsWith(slicePath))
                    {
                        if (!isInitialSlice)
                        {
                            if (sliceCount == 0)
                                throw new Exception("Found slice child element before slice root element");

                            element.path.value = slicePath + "#" + sliceCount.ToString() + path.Substring(slicePath.Length);
                        }
                    }
                }
            }
        }

        private static ElementDefinition[] CreateSnapshot(StructureDefinition structure, IStructureDefinitionResolver locator)
        {
            ElementDefinition[] elements = structure.differential.WhenNotNull(t => t.element);

            if (elements == null)
                throw new Exception("No differential in definition");

            StructureDefinition baseStructure = locator.GetStructureDefinition(structure.@base.value);
            ElementDefinition[] baseSnapshotElements = baseStructure.differential.WhenNotNull(t => t.element);

            if (baseSnapshotElements == null)
                throw new Exception("No snapshot in base definition");

            List<ElementDefinition> result = new List<ElementDefinition>();

            foreach (ElementDefinition baseElement in baseSnapshotElements)
            {
                ElementDefinition element = structure.differential.element.FirstOrDefault(t => t.path.value == baseElement.path.value);

                if (element != null)
                    result.Add(element);
                else
                    result.Add(baseElement);
            }

            foreach (ElementDefinition element in elements)
                if (!result.Any(t => t == element))
                    result.Add(element);

            return result.ToArray();
        }

        private SDTreeNode GenerateTree(ElementDefinition[] elements)
        {
            ElementDefinition rootElement = elements.GetRootElement();

            if (rootElement == null)
                throw new Exception("Could not find root element");

            SDTreeNode rootNode = new SDTreeNode(rootElement);

            Stack<SDTreeNode> stack = new Stack<SDTreeNode>();
            stack.Push(rootNode);

            while (stack.Any())
            {
                SDTreeNode node = stack.Pop();

                foreach (ElementDefinition element in elements.GetChildren(node.Element))
                {
                    SDTreeNode childNode = new SDTreeNode(element);
                    node.AddChild(childNode);
                    stack.Push(childNode);
                }
            }

            return rootNode;
        }
    }
}
