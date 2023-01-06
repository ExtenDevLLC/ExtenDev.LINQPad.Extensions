using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ExtenDev.LINQPad.Extensions.UI;
using LINQPad;

namespace ExtenDev.LINQPad.Extensions.IO
{
    // TODO: Add XML comments to all members
    public class MatchedPath
    {
        public MatchedPath(string fullPath, string searchRootDir = null, string branch = null)
        {
            FullPath = fullPath;

            if (searchRootDir != null)
            {
                if (branch != null)
                {
                    int tfsIdx = searchRootDir.Length + (searchRootDir[searchRootDir.Length - 1] == Path.DirectorySeparatorChar ? 0 : 1);
                    TfsProject = fullPath.Substring(tfsIdx, fullPath.IndexOf(branch) - tfsIdx - 1);

                    RelativeBasePath = Path.Combine(searchRootDir, TfsProject, branch);
                }
                else
                {
                    RelativeBasePath = searchRootDir;
                }
            }
        }

        public string FullPath { get; }
        public string RelativeBasePath { get; }
        
        // TODO: make this VCS-agnostic
        public string TfsProject { get; }

        public string RelativePath => RelativeBasePath == FullPath
            ? ""
            : FullPath.Substring(RelativeBasePath.Length + (RelativeBasePath[RelativeBasePath.Length - 1] == Path.DirectorySeparatorChar ? 0 : 1));

        public void GetLatestFromTfs(bool keepOpen = true)
        {
            ProcessWrapper.DumpOutput("TF.exe", $@"get ""{FullPath}"" /recursive");
            //		ProcessStartInfo proc = new ProcessStartInfo()
            //		{
            //			FileName = @"cmd",
            //			Arguments = $@"/c echo Getting Latest For ""{FullPath}"" & TF.exe get ""{FullPath}"" /recursive" + (keepOpen ? " & pause" : ""),
            //			WorkingDirectory = FullPath
            //		};
            //		var p = Process.Start(proc);
            //		p.WaitForExit();
        }

        object ToDump() => Util.HorizontalRun(true,
            (TfsProject != null ? (object)new Hyperlinq(() => OpenWithBox.Show(RelativeBasePath), TfsProject) : ""),
            (RelativePath != "" ? (object)new Hyperlinq(() => OpenWithBox.Show(FullPath), RelativePath) : "")
        );
    }
}
