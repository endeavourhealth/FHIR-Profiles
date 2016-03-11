using FhirProfilePublisher.Specification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FhirProfilePublisher.Engine
{
    internal static class Scripts
    {
        public static readonly string ScriptsResourceLocation = typeof(Styles).Assembly.GetName().Name + ".WebContent.Scripts.";
        public const string BootstrapScriptFileName = "bootstrap.js";
        public const string JQueryScriptFileName = "jquery-2.1.4.js";
        public const string JQueryUIScriptFileName = "jquery-ui.js";
        public const string TreeViewHelpersScriptFileName = "treeviewhelpers.js";

        public static void WriteScriptsToDisk(OutputPaths outputPaths)
        {
            string[] scriptNames = GetScriptNames();

            foreach (string scriptName in scriptNames)
                ResourceHelper.WriteResourceToDisk(ScriptsResourceLocation + scriptName, outputPaths.GetOutputPath(OutputFileType.Script, scriptName));
        }

        private static string[] GetScriptNames()
        {
            return new string[]
            {
                JQueryScriptFileName,
                BootstrapScriptFileName
            };
        }

        internal static object GetScriptTags(OutputPaths outputPaths)
        {
            return GetScriptNames().Select(t => Html.Script(outputPaths.GetRelativePath(OutputFileType.Script, t))).ToArray();
        }

        public static string GetScript(string scriptName)
        {
            return ResourceHelper.LoadStringResource(ScriptsResourceLocation + TreeViewHelpersScriptFileName);
        }
    }
}
