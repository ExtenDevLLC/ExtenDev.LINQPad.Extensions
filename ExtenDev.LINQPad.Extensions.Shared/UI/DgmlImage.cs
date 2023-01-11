using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using LINQPad;

namespace ExtenDev.LINQPad.Extensions.UI
{
    public static class DgmlImage
    {
        private static readonly string DgmlImageExePath;

        static DgmlImage()
        {
            var dgmlImagePackagePath = Util.CurrentQuery.NuGetReferences
                .Where(ngr => ngr.PackageID == typeof(DgmlImage).Assembly.GetName().Name)
                .First()
                .GetPackageFolders()
                .Where(pkgPath => Path.GetFileName(pkgPath).StartsWith("DgmlImage"))
                .Single();

            DgmlImageExePath = Path.Combine(dgmlImagePackagePath, @"tools\DgmlImage.exe");
        }

        private static void ExecuteWithArguments(string arguments)
        {
            var startInfo = new ProcessStartInfo(DgmlImageExePath, arguments);
            startInfo.CreateNoWindow = true;

            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;

            Process.Start(startInfo).WaitForExit();
        }

        private static string EscapePath(string path) => path.Contains(' ') ? '"' + path + '"' : path;

        private static string GetFilesArgument(IEnumerable<string> files)
        {
            return files.Select(EscapePath).Join(" ");
        }

        public static void Generate(params string[] files)
        {
            Generate((IEnumerable<string>)files);
        }

        public static void Generate(string file, string outputDirectory = null, DgmlImageFormat? imageFormat = null, double? zoom = null, bool? showLegend = null, int? width = null)
        {
            Generate(new string[] { file }, outputDirectory: outputDirectory, imageFormat: imageFormat, zoom: zoom, showLegend: showLegend, width: width);
        }

        public static IEnumerable<object> GenerateDumpable(params string[] files)
        {
            return GenerateDumpable((IEnumerable<string>)files);
        }

        public static object GenerateDumpable(string file, string outputDirectory = null, DgmlImageFormat? imageFormat = null, double? zoom = null, bool? showLegend = null, int? width = null)
        {
            return GenerateDumpable(new string[] { file }, outputDirectory: outputDirectory, imageFormat: imageFormat, zoom: zoom, showLegend: showLegend, width: width).Single();
        }

        public static IEnumerable<object> GenerateDumpable(IEnumerable<string> files, string outputDirectory = null, DgmlImageFormat? imageFormat = null, double? zoom = null, bool? showLegend = null, int? width = null)
        {
            Generate(files, outputDirectory: outputDirectory, imageFormat: imageFormat, zoom: zoom, showLegend: showLegend, width: width);

            string imgExtension = imageFormat?.ToString()?.ToLowerInvariant() ?? "png";
            var filePaths = files.Select(f => Path.Combine(outputDirectory ?? Path.GetDirectoryName(f), Path.GetFileNameWithoutExtension(f) + $".{imgExtension}"));

            return filePaths.Select(outFile => Util.RawHtml($"<img src=\"{new Uri(outFile).AbsoluteUri}\" />"));

        }

        /*
        Usage: DgmlImage /format:png /zoom:level files...
        Converts given DGML documents to given image format
        Options:
            /format:name, supported formats are 'png', 'bmp', 'gif', 'tiff', 'jpg', 'xps', 'svg' (default png)
            /f:name, short hand for /format:name
            /zoom:level, default zoom is 1.
            /z:level, short hand for /zoom:level
            /width:n, width of the image (defaults to 100% of graph size)
            /legend, show the legend (default hidden)
            /out:directory, the directory in which to write the image files
        */

        public static void Generate(IEnumerable<string> files, string outputDirectory = null, DgmlImageFormat? imageFormat = null, double? zoom = null, bool? showLegend = null, int? width = null)
        {
            var notFound = files.FirstOrDefault(f => !File.Exists(f));
            if (notFound != null)
            {
                throw new FileNotFoundException(null, notFound);
            }

            var args = new StringBuilder();

            if (imageFormat != null)
            {
                args.Append("/format:").Append(imageFormat.Value.ToString().ToLowerInvariant()).Append(' ');
            }

            if (zoom != null)
            {
                args.Append("/zoom:").Append(zoom.Value.ToString(CultureInfo.InvariantCulture)).Append(' ');
            }

            if (showLegend != null && showLegend.Value)
            {
                args.Append("/legend ");
            }

            if (width != null)
            {
                args.Append("/width:").Append(width.Value.ToString(CultureInfo.InvariantCulture)).Append(' ');
            }

            if (outputDirectory != null)
            {
                if (!Directory.Exists(outputDirectory)) throw new DirectoryNotFoundException();
                args.Append("/out:").Append(EscapePath(outputDirectory)).Append(' ');
            }

            args.Append(GetFilesArgument(files));

            ExecuteWithArguments(args.ToString());
        }
    }
}
