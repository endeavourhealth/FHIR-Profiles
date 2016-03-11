using Hl7.Fhir.V101;
using System;
using System.Collections.Generic;
using System.Linq;
using FhirProfilePublisher.Specification;

namespace FhirProfilePublisher.Engine
{
    public class StructureDefinitionTreeBuilder
    {
        public StructureDefinitionTreeBuilder()
        {
        }

        public SDTreeNode GenerateTree(StructureDefinition structureDefinition, IStructureDefinitionResolver locator)
        {
            IndexSlices(structureDefinition.differential.element);

            ElementDefinition[] elements = CreateSnapshot(structureDefinition, locator);

            SDTreeNode rootNode = GenerateTree(elements);

            GroupSlices(rootNode);

            return rootNode;
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
            ElementDefinition rootElement = GetRootElement(elements);

            if (rootElement == null)
                throw new Exception("Could not find root element");

            SDTreeNode rootNode = new SDTreeNode(rootElement);

            Stack<SDTreeNode> stack = new Stack<SDTreeNode>();
            stack.Push(rootNode);

            while (stack.Any())
            {
                SDTreeNode node = stack.Pop();

                foreach (ElementDefinition element in GetChildren(node.Element, elements))
                {
                    SDTreeNode childNode = new SDTreeNode(element);
                    node.AddChild(childNode);
                    stack.Push(childNode);
                }
            }

            return rootNode;
        }

        private ElementDefinition GetRootElement(ElementDefinition[] elements)
        {
            return elements.Single(t => t.path.value.Split('.').Count() == 1);
        }

        private ElementDefinition[] GetChildren(ElementDefinition element, ElementDefinition[] elements)
        {
            List<ElementDefinition> result = new List<ElementDefinition>();

            result.AddRange(elements.Where(t => IsDirectChildpath(element.path.value, t.path.value)).ToArray());

            //if (parent.HasDataTypeChoice())
                //result.AddRange(parent.type.Select(t => GenerateFakeDataTypeChoiceElement(_currentElement, t)));

            return result.ToArray();
        }

        private static bool IsDirectChildpath(string parentpath, string childpath)
        {
            if (!childpath.StartsWith(parentpath + "."))
                return false;

            string childpathWithoutParent = childpath.Remove(0, Math.Min(parentpath.Length + 1, childpath.Length));

            return (!childpathWithoutParent.Contains("."));
        }
    }
}
