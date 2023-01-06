using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace ExtenDev.LINQPad.Extensions.UI
{
    // TODO: Add XML comments to all members
    public class OpenWithOption
    {
        private Action<string, int?> openAction;

        public OpenWithOption(string caption, string processPath,
            string fileFormatArguments, string lineFormatArguments = null,
            bool requiresAdmin = false, bool requiresLineNumber = false) :
            this(caption,
                (fullPath, lineNumber) => OpenWithProcess(processPath, fileFormatArguments, lineFormatArguments, requiresAdmin, fullPath, lineNumber),
                requiresLineNumber: requiresLineNumber)
        {
        }

        public OpenWithOption(string caption, Action<string, int?> openAction, bool requiresLineNumber = false)
        {
            Caption = caption;
            RequiresLineNumber = requiresLineNumber;
            this.openAction = openAction;
        }

        public string Caption { get; }
        public bool RequiresLineNumber { get; }

        internal static void OpenWithProcess(
            string processPath,
            string fileFormatArguments = null, string lineFormatArguments = null,
            bool requiresAdmin = false,
            string fullPath = null, int? lineNumber = null)
        {
            var args = fileFormatArguments;
            if (lineNumber != null && lineFormatArguments != null)
            {
                args = lineFormatArguments;
            }

            if (args != null)
            {
                args = string.Format(System.Globalization.CultureInfo.InvariantCulture, args,
                    fullPath, lineNumber);
            }
            ProcessStartInfo startInfo = new ProcessStartInfo(processPath);
            if (args != null) startInfo.Arguments = args;
            if (requiresAdmin) startInfo.Verb = "runas";

            Process.Start(startInfo);
        }

        public void Open(string fullPath, int? lineNumber = null)
        {
            openAction(fullPath, lineNumber);
        }

        public static OpenWithOption SimpleShellOption = new OpenWithOption("&Shell", (fullPath, lineNumber) => OpenWithProcess(fullPath));
        public static OpenWithOption ShellAsAdminOption = new OpenWithOption("&Shell (As Admin)", (fullPath, lineNumber) => OpenWithProcess(fullPath, requiresAdmin: true));
        public static OpenWithOption CopyPathToClipboard = new OpenWithOption("&Copy Path to Clipboard", (fullPath, lineNumber) => Clipboard.SetText(fullPath));
        public static OpenWithOption CopyLineNumberToClipboard = new OpenWithOption("Copy &Line Number to Clipboard", (fullPath, lineNumber) => Clipboard.SetText(lineNumber.Value.ToString()), true);
    }
}
