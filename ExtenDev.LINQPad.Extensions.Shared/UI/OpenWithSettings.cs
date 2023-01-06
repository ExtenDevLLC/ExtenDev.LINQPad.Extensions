using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using ExtenDev.LINQPad.Extensions.Development;
using ExtenDev.LINQPad.Extensions.IO;
using LINQPad;

namespace ExtenDev.LINQPad.Extensions.UI
{
    // TODO: Add XML comments to all members
    public sealed class OpenWithSettings
    {
        // TODO: Separate default configuration, global configuration, and instance configuration
        // TODO: Allow new options to be added from UI?
        // TODO: Persist preferences (see ExtensionSettings)

        public List<OpenWithOption> AllFilesOptions { get; }
        public List<OpenWithOption> AllFoldersOptions { get; }
        public Dictionary<string, List<OpenWithOption>> OptionsByExtension { get; }
        public Dictionary<string, OpenWithOption> DefaultOptionByExtension { get; }
        public Dictionary<string, OpenWithOption> DefaultOptionByExtensionWithLineNumber { get; }

        public OpenWithOption AllLineNumbersDefaultOption { get; set; }
        public OpenWithOption AllFilesDefaultOption { get; set; }
        public OpenWithOption AllFoldersDefaultOption { get; set; }

        private OpenWithSettings()
        {
            AllFilesOptions = new List<OpenWithOption>();
            AllFoldersOptions = new List<OpenWithOption>();
            OptionsByExtension = new Dictionary<string, List<OpenWithOption>>();
            DefaultOptionByExtension = new Dictionary<string, OpenWithOption>();
            DefaultOptionByExtensionWithLineNumber = new Dictionary<string, OpenWithOption>();
        }

        public void AddOptionForExtension(string extension, OpenWithOption option)
        {

            List<OpenWithOption> options;
            if (!OptionsByExtension.TryGetValue(extension, out options))
            {
                options = new List<OpenWithOption>();
                OptionsByExtension.Add(extension, options);
            }
            options.Add(option);
        }

        private static IEnumerable<OpenWithOption> GetAllFilesOptions() =>
            new HashSet<OpenWithOption>(AllSettings.SelectMany(s => s.AllFilesOptions));

        public static IEnumerable<OpenWithOption> GetFileOptionsByExtension(string extension) =>
            new HashSet<OpenWithOption>(
                AllSettings.SelectMany(s => s.OptionsByExtension.TryGetValue(extension, out var options)
                ? options
                : Enumerable.Empty<OpenWithOption>()));

        public static OpenWithOption GetDefaultOptionByExtension(string extension, bool withLineNumber)
        {
            var defaults = Enumerable.Empty<Dictionary<string, OpenWithOption>>();
            if (withLineNumber)
            {
                defaults = AllSettings.Select(s => s.DefaultOptionByExtensionWithLineNumber);
            }
            defaults = defaults.Concat(AllSettings.Select(s => s.DefaultOptionByExtension));

            return defaults.GetFirstValueOrDefault(extension);
        }

        private static IEnumerable<OpenWithOption> GetAllFoldersOptions() =>
            new HashSet<OpenWithOption>(AllSettings.SelectMany(s => s.AllFoldersOptions));

        public static OpenWithOption GetDefaultFolderOption() =>
            AllSettings.Select(s => s.AllFoldersDefaultOption).FirstOrDefault(o => o != null);

        public static OpenWithSettings Global { get; private set; }
        public static OpenWithSettings Session { get; private set; }
        public static OpenWithSettings Query { get; private set; }
        private static OpenWithSettings[] AllSettings { get; set; }

        public static Dictionary<string, OpenWithOption> GlobalDefaultOptionByExtension { get; }

        static OpenWithSettings()
        {
            Global = new OpenWithSettings();

            {
                var session = AppDomain.CurrentDomain.GetData("OpenWithSettings.Session") as OpenWithSettings;
                if (session != null)
                {
                    Session = session;
                }
                else
                {
                    Session = new OpenWithSettings();
                    AppDomain.CurrentDomain.SetData("OpenWithSettings.Session", Session);
                }
            }

            Query = new OpenWithSettings();

            AllSettings = new OpenWithSettings[] { Query, Session, Global };

            Util.Cleanup += (sender, args) =>
            {
                Query = new OpenWithSettings();
                // TODO? Use observable collections to rebuild flat collections on demand?
                AllSettings = new OpenWithSettings[] { Query, Session, Global };
            };

            Global.AllFilesOptions.Add(OpenWithOption.CopyPathToClipboard);
            Global.AllFilesOptions.Add(OpenWithOption.CopyLineNumberToClipboard);
            Global.AllFilesOptions.Add(OpenWithOption.SimpleShellOption);

            Global.AllFoldersOptions.Add(OpenWithOption.CopyPathToClipboard);
            Global.AllFoldersOptions.Add(OpenWithOption.SimpleShellOption);

            if (!IsCurrentProcessRunningAsAdmin)
            {
                Global.AllFilesOptions.Add(OpenWithOption.ShellAsAdminOption);
            }

            // configure default settings here, generally only add to Global.
            // Session (running the same query multiple times without resetting)
            // and Query (dumped data from the most recent execution) 
            // should be configured elsewhere (run once per session or query)
            // - these are primarily used by the OpenWithBox.Show() modal dialog command

            Global.AllFilesOptions.Add(new OpenWithOption("Edit with &Notepad++", @"C:\Program Files\Notepad++\notepad++.exe",
                "\"{0}\"", "-n{1} \"{0}\""));

            Global.AllFilesOptions.Add(new OpenWithOption("Check out in &TFS", (fullPath, lineNumber) => fullPath.CheckOutInTFS()));

            // TODO: Move "Existing Visual Studio" logic from separate executable into this code
            // and improve (instance selection, open new instance, etc.)
            Global.AllFilesOptions.Add(new OpenWithOption("Open in Existing &Visual Studio", @"C:\Development\Tools\VisualStudioFileOpenTool.exe",
                fileFormatArguments: "17 \"{0}\" 1",
                lineFormatArguments: "17 \"{0}\" {1}"));//, requiresLineNumber: true));

            Global.AddOptionForExtension(".sln", new OpenWithOption("Open in Visual Studio",
                @"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\devenv.exe",
                "\"{0}\""));

            if (!IsCurrentProcessRunningAsAdmin)
            {
                Global.AddOptionForExtension(".sln", new OpenWithOption("Open in Visual Studio (As Admin)",
                @"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\devenv.exe",
                "\"{0}\"", requiresAdmin: true));
            }

            var restorePackages = new OpenWithOption("Hard Clean and MSBuild /m /t:Restore", (fullPath, lineNumber) =>
            {
                var dir = Path.GetDirectoryName(fullPath);
                Directory.EnumerateDirectories(dir, "bin", SearchOption.AllDirectories).Concat(Directory.EnumerateDirectories(dir, "obj", SearchOption.AllDirectories)).ForEach(d => Directory.Delete(d, true));
                ProcessWrapper.DumpOutput(@"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\bin\MSBuild.exe", "\"" + fullPath + "\" /m /t:Restore");
            });

            var build = new OpenWithOption("MSBuild /m /t:Build", (fullPath, lineNumber) =>
            {
                ProcessWrapper.DumpOutput(@"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\bin\MSBuild.exe", "\"" + fullPath + "\" /m /t:Build");
            });

            var cleanRestoreBuild = new OpenWithOption("Hard Clean and MSBuild /m /t:Restore;Build", (fullPath, lineNumber) =>
            {
                var dir = Path.GetDirectoryName(fullPath);
                Directory.EnumerateDirectories(dir, "bin", SearchOption.AllDirectories).Concat(Directory.EnumerateDirectories(dir, "obj", SearchOption.AllDirectories)).ForEach(d => Directory.Delete(d, true));
                ProcessWrapper.DumpOutput(@"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\bin\MSBuild.exe", "\"" + fullPath + "\" /m /t:Restore;Build");
            });

            Global.AddOptionForExtension(".sln", restorePackages);
            Global.AddOptionForExtension(".csproj", restorePackages);
            Global.AddOptionForExtension(".wixproj", restorePackages);
            Global.AddOptionForExtension(".sln", build);
            Global.AddOptionForExtension(".csproj", build);
            Global.AddOptionForExtension(".wixproj", build);
            Global.AddOptionForExtension(".sln", cleanRestoreBuild);
            Global.AddOptionForExtension(".csproj", cleanRestoreBuild);
            Global.AddOptionForExtension(".wixproj", cleanRestoreBuild);
            //C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\bin\MSBuild.exe

            var openNearestSolutionOption = new OpenWithOption("Open Nearest Parent Solution", (fullPath, lineNumber) =>
            {
                string directory = fullPath;
                while ((directory = Path.GetDirectoryName(directory)) != null)
                {
                    var sln = Directory.EnumerateFiles(directory, "*.sln", SearchOption.TopDirectoryOnly).FirstOrDefault();
                    if (sln != null)
                    {
                        OpenWithBox.Show(sln);
                        break;
                    }
                }
            });


            Global.AddOptionForExtension(".cs", openNearestSolutionOption);
            Global.AddOptionForExtension(".wxs", openNearestSolutionOption);
            
            Global.AddOptionForExtension(".config", openNearestSolutionOption);
            
            Global.AddOptionForExtension(".csproj", openNearestSolutionOption);
            Global.AllFoldersOptions.Add(openNearestSolutionOption);

#if NET46
            // TODO: Use CodeMirror
            Global.AddOptionForExtension(".cs", new OpenWithOption("&Dump Highlighted Source", (file, line) => File.ReadAllText(file).DumpWithSyntaxHightlighting(SyntaxLanguageStyle.CSharp)));

            var dumpHighlightedXml = new OpenWithOption("&Dump Highlighted Source", (file, line) => File.ReadAllText(file).DumpWithSyntaxHightlighting(SyntaxLanguageStyle.XML));

            Global.AddOptionForExtension(".wxs", dumpHighlightedXml);
            Global.AddOptionForExtension(".xml", dumpHighlightedXml);
            Global.AddOptionForExtension(".config", dumpHighlightedXml);
#endif
        }

        public static IEnumerable<OpenWithOption> GetOptionsForPath(string fullPath, int? lineNumber, bool ignoreDefaults = false)
        {
            IEnumerable<OpenWithOption> ret;
            if (File.Exists(fullPath))
            {
                OpenWithOption defaultOption = null;
                if (!ignoreDefaults)
                {
                    defaultOption = GetDefaultOptionByExtension(Path.GetExtension(fullPath), lineNumber.HasValue);
                }

                ret = (defaultOption != null)
                    ? ((IEnumerable<OpenWithOption>)new OpenWithOption[] { defaultOption })
                    : new HashSet<OpenWithOption>(
                        GetAllFilesOptions().Concat(GetFileOptionsByExtension(Path.GetExtension(fullPath)))
                        .Where(o => lineNumber.HasValue || !o.RequiresLineNumber));
            }
            else if (Directory.Exists(fullPath))
            {
                OpenWithOption defaultOption = null;
                if (!ignoreDefaults)
                {
                    defaultOption = GetDefaultFolderOption();
                }

                ret = (defaultOption != null)
                    ? ((IEnumerable<OpenWithOption>)new OpenWithOption[] { defaultOption })
                    : new HashSet<OpenWithOption>(GetAllFoldersOptions());
            }
            else
            {
                throw new FileNotFoundException("Unable to Open Path", fullPath);
            }
            return ret;
        }

        private static Lazy<bool> isCurrentProcessRunningAsAdmin = new Lazy<bool>(() =>
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        });

        public static bool IsCurrentProcessRunningAsAdmin => isCurrentProcessRunningAsAdmin.Value;
    }
}
