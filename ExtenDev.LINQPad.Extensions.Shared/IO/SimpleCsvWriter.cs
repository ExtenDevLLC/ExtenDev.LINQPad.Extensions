using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ExtenDev.LINQPad.Extensions.IO
{
    // TODO: Add XML comments to all members
    public sealed class SimpleCsvWriter : IDisposable
    {

        #region Fields

        private char delimiter;
        private char columnEscape;
        private string escapeStr;
        private string doubleEscape;
        private bool keepOpen;
        private TextWriter writer;

        #endregion

        #region Constructors

        public SimpleCsvWriter(string filename, char delimiter = ',', char columnEscape = '\"')
            : this(File.CreateText(filename), keepOpen: false, delimiter: delimiter, columnEscape: columnEscape) { }

        public SimpleCsvWriter(Stream stream, bool keepOpen = false, char delimiter = ',', char columnEscape = '\"')
            : this(new StreamWriter(stream), keepOpen: keepOpen, delimiter: delimiter, columnEscape: columnEscape) { }

        public SimpleCsvWriter(TextWriter writer, bool keepOpen = false, char delimiter = ',', char columnEscape = '\"')
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));

            this.writer = writer;
            this.keepOpen = keepOpen;
            this.delimiter = delimiter;
            this.columnEscape = columnEscape;
            this.escapeStr = columnEscape.ToString();
            this.doubleEscape = $"{columnEscape}{columnEscape}";
        }

        #endregion

        #region Methods

        public void WriteRow(params string[] columns)
        {
            WriteRow((IEnumerable<string>)columns);
        }

        public void WriteRow(IEnumerable<string> columns)
        {
            bool lineStarted = false;
            foreach (var c in columns)
            {
                if (lineStarted) writer.Write(delimiter);

                if (c.Contains(delimiter))
                {
                    writer.Write(columnEscape);
                    if (c.Contains(columnEscape))
                    {
                        writer.Write(c.Replace(escapeStr, doubleEscape));
                    }
                    else
                    {
                        writer.Write(c);
                    }
                    writer.Write(columnEscape);
                }
                else
                {
                    writer.Write(c);
                }

                lineStarted = true;
            }
            writer.WriteLine();
        }

        #endregion

        #region IDisposible Implementation

        public void Dispose()
        {
            if (writer == null) throw new ObjectDisposedException(nameof(SimpleCsvWriter));

            if (!keepOpen)
            {
                this.writer.Dispose();
            }
            this.writer = null!;
        }

        #endregion
    }
}
