using ImageMagick;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace PixelIT.web
{
    public class PixelTools
    {
        private readonly PixelItRepo pixelRepo;

        public PixelTools()
        {
            using MySqlConnection con = new(Globe.ConnectionString);
            pixelRepo = new PixelItRepo(con);
        }

        public void CheckAndCreateThumb()
        {
            var bmps = pixelRepo.GetBMPAll();

            foreach (var x in bmps)
            {
                if (!File.Exists($"wwwroot/images/{x.ID}.png") && !File.Exists($"wwwroot/images/{x.ID}.gif"))
                {
                    x.RGB565Array = x.RGB565Array.Replace(" ", "");
                    var converter = new HtmlConverter();
                    // Nicht animiert!
                    if (x.RGB565Array.Count(y => y == '[') == 1)
                    {
                        string html = CreateHTMLForImage(x.RGB565Array, x.SizeX, x.SizeY);
                        byte[] bytes = Array.Empty<byte>();

                        if (x.SizeX == 8)
                        {
                            bytes = converter.FromHtmlString(html, width: 105, croph: 89, cropw: 89, cropx: 8, cropy: 8, format: ImageFormat.Png, quality: 1);
                        }
                        else if (x.SizeX == 32)
                        {
                            bytes = converter.FromHtmlString(html, width: 315, croph: 89, cropw: 300, cropx: 8, cropy: 8, format: ImageFormat.Png, quality: 1);
                        }

                        File.WriteAllBytes($"wwwroot/images/{x.ID}.png", bytes);

                        Log.Information("{BMP} erstellt", $"{x.ID}.png");
                    }
                    else
                    {

                        using MagickImageCollection collection = new();
                        string[] frames = x.RGB565Array.Split(new string[] { "],[" }, StringSplitOptions.None);
                        var counter = 0;
                        foreach (var frame in frames)
                        {
                            var html = CreateHTMLForImage(frame, x.SizeX, x.SizeY);
                            byte[] bytes = Array.Empty<byte>();

                            if (x.SizeX == 8)
                            {
                                bytes = converter.FromHtmlString(html, width: 105, croph: 89, cropw: 89, cropx: 8, cropy: 8, format: ImageFormat.Png, quality: 1);
                            }
                            else if (x.SizeX == 32)
                            {
                                bytes = converter.FromHtmlString(html, width: 315, croph: 89, cropw: 300, cropx: 8, cropy: 8, format: ImageFormat.Png, quality: 1);
                            }

                            File.WriteAllBytes($"wwwroot/images/{x.ID}_{counter}.png", bytes);

                            // Add To GIF Maker
                            collection.Add($"wwwroot/images/{x.ID}_{counter}.png");
                            if (frames.Length > 3)
                            {
                                collection[counter].AnimationDelay = 20;
                            }
                            else
                            {
                                collection[counter].AnimationDelay = 40;
                            }
                            counter++;
                        }

                        // Optionally reduce colors
                        QuantizeSettings settings = new()
                        {
                            Colors = 256
                        };
                        collection.Quantize(settings);

                        // Optionally optimize the images (images should have the same size).
                        collection.Optimize();

                        // Save gif
                        collection.Write($"wwwroot/images/{x.ID}.gif");

                        // Old PNG´s
                        while (counter > 0)
                        {
                            File.Delete($"wwwroot/images/{x.ID}_{counter}.png");
                            counter--;
                        }

                        // Read image that needs a watermark
                        using (MagickImage image = new($"wwwroot/images/{x.ID}_0.png"))
                        {
                            // Read the watermark that will be put on top of the image
                            using (MagickImage watermark = new("wwwroot/lib/play.png"))
                            {
                                // Draw the watermark in the bottom right corner
                                image.Composite(watermark, Gravity.Center, CompositeOperator.Over);

                                // Optionally make the watermark more transparent
                                // watermark.Evaluate(Channels.All, EvaluateOperator.Divide, 1);

                                // Or draw the watermark at a specific location
                                // image.Composite(watermark, 200, 50, CompositeOperator.Over);
                            }

                            // Save the result
                            image.Write($"wwwroot/images/{x.ID}.png");
                        }

                        File.Delete($"wwwroot/images/{x.ID}_0.png");

                        Log.Information("{BMP} erstellt", $"{x.ID}.gif");
                    }
                }
            }
        }
        public string CreateHTMLForImage(string rgb565ArrayString, int x, int y)
        {
            rgb565ArrayString = rgb565ArrayString.Replace("[", "").Replace("]", "");

            string[] rgb565Array = rgb565ArrayString.Split(',');

            StringBuilder hmtlResult = new();

            hmtlResult.AppendLine("<html><head><style>#bmp{display: table;border-spacing: 1px;background-color: rgb(194, 193, 193);border: 1px;} .trow {display: table-row;}.pixel {display: table-cell;background-color: black;width: 10px;height: 10px;}</style></head><body><div id='bmp'>");

            int counter = 0;
            int trowCounter = 0;
            while (trowCounter < y)
            {
                hmtlResult.AppendLine("<div class='trow'>");

                var pixelCounter = 0;
                while (pixelCounter < x)
                {
                    var rgbColor = RGB565IntToRGB(int.Parse(rgb565Array[counter]));

                    hmtlResult.AppendLine($"<div class='pixel' style='background-color: rgb({rgbColor[0]}, {rgbColor[1]}, {rgbColor[2]});'></div>");
                    counter++;
                    pixelCounter++;
                }

                hmtlResult.AppendLine("</div>");

                trowCounter++;
            }


            hmtlResult.AppendLine("</div></body></html>");

            return hmtlResult.ToString();
        }

        public List<int> RGB565IntToRGB(int color)
        {
            var r = ((((color >> 11) & 0x1F) * 527) + 23) >> 6;
            var g = ((((color >> 5) & 0x3F) * 259) + 33) >> 6;
            var b = (((color & 0x1F) * 527) + 23) >> 6;
            return new List<int> { r, g, b };
        }
    }

    public static class OperatingSystem
    {
        public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsMacOS() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static void RegisterInStartup(string appname, string path, bool aktivate)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (aktivate)
            {
                registryKey.SetValue(appname, path);
            }
            else
            {
                registryKey.DeleteValue(appname);
            }
        }
    }

    public static class ConfigMapping
    {
        public static T DictionaryToObject<T>(IDictionary<string, string> dict) where T : new()
        {
            var t = new T();
            PropertyInfo[] properties = t.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                KeyValuePair<string, string> item = dict.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));

                // Find which property type (int, string, double? etc) the CURRENT property is...
                Type tPropertyType = t.GetType().GetProperty(property.Name).PropertyType;

                // Fix nullables...
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;

                // ...and change the type
                object newA = Convert.ChangeType(item.Value, newT);
                t.GetType().GetProperty(property.Name).SetValue(t, newA, null);
            }
            return t;
        }

        public static object DictionaryToObject<T>(object p)
        {
            throw new NotImplementedException();
        }
    }

    public class App
    {
        /// <summary>
        /// Globaler Application-Name
        /// </summary>
        public static string ApplicationName { get; set; }

        /// <summary>
        /// GUID für die laufende Instanz, wird bei jedem Programmstart neu generiert
        /// </summary>
        public static string InstanceGUID { get; set; }
        public static object GlobalExceptionHandler { get; private set; }

        public static void InitApplication(ApplicationStartupParameter startupOptions)
        {
            if (String.IsNullOrEmpty(startupOptions.ApplicationName))
            {
                throw new Exception(String.Format("ApplicationName darf nicht leer sein!"));
            }

            // Application-Name und InstanceGUID setzen
            ApplicationName = startupOptions.ApplicationName;
            InstanceGUID = Guid.NewGuid().ToString();

            // Serilog 
            LoggerConfiguration serilogConfig = new();
            serilogConfig = serilogConfig.Enrich.With<LogEnricherBase>();
            serilogConfig = serilogConfig.Enrich.FromLogContext();

            // an Seq senden ?    
            if (startupOptions.SerilogConfiguration.WriteToSeq == true)
            {
                serilogConfig = serilogConfig.WriteTo.Seq(startupOptions.SerilogConfiguration.SeqServer, apiKey: startupOptions.SerilogConfiguration.SeqAPIKey);
            }

            // und finale Instanz setzen
            Log.Logger = serilogConfig.CreateLogger();

            // Cleanup-Code aktivieren... der Code wird nicht immer aufgerufen, z.B. bei Consolen-Anwendung wenn man Ctrl-C drückt
            // oder bei einer Webanwendung, wo die Instanz runtergefahren wird... 
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                App.ShutdownApplication();
            };
        }

        /// <summary>
        /// Beendet die SIBS-Application
        /// </summary>
        public static void ShutdownApplication()
        {
            Log.Information("ShutdownApplication");
            // Serilog-Buffer flushen
            Log.CloseAndFlush();
        }
    }

    public class ApplicationStartupParameter
    {
        public ApplicationStartupParameter()
        {
            // Default setzen
            EnableGlobalExceptionHandler = true;
            SerilogConfiguration = new SerilogConfiguration();
        }

        /// <summary>
        /// Configuration für Serilog/Seq
        /// </summary>
        public SerilogConfiguration SerilogConfiguration { get; set; }

        /// <summary>
        /// Name der Applikation... darf nicht leer sein
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// soll ein globaler ExceptionHandler installiert werden ? Per Default ist dieser aktiv (außer, wenn ein Debugger verbunden ist)
        /// </summary>
        public bool EnableGlobalExceptionHandler { get; set; }
    }

    public class SerilogConfiguration
    {

        public SerilogConfiguration()
        {
            // Default setzen
            WriteToSeq = true;
            WriteRollingFile = true;
            WriteRollingFilePath = "/app/logs/log-{Date}.txt";
            WriteToColoredConsole = true;
            SeqServer = "http://seq:5341";
        }

        /// <summary>
        /// soll an Seq das Log gesendet werden ?
        /// </summary>
        public bool WriteToSeq { get; set; }

        /// <summary>
        /// soll an RollingFile das Log gesendet werden ?
        /// </summary>
        public bool WriteRollingFile { get; set; }

        /// <summary>
        /// soll an RollingFile das Log gesendet werden ?
        /// </summary>
        public string WriteRollingFilePath { get; set; }

        /// <summary>
        /// soll an ColoredConsole das Log gesendet werden ?
        /// </summary>
        public bool WriteToColoredConsole { get; set; }

        /// <summary>
        /// der API-Key für Seq
        /// </summary>
        public string SeqAPIKey { get; set; }
        /// <summary>
        /// Connection-String für Seq
        /// </summary>
        public string SeqServer { get; set; }

    }
}