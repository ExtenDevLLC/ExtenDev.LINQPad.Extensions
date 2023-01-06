using System;
using System.Collections.Generic;
using System.Text;
using ExtenDev.LINQPad.Extensions.UI;
using LINQPad;

namespace ExtenDev.LINQPad.Extensions.IO
{
    // TODO: Add XML comments to all members
    public class MatchedFileLine
    {
        public MatchedFileLine(MatchedFile parentFile, string line, int lineNumber)
        {
            ParentFile = parentFile;
            Line = line;
            LineNumber = lineNumber;
        }

        public MatchedFile ParentFile { get; }
        public string Line { get; }
        public int LineNumber { get; }

        public Func<string, object> LineDumpFormatter { get; set; } = s => s.Trim();

        object ToDump() => Util.HorizontalRun(true,
            new Hyperlinq(() => OpenWithBox.Show(ParentFile.FullPath, LineNumber), LineNumber.ToString()),
            LineDumpFormatter(Line));
    }
}
