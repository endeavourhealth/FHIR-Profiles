using Hl7.Fhir.V102;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using FhirProfilePublisher.Specification;

namespace FhirProfilePublisher.Engine
{
    internal class ValueSetHtmlGenerator
    {
        private const string _htmlExtension = "html";
        private ResourceFileSet _profileSet;
        private OutputPaths _outputPaths;
        private Dictionary<ValueSet, string> _fileNames = new Dictionary<ValueSet, string>();

        public ValueSetHtmlGenerator(ResourceFileSet profileSet, OutputPaths outputPaths)
        {
            if (profileSet == null)
                throw new ArgumentNullException("profileSet");

            if (outputPaths == null)
                throw new ArgumentNullException("outputPaths");

            _profileSet = profileSet;
            _outputPaths = outputPaths;
        }

        public void GenerateAll()
        {
            foreach (ValueSetFile valuesetFile in _profileSet.ValueSetFiles)
                Generate(valuesetFile);
        }

        public void Generate(ValueSetFile valuesetFile)
        {
            if (!_profileSet.ValueSetFiles.Contains(valuesetFile))
                throw new ArgumentException("ValueSet does not exist in FhirXmlProfileSet", "valueset");

            string html = GenerateHtml(valuesetFile);

            _outputPaths.WriteUtf8File(OutputFileType.Html, valuesetFile.OutputHtmlFilename, html);
        }

        private string GenerateHtml(ValueSetFile valuesetFile)
        {
            string displayName = valuesetFile.ValueSet.name.value;

            object[] content = GenerateContent(valuesetFile, displayName);

            string contentHtml = Html.Div(content.ToArray()).ToString(SaveOptions.DisableFormatting);

            return Templates.Instance.GetPage(displayName, contentHtml, "0.1", DateTime.Now);
        }

        private object[] GenerateContent(ValueSetFile valuesetFile, string displayName)
        {
            List<object> content = new List<object>();

            ValueSet valueset = valuesetFile.ValueSet;

            string name = valueset.name.value;
            string url = valueset.url.value;

            content.AddRange(new object[]
            {
                Html.H3(GetValueSetNameLabel(name)),
                Html.P("The official URL for this value set is: "),
                Html.Pre(url),
                Html.P(GetMaturity(valuesetFile.Maturity)),
            });

            string description = valueset.description.WhenNotNull(t => t.value);
            string referenceUrl = valueset.GetExtensionValueAsString(FhirConstants.ValueSetSourceReferenceExtensionUrl);

            if (!string.IsNullOrEmpty(description))
            {
                content.AddRange(new object[]
                {
                    Html.H3("Description"),
                    GetFormattedDescription(description, referenceUrl)
                });
            }

            content.AddRange(new object[]
            {
                Html.H3("Definition"),
                GenerateDefinition(valueset)
            });

            if (CanExpand(valueset))
            {
                content.AddRange(new object[]
                {
                    Html.H3("Code expansion"),
                    Html.P("Expansion generated on " + DateTime.Now.ToString("dd-MMM-yyyy") + ".  Please check the source code system(s) for the most recent data."),
                    GenerateCodeExpansion(valueset)
                });
            }

            string copyright = valueset.copyright.WhenNotNull(t => t.value);

            if (!string.IsNullOrEmpty(copyright))
            {
                content.AddRange(new object[]
                {
                    Html.H3("Copyright"),
                    Html.P(copyright)
                });
            }

            return content.ToArray();
        }

        private object[] GetFormattedDescription(string description, string referenceUrl)
        {
            List<object> result = (description ?? string.Empty)
                .Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => (object)Html.P(t))
                .ToList();

            if (!string.IsNullOrWhiteSpace(referenceUrl))
            {
                result.Add(Html.P(new object[]
                {
                    "Reference: ",
                    Html.A(new Link(referenceUrl, referenceUrl)),
                    "."
                }));
            }

            return result.ToArray();
        }

        private object[] GetMaturity(ResourceMaturity maturity)
        {
            return new object[]
            {
                "Maturity level: ",
                Html.Img(_outputPaths.GetRelativePath(OutputFileType.Image, maturity.GetAssociatedIcon())),
                " ",
                maturity.GetDescription(),
                "."
            };
        }

        private object[] GetValueSetNameLabel(string name)
        {
            return new object[] { BootstrapHtml.Label("Value set"), " ", name };
        }

        private bool CanExpand(ValueSet valueset)
        {
            // has defined expansion
            if (valueset.expansion != null)
                if (valueset.expansion.contains.WhenNotNull(t => t.Length > 0))
                    return true;

            if (valueset.compose != null)
            {
                // has import
                if (valueset.compose.import.WhenNotNull(t => t.Length) > 0)
                    return true;

                if (valueset.compose.include != null)
                {
                    // has include with filter
                    if (valueset.compose.include.Any(t => t.filter.WhenNotNull(s => s.Length > 0)))
                    {
                        return true;
                    }
                    else
                    {
                        // has include without concept
                        if (valueset.compose.include.Any(t => t.concept == null))
                            return true;

                        if (valueset.compose.include.Any(t => t.concept.Length == 0))
                            return true;
                    }
                }
            }

            return false;
        }

        private XElement GenerateCodeExpansion(ValueSet valueset)
        {
            ValueSetCode[] expansion = new ValueSetCode[] { };

            bool showCodeSystem = true;

            // has already defined expansion
            if (valueset.expansion.WhenNotNull(t => t.contains.WhenNotNull(s => s.Length > 0)))
            {
                expansion = valueset
                    .expansion
                    .contains
                    .Select(t => new ValueSetCode()
                    {
                        Code = t.code.WhenNotNull(s => s.value),
                        DisplayName = t.display.WhenNotNull(s => s.value),
                        Definition = t.system.WhenNotNull(s => s.value)
                    })
                    .ToArray();

                showCodeSystem = (valueset
                    .expansion
                    .contains
                    .DistinctBy(t => t.system.WhenNotNull(s => s.value)).Count() > 1);
            }
            
            if (valueset.compose != null)
            {
                if (valueset.compose.import != null)
                {
                    foreach (uri uri in valueset.compose.import)
                    {
                        // process valueset imports
                    }
                }

                if (valueset.compose.include != null)
                {
                    foreach (ValueSetInclude include in valueset.compose.include)
                    {
                        if (include.concept.WhenNotNull(t => t.Length > 0))
                            continue;

                        if (include.filter.WhenNotNull(t => t.Length > 0))
                        {
                            foreach (ValueSetFilter filter in include.filter)
                            {
                                // process filters
                            }
                        }
                        else
                        {
                            // process all
                        }
                    }
                }
            }

            object tableRows = Html.Tr(Html.Td(Html.I("(not available)")));

            if (expansion.Length > 0)
            {
                tableRows = expansion
                    .Select(t => Html.Tr(new object[]
                    {
                        Html.Td(t.Code),
                        Html.Td(t.DisplayName),
                        Html.Td(showCodeSystem ? t.Definition : string.Empty)
                    }))
                    .ToArray();
            }

            return BootstrapHtml.Table(Styles.TableLayoutFixedClassName, new object[] 
            {
                Html.THead(new object[]
                {
                    Html.Th(Styles.TableColumn20pcClassName, "Code"),
                    Html.Th(Styles.TableColumn20pcClassName, "Display"),
                    Html.Th(showCodeSystem ? "Code system" : string.Empty)
                }),
                Html.TBody
                (
                    tableRows
                )
            });
        }

        private object[] GenerateDefinition(ValueSet valueset)
        {
            List<object> result = new List<object>();
            
            bool isFirst = true;

            if (valueset.codeSystem != null)
            {
                result.Add(GenerateInlineValuesetHtml(valueset.codeSystem));

                isFirst = false;
            }

            if (valueset.compose != null)
            {
                if (valueset.compose.import.WhenNotNull(t => t.Length) > 0)
                    result.Add(GenerateImportValuesetHtml(valueset.compose.import, isFirst));

                if (valueset.compose.include != null)
                {
                    foreach (ValueSetInclude include in valueset.compose.include)
                    {
                        string name = include.GetExtensionValueAsString(FhirConstants.ValueSetSystemNameExtensionUrl);
                        string url = include.GetExtensionValueAsString(FhirConstants.ValueSetSystemUrlExtensionUrl);

                        Link additionalValueSetLink = null;

                        if ((!string.IsNullOrWhiteSpace(name)) && (!string.IsNullOrWhiteSpace(url)))
                            additionalValueSetLink = new Link(url, name);

                        if (include.concept.WhenNotNull(t => t.Length) > 0)
                            result.Add(GenerateIncludeCodesValuesetHtml(include, isFirst));
                        else if (include.filter.WhenNotNull(t => t.Length > 0))
                            result.Add(GenerateIncludeFilterValuesetHtml(include, isFirst));
                        else
                            result.Add(GenerateIncludeAllValuesetHtml(include, isFirst, additionalValueSetLink));

                        isFirst = false;
                    }
                }
            }

            return result.ToArray();
        }

        private object[] GenerateInlineValuesetHtml(ValueSetCodeSystem codeSystem)
        {
            return new object[]
            {
                GetInlineDescription(true, codeSystem.system.value),
                GenerateCodeTable(ValueSetCode.FromValueSetConcept(codeSystem.concept), codeSystem.system, false)
            };
        }

        private object[] GenerateImportValuesetHtml(uri[] importUris, bool isFirst)
        {
            return new object[]
            {
                GetImportDescription(isFirst, importUris.Any()),
                Html.Ul(importUris.Select(t => Html.Li(Html.A(t.value, t.value))))
            };
        }

        private object[] GenerateIncludeFilterValuesetHtml(ValueSetInclude include, bool isFirst)
        {
            return new object[]
            {
                GetIncludeFilterDescription(isFirst, include.system.value),
                Html.Ul(include
                    .filter
                    .Select(t => Html.Li(GetIncludeFilterClauseDescription(t, include.system)))
                    .ToArray())
            };
        }

        private object[] GetIncludeFilterClauseDescription(ValueSetFilter filter, uri codeSystem)
        {
            return new object[] 
            {
                filter.property.value + " ",
                BootstrapHtml.Abbreviation(filter.op.value.GetDescription(), Html.B(filter.op.value.GetName())),
                " ",
                GetCodeWithHyperlink(filter.value.value, codeSystem.value)
            };
        }

        private static object GetCodeWithHyperlink(string code, string codeSystem)
        {
            object result = code;

            if (SnomedHelper.IsSnomedSystemUri(codeSystem))
                result = Html.A(SnomedHelper.GetBrowserUrl(code), code);

            return result;
        }

        private object[] GenerateIncludeCodesValuesetHtml(ValueSetInclude include, bool isFirst)
        {
            return new object[]
            {
                GetIncludeCodesDescription(isFirst, include.system.value),
                GenerateCodeTable(ValueSetCode.FromValueSetConcept1(include.concept), include.system, true)
            };
        }

        private object[] GenerateIncludeAllValuesetHtml(ValueSetInclude include, bool isFirst, Link additionalValueSetLink)
        {
            return new object[]
            {
                GetIncludeAllDescription(isFirst, include.system.value, additionalValueSetLink)
            };
        }

        private object GetDescriptionPrefix(bool isFirst)
        {
            return (isFirst ? (object)"This valueset is comprised of " : (object)Html.B("AND "));
        }

        private XElement GetImportDescription(bool isFirst, bool moreThanOneUri)
        {
            return Html.P(new object[]
            {
                GetDescriptionPrefix(isFirst),
                "ALL codes from the following ",
                Html.B("valueset" + (moreThanOneUri ? "s" : string.Empty)),
                ":"
            });
        }

        private XElement GetIncludeAllDescription(bool isFirst, string systemUri, Link additionalValueSetLink)
        {
            return Html.P(new object[]
            {
                GetDescriptionPrefix(isFirst),
                Html.B("ALL "),
                "codes from the following ",
                Html.B("external code system "),
                GetCodeSystemLink(systemUri),
                GetAdditionalValueSetLink(additionalValueSetLink),
                "."
            });
        }

        private object GetCodeSystemLink(string systemUri)
        {
            if (!WebHelper.IsHttpUrl(systemUri))
                return systemUri;

            return Html.A(systemUri, systemUri);
        }

        private object[] GetAdditionalValueSetLink(Link additionalValueSetLink)
        {
            if (additionalValueSetLink == null)
                return null;

            return new object[]
            {
                ", ",
                additionalValueSetLink.GetAsXElement()
            };
        }

        private XElement GetIncludeFilterDescription(bool isFirst, string systemUri)
        {
            return Html.P(new object[]
            {
                GetDescriptionPrefix(isFirst),
                "codes from ",
                Html.B("external code system "),
                Html.A(systemUri, systemUri),
                " where:"
            });
        }

        private XElement GetInlineDescription(bool isFirst, string systemUri)
        {
            return Html.P(new object[] 
            {
                GetDescriptionPrefix(isFirst),
                " the following codes from ",
                Html.B("inline code system "),
                Html.A(systemUri, systemUri),
                ":"
            });
        }

        private XElement GetIncludeCodesDescription(bool isFirst, string systemUri)
        {
            return Html.P(new object[] 
            {
                GetDescriptionPrefix(isFirst),
                " the following codes from ",
                Html.B("external code system "),
                Html.A(systemUri, systemUri),
                ":"
            });
        }

        private XElement GenerateCodeTable(ValueSetCode[] codes, uri codeSystem, bool isExternal)
        {
            bool showDefinition = (!isExternal) && (!codes.All(t => string.IsNullOrWhiteSpace(t.Definition)));

            return BootstrapHtml.Table(Styles.TableLayoutFixedClassName, new object[] 
            {
                Html.THead(new object[]
                {
                    Html.Th(Styles.TableColumn20pcClassName, "Code"),
                    Html.Th(Styles.TableColumn20pcClassName, "Display"),
                    Html.Th(showDefinition ? "Definition" : string.Empty)
                }),
                Html.TBody
                (
                    codes.Select(t => Html.Tr(new object[]
                    {
                        Html.Td(GetCodeWithHyperlink(t.Code, codeSystem.WhenNotNull(s => s.value))),
                        Html.Td(t.DisplayName ?? (isExternal ? "(see external code system)" : string.Empty)),
                        Html.Td(showDefinition ? t.Definition : string.Empty)
                    }))
                )
            });
        }
    }
}
