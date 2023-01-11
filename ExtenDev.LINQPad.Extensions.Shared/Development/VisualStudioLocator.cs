using System.IO;
using System.Linq;
using System.Text;
using LINQPad;
using Newtonsoft.Json;

namespace ExtenDev.LINQPad.Extensions.Development
{
    // TODO: Add support for better querying
    // TODO: Consider migrating to use MSBuildLocator? Gives less information overall, but includes MSBuild path where this does not
    public static class VisualStudioLocator
    {

        private static string GetVsWherePath()
        {
            var vswherePackagePath = Util.CurrentQuery.NuGetReferences
                .Where(ngr => ngr.PackageID == typeof(VisualStudioLocator).Assembly.GetName().Name)
                .First()
                .GetPackageFolders()
                .Where(pkgPath => Path.GetFileName(pkgPath).StartsWith("vswhere"))
                .Single();

            return Path.Combine(vswherePackagePath, @"tools\vswhere.exe");
        }

        private static VisualStudioInstallationSet GetRawInstallationSet(string args)
        {
            var vswhere = GetVsWherePath();
#if NET472
            return JsonConvert.DeserializeObject<VisualStudioInstallationSet>(Util.Cmd(vswhere, "-format json -utf8", true).Join(""))!;
#else
            return JsonConvert.DeserializeObject<VisualStudioInstallationSet>(Util.Cmd(vswhere, "-format json -utf8", Encoding.UTF8, quiet: true).Join(""))!;
#endif
        }

        public static VisualStudioInstallation GetLatestInstallation(bool? includePrerelease = null, bool includeIncomplete = false)
        {
            if (includePrerelease == null) includePrerelease = ExtensionSettings.AllowVisualStudioPreviewEditions;
            
            var args = "-format json -utf8 -latest";
            if (includePrerelease ?? false) args += " -prerelease";
            if (includeIncomplete) args += " -all";

            return GetRawInstallationSet(args).Single();
        }

        public static VisualStudioInstallationSet GetInstallations(bool? includePrerelease = null, bool includeIncomplete = false)
        {
            if (includePrerelease == null) includePrerelease = ExtensionSettings.AllowVisualStudioPreviewEditions;

            var args = "-format json -utf8";
            if (includePrerelease ?? false) args += " -prerelease";
            if (includeIncomplete) args += " -all";

            return GetRawInstallationSet(args);
        }

    }
}
