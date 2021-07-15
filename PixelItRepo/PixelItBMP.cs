using System;
using System.Linq;

namespace PixelIT.web
{
    public class PixelItBMP
    {
        public int ID { get; set; }
        public DateTime? DateTime { get; set; }
        public string Name { get; set; }
        public string RGB565Array { get; set; }
        public bool Animated { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public int HitCount { get; set; }
    }
}
