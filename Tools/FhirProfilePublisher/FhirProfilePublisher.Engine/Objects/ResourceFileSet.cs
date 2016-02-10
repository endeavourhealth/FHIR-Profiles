﻿using Hl7.Fhir.V101;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Engine
{
    internal class ResourceFileSet
    {
        private List<StructureDefinitionFile> _structureDefinitions = new List<StructureDefinitionFile>();
        private List<ValueSetFile> _valueSets = new List<ValueSetFile>();

        public ResourceFileSet()
        {
        }

        public StructureDefinitionFile[] StructureDefinitionFiles
        {
           get 
           {
               return _structureDefinitions.ToArray();
           }
        }

        public ValueSetFile[] ValueSetFiles
        {
            get
            {
                return _valueSets.ToArray();
            }
        }

        public StructureDefinitionFile[] StructureDefinitionFilesWithoutExtensions
        {
            get
            {
                return StructureDefinitionFiles
                    .Where(t => !t.StructureDefinition.IsExtension())
                    .ToArray();
            }
        }

        public Dictionary<string, StructureDefinitionFile[]> StructureDefinitionsByW5Group
        {
            get
            {
                return StructureDefinitionFilesWithoutExtensions
                    .GroupBy(t => t.StructureDefinition.W5TopLevelGroup)
                    .ToDictionary(t => t.Key, t => t.ToArray());
            }
        }

        public StructureDefinitionFile[] StructureDefinitionExtensionFiles
        {
            get
            {
                return StructureDefinitionFiles
                    .Where(t => t.StructureDefinition.IsExtension())
                    .ToArray();
            }
        }

        public ResourceFile[] Files
        {
            get
            {
                return StructureDefinitionFiles
                    .Select(t => (ResourceFile)t)
                    .Concat(ValueSetFiles.Select(t => (ResourceFile)t))
                    .ToArray();
            }
        }

        public void LoadXmlResourceFiles(string[] inputFilePaths)
        {
            string[] inputFiles = inputFilePaths
                .Where(t => !string.IsNullOrEmpty(t))
                .Select(t => Utilities.ReadInputFile(t))
                .ToArray();

            foreach (string inputFile in inputFiles)
                AddXmlResourceFile(inputFile);
        }

        private void AddXmlResourceFile(string fhirProfileXml)
        {
            string rootNodeName = XmlHelper.GetRootNodeName(fhirProfileXml);

            if (rootNodeName == typeof(StructureDefinition).Name)
                _structureDefinitions.Add(new StructureDefinitionFile(fhirProfileXml));
            else if (rootNodeName == typeof(ValueSet).Name)
                _valueSets.Add(new ValueSetFile(fhirProfileXml));
            else
                throw new NotSupportedException(rootNodeName + " not recognised as FHIR profile resource.");
        }

        public StructureDefinition GetStructureDefinition(string url, bool alsoCheckBaseProfiles = true)
        {
            StructureDefinition definition = _structureDefinitions.FirstOrDefault(t => t.StructureDefinition.url.value == url).WhenNotNull(t => t.StructureDefinition);

            if (definition == null)
                if (alsoCheckBaseProfiles)
                    definition = FhirData.Instance.FindStructureDefinition(url);

            if (definition == null)
                throw new ArgumentException(string.Format("Could not find base definition {0}", url), "definition");
            
            return definition;
        }

        public Link GetStructureDefinitionLink(string canonicalUrl)
        {
            StructureDefinitionFile structureDefinitionFile = _structureDefinitions.SingleOrDefault(t => t.CanonicalUrl == canonicalUrl);

            if (structureDefinitionFile != null)
                return structureDefinitionFile.Link;

            StructureDefinition structureDefinition = FhirData.Instance.FindStructureDefinition(canonicalUrl);

            if (structureDefinition == null)
                throw new Exception("StructureDefinition " + canonicalUrl + " not found.");

            return new Link
            (
                url: (structureDefinition == null) ? canonicalUrl : structureDefinition.url.value,
                display: (structureDefinition == null) ? "WARNING" : structureDefinition.name.value
            );
        }

        public Link GetValueSetLink(string canonicalUrl)
        {
            ValueSetFile valueSetFile = _valueSets.SingleOrDefault(t => t.CanonicalUrl == canonicalUrl);

            if (valueSetFile != null)
                return valueSetFile.Link;

            ValueSet valueSet = FhirData.Instance.FindValueSet(canonicalUrl);

            if (valueSet == null)
                throw new Exception("ValueSet " + canonicalUrl + " not found.");

            return new Link
            (
                url: valueSet.url.value,
                display: valueSet.name.value
            );
        }
    }
}