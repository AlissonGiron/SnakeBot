using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeBOT
{
    public class Pixel
    {
        public Point Position { get; set; }
        public Color Color { get; set; }
        public bool Visited { get; set; }

        public Pixel(Point pos, Color color)
        {
            Position = pos;
            Color = color;
            Visited = false;
        }
    }
}
