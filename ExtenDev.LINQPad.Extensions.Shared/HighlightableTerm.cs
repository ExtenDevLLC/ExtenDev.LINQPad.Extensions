using System;
using System.Collections.Generic;
using System.Text;

namespace ExtenDev.LINQPad.Extensions
{
    // TODO: Add XML comments to all members
    public class HighlightableTerm
    {
        public HighlightableTerm(int termIndex, int startIndex, int length)
        {
            TermIndex = termIndex;
            StartIndex = startIndex;
            Length = length;
        }

        public int TermIndex { get; }
        public int StartIndex { get; }
        public int Length { get; }
    }
}
