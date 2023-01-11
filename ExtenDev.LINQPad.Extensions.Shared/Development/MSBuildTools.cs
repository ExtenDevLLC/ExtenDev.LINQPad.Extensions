using System.IO;
using System.Linq;
using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Locator;

namespace ExtenDev.LINQPad.Extensions.Development
{
    public static class MSBuildTools
    {

        public static string GetMSBuildDirectory(bool? includePreview = null)
        {
            return MSBuildLocator.QueryVisualStudioInstances()
                .OrderByDescending(vs => vs.Version)
                .First(vs => (includePreview ?? ExtensionSettings.AllowVisualStudioPreviewEditions) || !vs.MSBuildPath.Contains(@"Preview"))
                .MSBuildPath;
        }

        public static string GetMSBuildExePath(bool? includePreview = null)
        {
            return Path.Combine(GetMSBuildDirectory(includePreview: includePreview), "MSBuild.exe");
        }

        public static class ProjectTools
        {
            public static Project GetOrLoadProject(string fullPath, ProjectOptions projectOptions = null)
            {
                if (projectOptions == null) projectOptions = new ProjectOptions();
                var ret = ProjectCollection.GlobalProjectCollection.GetLoadedProjects(fullPath).SingleOrDefault();
                if (ret == null)
                {
                    ret = Project.FromFile(fullPath, projectOptions);
                }
                return ret;
            }
        }

    }
}
