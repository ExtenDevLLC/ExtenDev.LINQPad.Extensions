using System;
using System.Threading;
using LINQPad;

namespace ExtenDev.LINQPad.Extensions
{
    // TODO: Add XML comments to all members
    public class DumpContext : IDisposable
    {
        private static readonly AsyncLocal<DumpContext> current = new AsyncLocal<DumpContext>();

        private DumpContainer container;

        private DumpContext previous;

        public DumpContext(DumpContainer container)
        {
            this.container = container;
            previous = current.Value;
            current.Value = this;
        }

        public void Append(object? content)
        {
            if (container.Content != null)
            {
                container.Content = Util.VerticalRun(container.Content, content);
            }
            else
            {
                container.Content = content;
            }
        }

        public void Update(object? content)
        {
            container.Content = content;
        }

        public static DumpContext? Current => current.Value;

        public void Dispose()
        {
            current.Value = previous;
        }
    }
}
