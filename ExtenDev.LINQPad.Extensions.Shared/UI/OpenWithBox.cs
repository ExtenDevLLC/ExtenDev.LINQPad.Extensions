using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ExtenDev.LINQPad.Extensions.UI
{
    // TODO: Add XML comments to all members
    public class OpenWithBox : Form
    {
        // TODO: Consider using ModalDailogStack instead?

        private OpenWithBox(string fullPath, int? lineNumber)
        {
            FullPath = fullPath;
            LineNumber = lineNumber;

            InitializeComponent();
        }

        protected virtual void InitializeComponent()
        {
            var options = OpenWithSettings.GetOptionsForPath(FullPath, LineNumber, ignoreDefaults: Control.ModifierKeys.HasFlag(Keys.Alt))
                .ToList();

            if (options.Count == 1)
            {
                SelectedOption = options[0];
                return;
            }

            var flow = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                //AutoSize = true
            };
            int row = 0;

            Label adminLabel = null;
            if (OpenWithSettings.IsCurrentProcessRunningAsAdmin)
            {
                adminLabel = new Label()
                {
                    Text = "Warning! All Child Processes Will Launch in Administrative Mode!",
                    ForeColor = Color.Red,
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    MaximumSize = new Size(this.ClientRectangle.Width, 0)
                };
                flow.Controls.Add(adminLabel);
                flow.RowStyles.Add(new RowStyle()
                {
                    SizeType = SizeType.AutoSize
                });
                flow.SetRow(adminLabel, row++);
            }

            var pathLabel = new Label()
            {
                Text = $"Open \"{FullPath}\" With:",
                Dock = DockStyle.Fill,
                AutoSize = true,
                MaximumSize = new Size(this.ClientRectangle.Width, 0)
            };

            flow.Controls.Add(pathLabel);
            flow.RowStyles.Add(new RowStyle()
            {
                SizeType = SizeType.AutoSize
            });
            flow.SetRow(pathLabel, row++);

            int items = 0;

            foreach (var option in options)
            {
                Button b = new Button()
                {
                    Text = option.Caption,
                    Tag = option,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(5, 2, 5, 2)
                };
                b.Click += OptionClicked;
                flow.Controls.Add(b);
                flow.RowStyles.Add(new RowStyle()
                {
                    SizeType = SizeType.Percent,
                    Height = 1.0f / ((float)options.Count)
                });
                flow.SetRow(b, row++);
                items++;
            }

            var useDefault = new CheckBox()
            {
                AutoSize = true,
                Margin = new Padding(5, 2, 5, 2),
                MaximumSize = new Size(this.ClientRectangle.Width, 0)
            };

            if (File.Exists(FullPath))
            {
                var extension = Path.GetExtension(FullPath);
                if (string.IsNullOrEmpty(extension))
                {
                    extension = "files with no extension";
                }
                else
                {
                    extension = "'" + extension + "' files";
                }

                useDefault.Text = "Use as &Default for "
                        + (LineNumber.HasValue ? "Line Numbers of " : "")
                        + extension;
            }
            else
            {
                useDefault.Text = "Use as &Default for Folders";
            }

            flow.Controls.Add(useDefault);
            flow.RowStyles.Add(new RowStyle()
            {
                SizeType = SizeType.AutoSize
            });
            flow.SetRow(useDefault, row++);

            QueryDefault = true;

            var queryDefault = new RadioButton()
            {
                AutoSize = true,
                Margin = new Padding(15, 2, 5, 2),
                MaximumSize = new Size(this.ClientRectangle.Width, 0),
                Text = "For This Query",
                Checked = true,
                Enabled = false
            };

            flow.Controls.Add(queryDefault);
            flow.RowStyles.Add(new RowStyle()
            {
                SizeType = SizeType.AutoSize
            });
            flow.SetRow(queryDefault, row++);

            var sessionDefault = new RadioButton()
            {
                AutoSize = true,
                Margin = new Padding(15, 2, 5, 2),
                MaximumSize = new Size(this.ClientRectangle.Width, 0),
                Text = "For This Session",
                Checked = false,
                Enabled = false
            };

            flow.Controls.Add(sessionDefault);
            flow.RowStyles.Add(new RowStyle()
            {
                SizeType = SizeType.AutoSize
            });
            flow.SetRow(sessionDefault, row++);

            queryDefault.CheckedChanged += (sender, args) => QueryDefault = queryDefault.Checked;
            useDefault.CheckedChanged += (sender, args) => queryDefault.Enabled = sessionDefault.Enabled = UseAsDefault = useDefault.Checked;

            this.Resize += (sender, args) =>
            {
                var maxWidth = new Size(this.ClientRectangle.Width, 0);
                if (adminLabel != null) adminLabel.MaximumSize = maxWidth;
                pathLabel.MaximumSize = maxWidth;
                useDefault.MaximumSize = maxWidth;
                queryDefault.MaximumSize = maxWidth;
                sessionDefault.MaximumSize = maxWidth;
                flow.MaximumSize = this.ClientRectangle.Size;
            };

            this.Controls.Add(flow);

            this.Height = Math.Min((Screen.AllScreens.Where(s => s.Bounds.Contains(this.Left, this.Top)).FirstOrDefault() ?? Screen.PrimaryScreen).WorkingArea.Height, row * 50);
            this.Width = 480;
        }

        private void OptionClicked(object sender, EventArgs args)
        {
            this.SelectedOption = ((OpenWithOption)((Button)sender).Tag);
            if (UseAsDefault)
            {
                var setting = QueryDefault ? OpenWithSettings.Query : OpenWithSettings.Session;
                if (File.Exists(FullPath))
                {
                    var dict = LineNumber.HasValue ? setting.DefaultOptionByExtensionWithLineNumber : setting.DefaultOptionByExtension;
                    setting.DefaultOptionByExtension[Path.GetExtension(FullPath)] = this.SelectedOption;
                }
                else
                {
                    setting.AllFoldersDefaultOption = this.SelectedOption;
                }
            }
            this.Close();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (SelectedOption != null)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        public bool UseAsDefault { get; private set; }

        public bool QueryDefault { get; private set; }

        public string FullPath { get; }

        public int? LineNumber { get; }

        public OpenWithOption SelectedOption { get; private set; }

        // Named for MessageBox.Show because this pattern is more similar to that than OpenFileDialog.ShowDialog
        public static void Show(string fullPath, int? lineNumber = null)
        {
            if (!File.Exists(fullPath) && !Directory.Exists(fullPath))
            {
                throw new FileNotFoundException("Unable to Open Path", fullPath);
            }

            var dlg = new OpenWithBox(fullPath, lineNumber);
            dlg.ShowDialog(LINQPadOwnerWindow.Instance);
            dlg.SelectedOption?.Open(fullPath, lineNumber);
        }
    }
}
