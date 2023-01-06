using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using LINQPad;

namespace ExtenDev.LINQPad.Extensions.IO
{
    // TODO: Add XML comments to all members
    public static class ProcessWrapper
    {
        public static int DumpOutput(string fileName, string arguments, bool appendWhenUsingLocalContext = true)
        {
            $">{fileName} {arguments}".DumpLocal(append: appendWhenUsingLocalContext);

            var startInfo = new ProcessStartInfo(fileName, arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using (var process = new Process())
            {
                process.StartInfo = startInfo;

                process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null) args.Data.DumpLocal(true);
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null) Util.WithStyle(args.Data, "color: darkred").DumpLocal(true);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    Util.WithStyle($"Process Exited with Code {process.ExitCode}", "color: darkred").DumpLocal(true);
                }
                return process.ExitCode;
            }
        }
    }
}
