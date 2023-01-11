using System.Threading.Tasks;
using LINQPad;
using LINQPad.Controls;

namespace ExtenDev.LINQPad.Extensions.UI
{
    public static class EasyMenu
    {

        public static Task<string> GetChoiceAsync(params string[] choices)
        {
            var tcs = new TaskCompletionSource<string>();

            var menu = new Table();
            menu.Styles["min-width"] = "200px;";

            foreach (var choice in choices)
            {
                if (string.IsNullOrEmpty(choice))
                {
                    menu.Rows.Add(new TableRow(new TableCell(new Literal("&nbsp;"))));
                }
                else
                {
                    var button = new Button(choice, _ =>
                    {
                        menu.Visible = false;
                        tcs.SetResult(choice);
                    });
                    button.Styles["min-width"] = "200px";
                    button.Styles["width"] = "100%";
                    menu.Rows.Add(new TableRow(new TableCell(button)));
                }
            }

            menu.Dump();

            return tcs.Task;
        }

    }
}
