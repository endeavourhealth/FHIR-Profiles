using Hl7.Fhir.V101;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FhirProfilePublisher.Specification;

namespace FhirProfilePublisher.Engine
{
    internal class TreeNode
    {
        private List<TreeNode> _children = new List<TreeNode>();
        private string _lastPathElement;
        private string _name;

        public TreeNode Parent { get; set; }
        public string Path { get; private set; }
        public ElementDefinition Element { get; private set; }

        public bool IsSlicingEntry
        {
            get
            {
                return (Element.slicing != null);
            }
        }

        public TreeNode[] Children
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

        public TreeNode(ElementDefinition element)
        {
            Element = element;
            Path = element.path.WhenNotNull(t => t.value);
            _lastPathElement = element.GetNameFromPath();
            _name = element.name.WhenNotNull(t => t.value);
        }

        public void AddChild(TreeNode child)
        {
            _children.Add(child);
            child.Parent = this;
        }

        public void RemoveChild(TreeNode child)
        {
            _children.Remove(child);
            child.Parent = null;
        }

        public bool IsLastChild()
        {
            if (Parent == null)
                return true;

            TreeNode lastChild = Parent.Children.LastOrDefault();

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
    }
}
