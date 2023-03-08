using System;
using System.IO;

namespace PixelIT.web
{
    public class Data
    {
        /// <summary>
        /// Outputs the size in the optimal conversion.
        /// </summary>
        /// <param name="bytes">File size in byte</param>
        /// <returns>String with unit e.g. "100.00 MB".</returns>
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
        /// Returns the size of a folder.
        /// </summary>
        /// <param name="path">Path to folder</param>
        /// <param name="includeSubDirectories">Should the subfolders be included?</param>
        /// <returns>Folder size in bytes</returns>
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
