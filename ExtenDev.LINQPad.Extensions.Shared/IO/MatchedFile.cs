using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LINQPad;
using System.Text.RegularExpressions;
using ExtenDev.LINQPad.Extensions.UI;

namespace ExtenDev.LINQPad.Extensions.IO
{
    // TODO: Add XML comments to all members
    public class MatchedFile
    {
        public MatchedFile(string fullPath, string searchRootDir = null, string branch = null)
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

        public string RelativePath => FullPath.Substring(RelativeBasePath.Length + (RelativeBasePath[RelativeBasePath.Length - 1] == Path.DirectorySeparatorChar ? 0 : 1));

        public string ReadAllText()
        {
            return File.ReadAllText(FullPath);
        }

        private IEnumerable<MatchedFileLine> EnumerateLinesWhereCore(Func<string, bool> predicate)
        {
            var allLines = File.ReadAllLines(FullPath);
            for (int x = 0; x < allLines.Length; x++)
            {
                var line = allLines[x];
                if (predicate(line))
                {
                    yield return new MatchedFileLine(this, line, x + 1);
                }
            }
        }

        public IEnumerable<MatchedFileLine> EnumerateLinesContainingAny(params string[] strings) => EnumerateLinesContainingAny((IEnumerable<string>)strings);

        public IEnumerable<MatchedFileLine> EnumerateLinesContainingAny(IEnumerable<string> strings)
        {
            var ret = EnumerateLinesWhereCore(l => strings.Any(s => l.Contains(s))).ToList();
            Func<string, object> formatter = l => l.Trim().HighlightStringMatches(strings);
            ret.ForEach(ml => ml.LineDumpFormatter = formatter);

            return ret;
        }

        public IEnumerable<MatchedFileLine> EnumerateLinesContainingAll(params string[] strings) => EnumerateLinesContainingAll((IEnumerable<string>)strings);

        public IEnumerable<MatchedFileLine> EnumerateLinesContainingAll(IEnumerable<string> strings)
        {
            var ret = EnumerateLinesWhereCore(l => strings.All(s => l.Contains(s))).ToList();
            Func<string, object> formatter = l => l.Trim().HighlightStringMatches(strings);
            ret.ForEach(ml => ml.LineDumpFormatter = formatter);

            return ret;
        }

        public IEnumerable<MatchedFileLine> EnumerateLinesMatchingAny(params string[] regexes)
        {
            var ret = EnumerateLinesWhereCore(l => regexes.Any(s => Regex.Match(l, s).Success)).ToList();
            Func<string, object> formatter = l => l.Trim().HighlightRegexMatches(regexes);
            ret.ForEach(ml => ml.LineDumpFormatter = formatter);

            return ret;
        }

        public IEnumerable<MatchedFileLine> EnumerateLinesMatchingAny(params Regex[] regexes)
        {
            var ret = EnumerateLinesWhereCore(l => regexes.Any(r => r.IsMatch(l))).ToList();
            Func<string, object> formatter = l => l.Trim().HighlightRegexMatches(regexes);
            ret.ForEach(ml => ml.LineDumpFormatter = formatter);

            return ret;
        }

        public IEnumerable<MatchedFileLine> EnumerateLinesMatchingAll(params string[] regexes)
        {
            var ret = EnumerateLinesWhereCore(l => regexes.All(s => Regex.Match(l, s).Success)).ToList();
            Func<string, object> formatter = l => l.Trim().HighlightRegexMatches(regexes);
            ret.ForEach(ml => ml.LineDumpFormatter = formatter);

            return ret;
        }

        public IEnumerable<MatchedFileLine> EnumerateLinesMatchingAll(params Regex[] regexes)
        {
            var ret = EnumerateLinesWhereCore(l => regexes.All(r => r.IsMatch(l))).ToList();
            Func<string, object> formatter = l => l.Trim().HighlightRegexMatches(regexes);
            ret.ForEach(ml => ml.LineDumpFormatter = formatter);

            return ret;
        }

        public IEnumerable<MatchedFileLine> EnumerateLinesWhere(Func<string, bool> predicate)
        {
            // forces evaluation to get all the extra lines out of memory
            return EnumerateLinesWhereCore(predicate).ToList();
        }

        protected virtual object ToDump() => Util.HorizontalRun(true,
            (TfsProject != null ? (object)new Hyperlinq(() => OpenWithBox.Show(RelativeBasePath), TfsProject) : ""),
            new Hyperlinq(() => OpenWithBox.Show(FullPath), RelativePath)
        );
    }
}
