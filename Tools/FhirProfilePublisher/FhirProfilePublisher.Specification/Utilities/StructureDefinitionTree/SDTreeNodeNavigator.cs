using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.V102;

namespace FhirProfilePublisher.Specification
{
    public class SDTreeNodeNavigator
    {
        private SDTreeNode _treeNode;
        private Stack<SDTreeNode> _stack;
        private SDTreeNode _currentNode;

        public SDTreeNodeNavigator(SDTreeNode treeNode)
        {
            if (treeNode == null)
                throw new ArgumentNullException("treeNode");

            _treeNode = treeNode;
            _stack = new Stack<SDTreeNode>();
            _stack.Push(treeNode);
        }

        public ElementDefinition Current
        {
            get
            {
                return _currentNode.Element;
            }
        }

        public SDTreeNode CurrentNode
        {
            get
            {
                return _currentNode;
            }
        }

        public bool CurrentHasChildren
        {
            get
            {
                return _currentNode.Children.Any();
            }
        }

        public bool MoveNext()
        {
            if (!_stack.Any())
                return false;

            _currentNode = _stack.Pop();

            foreach (SDTreeNode childNode in _currentNode.Children.Reverse())
                _stack.Push(childNode);

            return true;
        }
    }
}
