/*using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Snapshot;
using Hl7.Fhir.Specification.Source;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FhirProfilePublisher.Engine
{
    public class HtmlGeneratorNew
    {
        List<StructureDefinition> _structureDefinitions = new List<StructureDefinition>();
        List<ValueSet> _valueSets = new List<ValueSet>();

        public void Generate(string[] inputFilePaths, string outputDirectory, TextContent textContent)
        {
            foreach (string inputFilePath in inputFilePaths)
            {
                if (string.IsNullOrWhiteSpace(inputFilePath))
                    continue;

                string inputFile = Utilities.ReadInputFile(inputFilePath);  
                XmlReader reader = XmlReader.Create(new StringReader(inputFile));

                Resource resource = FhirParser.ParseResource(reader);

                if (resource.ResourceType == ResourceType.StructureDefinition)
                    _structureDefinitions.Add((StructureDefinition)resource);
                else if (resource.ResourceType == ResourceType.ValueSet)
                    _valueSets.Add((ValueSet)resource);
                else
                    throw new Exception("Resource type not supported");
            }

            ArtifactResolver resolver = ArtifactResolver.CreateOffline();
            SnapshotGenerator snapshotGenerator = new SnapshotGenerator(resolver);

            foreach (StructureDefinition structureDefinition in _structureDefinitions)
            {
                if (structureDefinition.IsConstraint)
                {
                    snapshotGenerator.Generate(structureDefinition);

                    string serialized = FhirSerializer.SerializeResourceToXml(structureDefinition);
                    Utilities.WriteText("X:\\a.xml", serialized);
                }
            }

            

        }
    }
}
*/