using System.Threading.Tasks;
using LINQPad;
using LINQPad.Controls;

namespace ExtenDev.LINQPad.Extensions.UI
{
    // TODO: Add XML comments to all members
    public static class ClickToContinue
    {
        // TODO: Consider generic async options UI pattern
        public static Task DumpAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            var button = new Button("Click to Continue", btn =>
            {
                if (tcs.TrySetResult(true))
                {
                    btn.Visible = false;
                }
            }).Dump();

            return tcs.Task;
        }

        public static void Dump()
        {
            DumpAsync().Wait();
        }
    }
}
