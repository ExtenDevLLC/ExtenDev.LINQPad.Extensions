using System;
using System.Collections.Generic;
using System.Text;
using ExtenDev.LINQPad.Extensions.IO;
using LINQPad;

namespace ExtenDev.LINQPad.Extensions.Development
{
    // TODO: Add XML comments to all members
    public static class DevelopmentEnvironmentExtensions
    {

        public static void CheckOutInTFS(this string path)
        {
            ProcessWrapper.DumpOutput(ExtensionSettings.TeamFoundationClientExePath, $@"checkout ""{path}""");
        }

        public static void AddInTFS(this string path)
        {
            ProcessWrapper.DumpOutput(ExtensionSettings.TeamFoundationClientExePath, $@"add ""{path}""");
        }

        public static void RenameInTFS(this string path, string newPath)
        {
            ProcessWrapper.DumpOutput(ExtensionSettings.TeamFoundationClientExePath, $@"rename ""{path}"" ""{newPath}""");
        }

        public static void DeleteInTFS(this string path)
        {
            ProcessWrapper.DumpOutput(ExtensionSettings.TeamFoundationClientExePath, $@"delete ""{path}""");
        }

#if NET472
        public static string DumpWithSyntaxHightlighting(this string code, SyntaxLanguageStyle language = SyntaxLanguageStyle.XML, string panelTitle = null)
        {
            if (panelTitle == null)
                panelTitle = Enum.GetName(typeof(SyntaxLanguageStyle), language) + "Result";

            PanelManager.DisplaySyntaxColoredText(code, language, panelTitle);
            return code;
        }
#endif
    }
}
