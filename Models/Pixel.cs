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
        public int Row { get; set; }
        public int Col { get; set; }

        public Pixel(Point pos, Color color, int row, int col)
        {
            Position = pos;
            Color = color;
            Visited = false;
            Row = row;
            Col = col;
        }

        public Pixel(Point pos, Color color) : this(pos, color, 0, 0)
        {
        }
    }
}
