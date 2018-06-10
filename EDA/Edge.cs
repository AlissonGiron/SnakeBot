using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDA
{
    public class Edge
    {
        public Node From { get; set; }
        public Node To { get; set; }
        public double Cost { get; set; }
    }
}
