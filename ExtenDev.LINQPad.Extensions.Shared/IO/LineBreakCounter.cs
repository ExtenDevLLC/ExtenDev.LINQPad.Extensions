using System;
using System.Collections.Generic;
using System.Text;

namespace ExtenDev.LINQPad.Extensions.IO
{
    public class LineBreakCounter
    {
        List<int> lineBreaks = new List<int>();
        int length;

        public LineBreakCounter(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            length = text.Length;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                    lineBreaks.Add(i);

                else if (text[i] == '\r' && i < text.Length - 1 && text[i + 1] == '\n')
                    lineBreaks.Add(++i);
            }
        }

        public int GetLine(int offset)
        {
            if (offset < 0 || offset > length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            var result = lineBreaks.BinarySearch(offset);
            if (result < 0)
                return ~result;
            else
                return result;
        }

        public int Lines => lineBreaks.Count + 1;

        public int GetOffset(int line)
        {
            if (line < 0 || line >= Lines)
                throw new ArgumentOutOfRangeException(nameof(line));

            if (line == 0)
                return 0;

            return lineBreaks[line - 1] + 1;
        }
    }
}
