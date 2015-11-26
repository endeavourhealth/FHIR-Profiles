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
        private IEnumerable<ElementDefinition> _elements;
        private Stack<ElementDefinition> _stack;
        private ElementDefinition _currentElement;

        public ElementNavigator(IEnumerable<ElementDefinition> elements)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");
            
            _elements = elements;
            _stack = new Stack<ElementDefinition>();
            _stack.Push(GetRootElement());
            SkipExtensionSlice = false;
        }

        public bool SkipExtensionSlice { get; set; }

        public ElementDefinition Current
        {
            get
            {
                return _currentElement;
            }
        }

        public bool CurrentHasChildren
        {
            get
            {
                return HasChildren(_currentElement);
            }
        }

        public bool MoveNext()
        {
            if (!_stack.Any())
                return false;

            _currentElement = _stack.Pop();

            if (_currentElement.IsEmptyExtensionSlice() && SkipExtensionSlice)
                return MoveNext();

            if (!_currentElement.IsRemoved())
                foreach (ElementDefinition childElement in GetChildren(_currentElement).Reverse())
                    _stack.Push(childElement);

            return true;
        }

        private static ElementDefinition GenerateFakeDataTypeChoiceElement(ElementDefinition element, ElementDefinitionType type)
        {
            ElementDefinition dataTypeChoiceElement = new ElementDefinition();
            dataTypeChoiceElement.path = new Hl7.Fhir.V101.@string();
            dataTypeChoiceElement.path.value = GenerateFakeDataTypeChoicePath(element, type);
            dataTypeChoiceElement.type = new ElementDefinitionType[] { type };
            return dataTypeChoiceElement;
        }

        private static string GenerateFakeDataTypeChoicePath(ElementDefinition element, ElementDefinitionType type)
        {
            return element.path.value + "." + element.GetNameFromPath().Replace("[x]", Utilities.UpperCaseFirstCharacter(type.code.value));
        }

        public bool[] GetHierarchyImageDefinition()
        {
            List<bool> definition = new List<bool>();

            string path = string.Empty;

            foreach (string pathElement in Current.path.value.Split('.'))
            {
                if (path != string.Empty)
                    definition.Add(!HasRemaingingSiblings(path + pathElement));

                path += pathElement + ".";
            }

            return definition.ToArray();
        }

        private ElementDefinition GetRootElement()
        {
            return _elements.Single(t => t.path.value.Split('.').Count() == 1);
        }

        private bool HasRemaingingSiblings(string path)
        {
            return _stack.Any(t => IsSibling(path, t));
        }

        private bool IsSibling(string path, ElementDefinition element)
        {
            return (RemoveLastPathElement(path) == RemoveLastPathElement(element.path.value));
        }

        private static string RemoveLastPathElement(string path)
        {
            return string.Join(".", path.Split('.').AllExceptLast());
        }

        private int CurrentDepth
        {
            get
            {
                return Current.path.value.Split('.').Count() - 1;
            }
        }

        private bool HasChildren(ElementDefinition parent)
        {
            return (GetChildren(parent).Any() || _currentElement.HasDataTypeChoice());
        }

        private IEnumerable<ElementDefinition> GetChildren(ElementDefinition parent)
        {
            List<ElementDefinition> result = new List<ElementDefinition>();

            result.AddRange(_elements.Where(t => IsDirectChildpath(parent.path.value, t.path.value)).ToArray());

            if (parent.HasDataTypeChoice())
                result.AddRange(parent.type.Select(t => GenerateFakeDataTypeChoiceElement(_currentElement, t)));

            return result;
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
