using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir.V102;
using System.Xml;
using System.Xml.Linq;

namespace FhirProfilePublisher.Engine
{
    public class HtmlGenerator
    {
        public HtmlGenerator()
        {
        }

        public string Generate(string[] inputFilePaths, string outputDirectory, TextContent textContent)
        {
            if (inputFilePaths == null)
                throw new ArgumentNullException("inputFilePaths");

            if (string.IsNullOrWhiteSpace(outputDirectory))
                throw new ArgumentNullException("outputDirectory");

            OutputPaths outputPaths = new OutputPaths
            (
                outputDirectory: Path.Combine(outputDirectory, "Generated"),
                stylesRelativePath: "styles",
                imagesRelativePath: "images",
                scriptsRelativePath: "scripts",
                structureDefinitionPath: "StructureDefinition", 
                valueSetPath: "ValueSet"
            );

            Templates.Instance.PageHeader = textContent.HeaderText;
            Templates.Instance.PageTitleSuffix = textContent.PageTitleSuffix;

            ResourceFileSet resourceFileSet = new ResourceFileSet();
            resourceFileSet.LoadXmlResourceFiles(inputFilePaths);

            return GenerateHtml(resourceFileSet, outputPaths, textContent);
        }

        private string GenerateHtml(ResourceFileSet resourceFileSet, OutputPaths outputPaths, TextContent textContent)
        {
            // copy supporting files
            Styles.WriteStylesToDisk(outputPaths);
            Images.WriteImagesToDisk(outputPaths);
            Scripts.WriteScriptsToDisk(outputPaths);

            // structure definition pages
            StructureDefinitionHtmlGenerator structureDefinitionGenerator = new StructureDefinitionHtmlGenerator(resourceFileSet, outputPaths);
            structureDefinitionGenerator.GenerateAll();

            // resource listing page
            ResourceListingHtmlGenerator resourceListingGenerator = new ResourceListingHtmlGenerator(outputPaths);
            resourceListingGenerator.GenerateStructureDefinitionListing("resources.html", resourceFileSet);

            // valueset pages
            ValueSetHtmlGenerator valuesetGenerator = new ValueSetHtmlGenerator(resourceFileSet, outputPaths);
            valuesetGenerator.GenerateAll();

            // valueset listing page
            ResourceListingHtmlGenerator valueSetsListingGenerator = new ResourceListingHtmlGenerator(outputPaths);
            resourceListingGenerator.GenerateValueSetListing("valuesets.html", resourceFileSet);

            // other pages
            GenericPageGenerator pageGenerator = new GenericPageGenerator(outputPaths);
            pageGenerator.Generate("index.html", "Overview", textContent.IndexPageHtml);

            pageGenerator.Generate("api.html", "API", GetApiPageContent());

            // profile xml and json files
            SourceFileManager sourceGenerator = new SourceFileManager(outputPaths, resourceFileSet);
            sourceGenerator.CopyXml();
            sourceGenerator.GenerateJson();
            sourceGenerator.CreateRedirectsForProfileUrls();

            return outputPaths.GetOutputPath(OutputFileType.Html, "index.html");
        }

        private string GetApiPageContent()
        {
            return Html.Div(new object[]
            {
                Html.H3("API"),
                Html.P(new object[]
                {
                    "Please see ",
                    Html.A("http://endeavour-cim.cloudapp.net/", "API documentation here"),
                    "."
                })
            }).ToString();
        }
    }
}
