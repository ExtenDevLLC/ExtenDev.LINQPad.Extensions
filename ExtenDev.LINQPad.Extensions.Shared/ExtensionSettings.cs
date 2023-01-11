using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ExtenDev.LINQPad.Extensions.Development;

namespace ExtenDev.LINQPad.Extensions
{
    // TODO: Add XML comments to all members
    // TODO: Should settings be centralized in a single static class like this or separated out
    // TODO: Provide customization, persistence of settings, etc.
    public static class ExtensionSettings
    {
        static ExtensionSettings()
        {
            DevEnvPath = "DevEnv.exe";
            TeamFoundationClientExePath = "TF.exe";
            MSBuildPath = "MSBuild.exe";
            
            try
            {
                var vsPath = VisualStudioLocator.GetLatestInstallation();
                DevEnvPath = vsPath.ProductPath ?? "DevEnv.exe";

                var tfexePath = Path.Combine(vsPath.InstallationPath, @"Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\TF.exe");
                if (File.Exists(tfexePath)) TeamFoundationClientExePath = tfexePath;
            }
            catch { }

            try
            {
                var msBuildPath = MSBuildTools.GetMSBuildExePath();
                if (File.Exists(msBuildPath)) MSBuildPath = msBuildPath;
            }
            catch { }
        }

        public static bool CopyableStringDumping { get; set; } = false;

        public static string DevEnvPath { get; set; }

        public static string MSBuildPath { get; set; }

        public static bool AllowVisualStudioPreviewEditions { get; set; } = false;

        public static string TeamFoundationClientExePath { get; set; }

        public static string TemporaryFilesPath { get; set; } = @"C:\Temp\"; // TODO: move to ProgramData or user temp path?

    }
}
