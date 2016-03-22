using Hl7.Fhir.V102;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Specification
{
    public class SDTreeNode
    {
        private List<SDTreeNode> _children = new List<SDTreeNode>();
        private string _lastPathElement;
        private string _name;

        public SDTreeNode Parent { get; set; }
        public string Path { get; private set; }
        public ElementDefinition Element { get; private set; }
        public bool IsSlice { get; set; }

        public bool IsSetupSlice
        {
            get
            {
                return (Element.slicing != null);
            }
        }

        public bool IsSetupSliceForExtension
        {
            get
            {
                return (IsSetupSlice && (Path.EndsWith(".extension") || (Path.EndsWith(".modifierExtension"))));
            }
        }

        public SDTreeNode[] Children
        {
            get
            {
                return _children.ToArray();
            }
        }

        public bool HasChildren
        {
            get
            {
                return Children.Any();
            }
        }

        public string LastPathElement
        {
            get
            {
                return _lastPathElement;
            }
        }

        public SDTreeNode(ElementDefinition element)
        {
            Element = element;
            Path = element.path.WhenNotNull(t => t.value);
            _lastPathElement = element.GetLastPathValue();
            _name = element.name.WhenNotNull(t => t.value);
            IsSlice = false;
        }

        public void AddChild(SDTreeNode child)
        {
            _children.Add(child);
            child.Parent = this;
        }

        public void RemoveChild(SDTreeNode child)
        {
            _children.Remove(child);
            child.Parent = null;
        }

        public void RemoveAllChildren()
        {
            foreach (SDTreeNode child in _children.ToArray())
                RemoveChild(child);
        }

        public void AddChildren(SDTreeNode[] children)
        {
            foreach (SDTreeNode child in children)
                AddChild(child);
        }

        public bool IsLastChild()
        {
            if (Parent == null)
                return true;

            SDTreeNode lastChild = Parent.Children.LastOrDefault();

            if (lastChild == null)
                throw new Exception("Anomolous tree structure detected");

            return (lastChild == this);
        }

        public override string ToString()
        {
            string result = _lastPathElement;

            if (!string.IsNullOrWhiteSpace(_name))
                result += " (" + _name + ")";

            return result;
        }

        public bool HasZeroMaxCardinality()
        {
            return HasZeroMaxCardinality(this);
        }

        private static bool HasZeroMaxCardinality(SDTreeNode treeNode)
        {
            if (treeNode == null)
                return false;

            return (treeNode.Element.IsRemoved() || HasZeroMaxCardinality(treeNode.Parent));
        }

        public string GetDisplayName()
        {
            if (GetNodeType() == SDNodeType.Extension)
            {
                return Element.name.WhenNotNull(t => t.value);
            }
            else
            {
                string result = Element.GetLastPathValue();

                string name = Element.name.WhenNotNull(t => t.value);

                if (!string.IsNullOrWhiteSpace(name))
                    result += " [" + name + "]";

                return result;
            }
        }

        public SDNodeType GetNodeType()
        {
            if (IsSetupSlice)
                return SDNodeType.SetupSlice;

            if (Element.type != null)
            {
                if (Element.type.Length == 0)
                {
                    return SDNodeType.Unknown;
                }
                else if (Element.type.Length == 1)
                {
                    ElementDefinitionType elementType = Element.type.Single();

                    if (elementType.IsBackboneElement())
                        return SDNodeType.Element;
                    else if (elementType.IsPrimitiveType())
                        return SDNodeType.PrimitiveType;
                    else if (elementType.IsReference())
                        return SDNodeType.Reference;
                    else if (elementType.IsComplexType())
                        return SDNodeType.ComplexType;
                    else if (elementType.IsExtension())
                        return SDNodeType.Extension;
                    else if (elementType.IsResource())
                        return SDNodeType.Resource;

                    return SDNodeType.Unknown;
                }
                else
                {
                    if (Element.type.Any(t => t.IsReference()))
                        return SDNodeType.Reference;

                    return SDNodeType.Choice;
                }
            }

            return SDNodeType.Unknown;
        }

        public void DepthFirstTreeWalk(Action<SDTreeNode> function)
        {
            Stack<SDTreeNode> stack = new Stack<SDTreeNode>();
            stack.Push(this);

            while (stack.Any())
            {
                SDTreeNode node = stack.Pop();

                function(node);

                foreach (SDTreeNode childNode in node.Children.Reverse())
                    stack.Push(childNode);
            }
        }
    }
}
