using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Enums;

namespace Models
{
    public class PathNode
    {
        public object currNode { get; set; }
        public object nextNode { get; set; }
        public Direction nextNodeDir { get; set; }
    }
}
