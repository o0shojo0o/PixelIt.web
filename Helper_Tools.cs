using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PixelIT.web
{ 
    public class Data
    {
        /// <summary>
        /// Gibt die Größe in der optimalen Umrechnung aus.
        /// </summary>
        /// <param name="bytes">Dateigrösse in Byte</param>
        /// <returns>String mit Einheit z.B. "100.00 MB"</returns>
        public static string ToFuzzyByteString(long bytes)
        {
            double s = bytes;
            string[] format = new string[]
                  {
                      "{0:0.00} bytes",
                      "{0:0.00} KB",
                      "{0:0.00} MB",
                      "{0:0.00} GB",
                      "{0:0.00} TB",
                      "{0:0.00} PB",
                      "{0:0.00} EB"
                  };

            int i = 0;

            while (i < format.Length && s >= 1024)
            {
                s = (long)(100 * s / 1024) / 100.0;
                i++;
            }
            return String.Format(format[i], s);
        }

        /// <summary>
        /// Gibt die Größe einen Ordners aus.
        /// </summary>
        /// <param name="path">Pfad zum Ordner</param>
        /// <param name="includeSubDirectories">Sollen die Unterordner mit einbezogen werden?</param>
        /// <returns>Ordnergrösse in Byte</returns>
        public static long GetDirectorySize(string path, bool includeSubDirectories)
        {
            long size = 0;
            if (includeSubDirectories)
            {
                try
                {
                    string[] subDirectories = Directory.GetDirectories(path);
                    foreach (string subDirectory in subDirectories)
                    {
                        size += GetDirectorySize(subDirectory, includeSubDirectories);
                    }
                }
                catch
                {
                    return -1;
                }
            }

            try
            {
                string[] fileNames = Directory.GetFiles(path);
                foreach (string fileName in fileNames)
                {
                    FileInfo fileInfo = new FileInfo(fileName);
                    size += fileInfo.Length;
                }
            }
            catch
            {
                return -1;
            }

            return size;
        }
    }
}
