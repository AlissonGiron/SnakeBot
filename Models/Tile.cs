using System.Collections.Generic;

namespace SnakeBOT
{
    public class Tile
    {
        public List<Pixel> Pixels { get; set; }

        public Tile()
        {
            Pixels = new List<Pixel>();
        }
    }
}
