using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Specification
{
    public class StringHelper
    {
        public static string UpperCaseFirstCharacter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}
