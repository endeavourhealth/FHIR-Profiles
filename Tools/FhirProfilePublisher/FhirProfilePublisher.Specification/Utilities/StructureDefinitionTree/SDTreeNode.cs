﻿using Hl7.Fhir.V101;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FhirProfilePublisher.Specification;

namespace FhirProfilePublisher.Engine
{
    public class SDTreeNode
    {
        private List<SDTreeNode> _children = new List<SDTreeNode>();
        private string _lastPathElement;
        private string _name;

        public SDTreeNode Parent { get; set; }
        public string Path { get; private set; }
        public ElementDefinition Element { get; private set; }

        public bool IsSlicingEntry
        {
            get
            {
                return (Element.slicing != null);
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

        public SDTreeNode(ElementDefinition element)
        {
            Element = element;
            Path = element.path.WhenNotNull(t => t.value);
            _lastPathElement = element.GetNameFromPath();
            _name = element.name.WhenNotNull(t => t.value);
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
    }
}