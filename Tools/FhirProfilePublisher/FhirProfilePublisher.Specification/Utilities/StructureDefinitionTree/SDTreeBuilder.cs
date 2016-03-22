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

        public SDTreeNode GenerateTree(StructureDefinition structureDefinition, IStructureDefinitionResolver resolver, bool includeNodesWithZeroMaxCardinality = true)
        {
            // process ElementDefinition list
            //
            // to create list where path values are unique (by indexing slices)
            // and where there are no orphan children by creating fake parents
            //

            ElementDefinition[] elements = structureDefinition.differential.WhenNotNull(t => t.element);

            // sanity checks
            PerformDifferentialElementsSanityCheck(elements, false);

            // "index" slices to create unique ElementDefinition.path values
            IndexSlices(elements);

            // Merge differential and the direct base StructureDefinition's differential. -- this needs expanding to include all ancestor base StructureDefinitions
            elements = CreateSnapshot(structureDefinition, resolver);

            // Add fake missing parents
            elements = AddFakeMissingParents(elements);


            // build tree
            //
            //

            // Build flat ElementDefinition list into tree
            SDTreeNode rootNode = GenerateTree(elements);


            // process tree
            //
            //

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

        private void PerformDifferentialElementsSanityCheck(ElementDefinition[] elements, bool checkPathForUniqueness)
        {
            if ((elements == null))
                throw new ArgumentException("StructureDefinition does not have differential element list populated");

            // sanity check #2 - all have path values
            if (!(elements.All(t => (t.path != null) && (!string.IsNullOrWhiteSpace(t.path.value)))))
                throw new ArgumentException("StructureDefinition has element with null or empty path value");

            // sanity check #3 - elements don't contain # character
            if (elements.Any(t => (t.path.value.Contains("#"))))
                throw new ArgumentException("StructureDefinition has element with path value containing a # character");

            // sanity check #4 - elements have unique path values
            if (checkPathForUniqueness)
                VerifyPathIsUnique(elements);
                    
        }

        private void VerifyPathIsUnique(ElementDefinition[] elements)
        {
            if ((elements.DistinctBy(t => t.path.value).Count() != elements.Count()))
                throw new ArgumentException("StructureDefinition has elements with non-unique path values");
        }


        private ElementDefinition[] AddFakeMissingParents(ElementDefinition[] elementDefinitions)
        {
            List<ElementDefinition> result = new List<ElementDefinition>();

            foreach (ElementDefinition elementDefinition in elementDefinitions)
            {
                string path = elementDefinition.path.WhenNotNull(t => t.value) ?? string.Empty;

                // walk up each path item testing for existence of parent element

                string pathTemporary = string.Empty;

                foreach (string pathItem in path.Split('.'))
                {
                    pathTemporary += pathItem;

                    // if the parent doesn't exist
                    // or we haven't already created a fake parent

                    if (!((elementDefinitions.Any(t => t.path.value == pathTemporary))
                        || (result.Any(t => t.path.value == pathTemporary))))
                    {
                        // create a fake parent

                        ElementDefinition fakeElement = new ElementDefinition();
                        fakeElement.IsFake = true;
                        fakeElement.path = new @string();
                        fakeElement.path.value = pathTemporary;
                        result.Add(fakeElement);
                    }

                    pathTemporary += ".";
                }

                result.Add(elementDefinition);
            }

            return result.ToArray();
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
                    StructureDefinition dataTypeDefinition = FhirData.Instance.FindDataTypeStructureDefinition(elementType.TypeName);

                    ElementDefinition dataTypeRootElement = dataTypeDefinition.differential.element.GetRootElement();
                    ElementDefinition[] dataTypeElements = dataTypeDefinition.differential.element.GetChildren(dataTypeRootElement).ToArray();

                    List<SDTreeNode> newChildren = new List<SDTreeNode>();

                    foreach (ElementDefinition dataTypeElement in dataTypeElements)
                    {
                        string lastPathElement = dataTypeElement.path.value.Substring(dataTypeRootElement.path.value.Length + 1);

                        SDTreeNode existingChild = node.Children.FirstOrDefault(t => t.LastPathElement == lastPathElement);

                        if (existingChild != null)
                        {
                            if (!existingChild.Element.IsFake)
                            {
                                newChildren.Add(existingChild);
                            }
                            else
                            {
                                SDTreeNode fakeReplacement = new SDTreeNode(dataTypeElement);
                                SDTreeNode[] childsChildren = existingChild.Children;
                                existingChild.RemoveAllChildren();
                                fakeReplacement.AddChildren(childsChildren);
                                newChildren.Add(fakeReplacement);
                            }
                        }
                        else
                        {
                            newChildren.Add(new SDTreeNode(dataTypeElement));
                        }


                    }

                    foreach (SDTreeNode childNode in node.Children)
                        if (newChildren.All(t => t.LastPathElement != childNode.LastPathElement))
                            newChildren.Add(childNode);

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

            List<ElementDefinition> elementsToAddToBeginning = new List<ElementDefinition>();

            string[] baseBaseElementNames = new string[]
            {
                "id",
                "meta",
                "implicitRules",
                "language",
                "text",
                "contained"
            };

            foreach (ElementDefinition element in elements)
            {
                if (!result.Any(t => t == element))
                {
                    if (baseBaseElementNames.Contains(element.GetLastPathValue()))
                        elementsToAddToBeginning.Add(element);
                    else
                        result.Add(element);
                }
            }

            result.InsertRange(0, elementsToAddToBeginning);

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
