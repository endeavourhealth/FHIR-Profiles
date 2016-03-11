using Hl7.Fhir.V101;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Specification
{
    public class FhirData
    {
        #region Class members

        private static readonly string AssemblyName = typeof(FhirSchemas).Assembly.GetName().Name;
        private static readonly string DataFilesPath = AssemblyName + ".Assets.Fhir.Data.fhir_v1._0._1.";
        private static readonly string ConceptMapsDataFilePath = DataFilesPath + "conceptmaps.xml";
        private static readonly string DataElementsDataFilePath = DataFilesPath + "dataelements.xml";
        private static readonly string ExtensionDefinitionsDataFilePath = DataFilesPath + "extension-definitions.xml";
        private static readonly string ProfilesOthersDataFilePath = DataFilesPath + "profiles-others.xml";
        private static readonly string ProfilesResourcesDataFilePath = DataFilesPath + "profiles-resources.xml";
        private static readonly string ProfilesTypesDataFilePath = DataFilesPath + "profiles-types.xml";
        private static readonly string SearchParametersDataFilePath = DataFilesPath + "search-parameters.xml";
        private static readonly string V2TablesDataFilePath = DataFilesPath + "v2-tables.xml";
        private static readonly string V3CodeSystemsDataFilePath = DataFilesPath + "v3-codesystems.xml";
        private static readonly string ValueSetsDataFilePath = DataFilesPath + "valuesets.xml";

        private static volatile FhirData _instance;
        private static object _key = new Object();

        public static FhirData Instance
        {
            get
            {
                if (_instance == null)
                    lock (_key)
                        if (_instance == null)
                            _instance = new FhirData();

                return _instance;
            }
        }

        #endregion

        #region Instance members

        public StructureDefinition[] ResourceDefinitions { get; private set; }
        private ValueSet[] ValueSets { get; set; }

        public string[] PrimitiveDataTypeNames { get; private set; }
        public string[] ComplexDataTypeNames { get; private set; }

        private FhirData()
        {
            LoadStructureDefinitions();
            LoadValueSets();
        }

        private void LoadStructureDefinitions()
        {
            StructureDefinition[] resourceDefinitions = GetBundleEntries<StructureDefinition>(ProfilesResourcesDataFilePath);
            StructureDefinition[] dataTypeDefinitions = GetBundleEntries<StructureDefinition>(ProfilesTypesDataFilePath);
            StructureDefinition[] extensionDefinitions = GetBundleEntries<StructureDefinition>(ExtensionDefinitionsDataFilePath);

            PrimitiveDataTypeNames = GetPrimitiveDataTypeNames(dataTypeDefinitions);
            ComplexDataTypeNames = GetComplexDataTypeNames(dataTypeDefinitions, PrimitiveDataTypeNames);

            ResourceDefinitions = resourceDefinitions
                                    .Concat(dataTypeDefinitions)
                                    .Concat(extensionDefinitions)
                                    .ToArray();
        }

        private void LoadValueSets()
        {
            ValueSet[] valueSetDefinitions = GetBundleEntries<ValueSet>(ValueSetsDataFilePath);
            ValueSet[] v3CodeSystemDefinitions = GetBundleEntries<ValueSet>(V3CodeSystemsDataFilePath);
            ValueSet[] v2TableDefinitions = GetBundleEntries<ValueSet>(V2TablesDataFilePath);

            ValueSets = valueSetDefinitions
                            .Concat(v3CodeSystemDefinitions)
                            .Concat(v2TableDefinitions)
                            .ToArray();
        }

        private Bundle LoadBundle(string resourcePath)
        {
            return XmlHelper.Deserialize<Bundle>(ResourceHelper.LoadStringResource(resourcePath));
        }

        private T[] GetBundleEntries<T>(string resourcePath) where T : class
        {
            return GetBundleEntries<T>(LoadBundle(resourcePath));
        }

        private T[] GetBundleEntries<T>(Bundle bundle) where T : class
        {
            return bundle
                .entry
                .Where(t => t.resource.Item is T)
                .Select(t => t.resource.Item as T)
                .ToArray();
        }

        public ValueSet FindValueSet(string canonicalUrl)
        {
            string valueSetUriNormalized = Fhir.NormaliseValueSetUri(canonicalUrl);
            return ValueSets.FirstOrDefault(t => t.url.value == valueSetUriNormalized);
        }

        public StructureDefinition FindStructureDefinition(string canonicalUrl)
        {
            return ResourceDefinitions.SingleOrDefault(t => t.url.value == canonicalUrl);
        }

        private static string[] GetPrimitiveDataTypeNames(StructureDefinition[] dataTypeDefinitions)
        {
            List<string> result = new List<string>();

            foreach (StructureDefinition d in dataTypeDefinitions)
            {
                string[] differentialPathValues = d.differential
                    .element
                    .Select(t => string.Join(".", t
                        .path
                        .value
                        .Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)
                        .Skip(1)))
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToArray();

                if (differentialPathValues.Length == 1)
                    if (differentialPathValues.Single() == "value")
                        result.Add(d.name.value);
            }

            return result
                .OrderBy(t => t)
                .ToArray();
        }

        private static string[] GetComplexDataTypeNames(StructureDefinition[] dataTypeDefinitions, string[] simpleDataTypeNames)
        {
            return dataTypeDefinitions
                .Where(t => !simpleDataTypeNames.Contains(t.name.value))
                .Select(t => t.name.value)
                .OrderBy(t => t)
                .ToArray();
        }

        public bool IsDataTypeName(string typeName)
        {
            return (IsPrimitiveTypeName(typeName) || IsComplexTypeName(typeName));
        }

        public bool IsPrimitiveTypeName(string typeName)
        {
            return PrimitiveDataTypeNames.Contains(typeName);
        }

        public bool IsComplexTypeName(string typeName)
        {
            return ComplexDataTypeNames.Contains(typeName);
        }

        #endregion
    }
}
