using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.V101;

namespace FhirProfilePublisher.Engine
{
    internal class ElementNavigator
    {
        private TreeNode _treeNode;
        private Stack<TreeNode> _stack;
        private TreeNode _currentNode;

        public ElementNavigator(TreeNode treeNode)
        {
            if (treeNode == null)
                throw new ArgumentNullException("treeNode");

            _treeNode = treeNode;
            _stack = new Stack<TreeNode>();
            _stack.Push(treeNode);
            SkipExtensionSlice = false;
        }

        public bool SkipExtensionSlice { get; set; }

        public ElementDefinition Current
        {
            get
            {
                return _currentNode.Element;
            }
        }

        public TreeNode CurrentNode
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

            foreach (TreeNode childNode in _currentNode.Children.Reverse())
                _stack.Push(childNode);

            return true;
        }
    }
}
