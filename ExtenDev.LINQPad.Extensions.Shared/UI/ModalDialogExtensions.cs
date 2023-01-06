using LINQPad;

namespace ExtenDev.LINQPad.Extensions.UI
{
    // TODO: Add XML comments to all members
    public static class ModalDialogExtensions
    {
        private static ModalDialogStack container = new ModalDialogStack().Dump();

        public static T DumpModal<T>(this T? content, string title = "Modal Dialog")
        {
            container.PushDialog(title, content);
            return content;
        }
    }
}
