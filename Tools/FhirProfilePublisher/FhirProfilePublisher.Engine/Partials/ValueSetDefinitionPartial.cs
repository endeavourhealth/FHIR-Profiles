﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FhirProfilePublisher.Engine;

namespace Hl7.Fhir.V101
{
    public partial class ValueSet
    {
        public string GetExtensionValueAsString(string extensionUrl)
        {
            if (extension == null)
                return null;

            return extension
                .FirstOrDefault(t => t.url == extensionUrl)
                .WhenNotNull(t => t.Item.GetValueAsString());
        }
    }
}
