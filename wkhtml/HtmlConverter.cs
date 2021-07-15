using System;
using System.Diagnostics;
using System.IO;

namespace PixelIT.web
{
    /// <summary>
    /// Html Converter. Converts HTML string and URLs to image bytes
    /// </summary>
    public class HtmlConverter
    {
        private static readonly string toolFilepath;

        static HtmlConverter()
        {
            if (OperatingSystem.IsWindows())
            {
                toolFilepath = Path.Combine("wkhtml", "wkhtmltoimage_win.exe");
            }
            else
            {
                toolFilepath = "wkhtmltoimage";
            }
        }

        /// <summary>
        /// Converts HTML string to image
        /// </summary>
        /// <param name="html">HTML string</param>
        /// <param name="width">Output document width</param>
        /// <param name="format">Output image format</param>
        /// <param name="quality">Output image quality 1-100</param>
        /// <returns></returns>
        public byte[] FromHtmlString(string html, int width, int croph, int cropw, int cropx, int cropy, ImageFormat format, int quality)
        {
            string filePath = Path.Combine("wwwroot", "temp", $"{Guid.NewGuid()}.html");
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }
            File.WriteAllText(filePath, html);
            var bytes = FromUrl(filePath, width, croph, cropw, cropx, cropy, format, quality);
            File.Delete(filePath);
            return bytes;
        }

        /// <summary>
        /// Converts HTML page to image
        /// </summary>
        /// <param name="url">Valid http(s):// URL</param>
        /// <param name="width">Output document width</param>
        /// <param name="format">Output image format</param>
        /// <param name="quality">Output image quality 1-100</param>
        /// <returns></returns>
        public byte[] FromUrl(string url, int width, int croph, int cropw, int cropx, int cropy, ImageFormat format, int quality)
        {
            var imageFormat = format.ToString().ToLower();
            var filename = Path.Combine("wwwroot", "temp", $"{Guid.NewGuid().ToString()}.{imageFormat}");

            var proc = new ProcessStartInfo(toolFilepath, $"--quality {quality} --width {width}  --crop-h {croph} --crop-w {cropw} --crop-x {cropx} --crop-y {cropy} -f {imageFormat} {url} {filename}");

            //var proc = new ProcessStartInfo("", $"--quality {quality} --width {width} -f {imageFormat} {url} {filename}");
            var process = Process.Start(proc);
            process.WaitForExit();


            if (File.Exists(filename))
            {
                var bytes = File.ReadAllBytes(filename);
                File.Delete(filename);
                return bytes;
            }

            throw new Exception("Something went wrong. Please check input parameters");
        }

        //private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        //{
        //    throw new Exception(e.Data);
        //}
    }
}
