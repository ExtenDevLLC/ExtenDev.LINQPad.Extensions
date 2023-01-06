using System;
using LINQPad;

namespace ExtenDev.LINQPad.Extensions
{
    // TODO: Add XML comments to all members
    public static class DumpContextExtensions
    {
        // TODO: this crashes after too many items are dumped to it. Perhaps use approache(s) here: https://stackoverflow.com/questions/61896122/how-to-add-not-replace-the-content-of-a-dumpcontainer-in-linqpad
        public static T DumpLocal<T>(this T? item, bool append = true)
        {
            var context = DumpContext.Current;
            if (context != null)
            {
                if (append)
                {
                    context.Append(item);
                }
                else
                {
                    context.Update(item);
                }
            }
            else
            {
                item.Dump();
            }

            return item;
        }

        public static Hyperlinq CreateLocalHyperlinq(this DumpContainer container, Action action, string text)
        {
            return new Hyperlinq(() =>
            {
                using (new DumpContext(container))
                {
                    action();
                }
            }, text);
        }
    }
}
