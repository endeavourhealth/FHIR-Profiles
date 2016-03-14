using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.V102;

namespace FhirProfilePublisher.Specification
{
    internal sealed class FhirSchemas
    {
        #region Class members

        private static readonly string AssemblyName = typeof(FhirSchemas).Assembly.GetName().Name;
        private static readonly string SchemaFilesPath = AssemblyName + ".Specification.Schemas.fhir_v1._0._1.";
        private static readonly string FhirSingleSchemaFilePath = SchemaFilesPath + "fhir-single.xsd";
        private static readonly string FhirXHtmlSchemaFilePath = SchemaFilesPath + "fhir-xhtml.xsd";
        private static readonly string FhirXmlSchemaFilePath = SchemaFilesPath + "xml.xsd";
        private static readonly string FhirSchematronFilePath = SchemaFilesPath + "fhir-invariants.sch";

        private static volatile FhirSchemas _instance;
        private static object _key = new Object();

        public static FhirSchemas Instance
        {
            get
            {
                if (_instance == null)
                    lock (_key)
                        if (_instance == null)
                            _instance = new FhirSchemas();

                return _instance;
            }
        }

        #endregion

        #region Instance members

        private string[] Xsds { get; set; }
        private string Schematron { get; set; }

        private FhirSchemas()
        {
            LoadSchemas();
        }

        private void LoadSchemas()
        {
            Xsds = new List<string>()
            {
                ResourceHelper.LoadStringResource(FhirSingleSchemaFilePath),
                ResourceHelper.LoadStringResource(FhirXHtmlSchemaFilePath),
                ResourceHelper.LoadStringResource(FhirXmlSchemaFilePath)
            }.ToArray();

            Schematron = ResourceHelper.LoadStringResource(FhirSchematronFilePath);
        }

        public void ValidateFhirXml(string xml)
        {
            XmlHelper.Validate(xml, Xsds.ToArray());
        }

        #endregion
    }
}
    
