using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LINQPad.ObjectModel;

namespace ExtenDev.LINQPad.Extensions
{
    public static class LINQPadTypeExtensions
    {
        // LINQPad 5 Backfills
#if NET472
        private static string GetLINQPadNuGetFolder()
        {
            return Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(typeof(StringExtensions).Assembly.Location)))));
        }

        public static string[] GetPackageFolders(this NuGetReference nuGetReference)
        {
            var linqNuGet = GetLINQPadNuGetFolder();

            var rootPackagePath = nuGetReference.LockedToVersion == null
                ? Path.Combine(linqNuGet, nuGetReference.PackageID)
                : Path.Combine(linqNuGet, $"{nuGetReference.PackageID} {nuGetReference.LockedToVersion}");

            return Directory.GetDirectories(rootPackagePath);
        }
#endif
    }
}
