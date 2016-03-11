using Hl7.Fhir.V101;
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

        ////////////////////////////////////////////////////////////////////////
        //
        // issues
        // ------
        //
        // 1. currently don't see the elements in ancestor SDs - only the direct parent.
        // 2. don't see elements from data types unless they have been overriden.
        //
        //
        ////////////////////////////////////////////////////////////////////////


        public SDTreeNode GenerateTree(StructureDefinition structureDefinition, IStructureDefinitionResolver locator)
        {
            // "index" slices to create unique ElementDefinition.path values
            IndexSlices(structureDefinition.differential.element);

            // Merge differential and the direct base StructureDefinition's differential. -- this needs expanding to include all ancestor base StructureDefinitions
            ElementDefinition[] elements = CreateSnapshot(structureDefinition, locator);

            // Build flat ElementDefinition list into tree
            SDTreeNode rootNode = GenerateTree(elements);

            // Expand out data types
            AddMissingComplexDataTypeElements(rootNode);

            // group slices under the slice "setup" node
            GroupSlices(rootNode);

            return rootNode;
        }

        private static void AddMissingComplexDataTypeElements(SDTreeNode rootNode)
        {
            Stack<SDTreeNode> stack = new Stack<SDTreeNode>();
            stack.Push(rootNode);

            while (stack.Any())
            {
                SDTreeNode node = stack.Pop();

                if (node.Element.type.WhenNotNull(t => t.Count()) == 1)
                {
                    ElementDefinitionType elementType = node.Element.type.First();

                    if (elementType.IsPrimitiveType())
                    {
                        // do nothing
                    }
                    else if (elementType.IsReference())
                    {
                        // do nothing
                    }
                    else if (elementType.IsComplexType())
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
                    else if (elementType.IsExtension())
                    {
                        // do nothing
                    }
                    else
                    {
                        // do nothing
                    }
                }

                foreach (SDTreeNode childNode in node.Children.Reverse())
                    stack.Push(childNode);
            }
        }

        private static void GroupSlices(SDTreeNode rootNode)
        {
            Stack<SDTreeNode> stack = new Stack<SDTreeNode>();
            stack.Push(rootNode);

            while (stack.Any())
            {
                SDTreeNode node = stack.Pop();

                SDTreeNode[] childSlicingEntries = node.Children.Where(t => t.IsSlicingEntry).ToArray();

                if (childSlicingEntries.Any())
                {
                    foreach (SDTreeNode childSlicingEntry in childSlicingEntries)
                    {
                        SDTreeNode[] childSlices = node.Children.Where(t => t.Path.StartsWith(childSlicingEntry.Path + "#")).ToArray();

                        foreach (SDTreeNode childSlice in childSlices)
                        {
                            node.RemoveChild(childSlice);
                            childSlicingEntry.AddChild(childSlice);
                        }
                    }
                }

                foreach (SDTreeNode child in node.Children.Reverse())
                    stack.Push(child);
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
