using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Enums;

namespace EDA
{
    public class Node
    {
        public object Info { get; set; }
        public List<Edge> Edges { get; set; }
        public bool Visited { get; set; }
        public Node Parent { get; set; }
        public NodeType NodeType { get; set; }
        public bool InPath { get; set; }

        public Node()
        {
            Edges = new List<Edge>();
        }

        public void AddEdge(Edge AEdge) => Edges.Add(AEdge);
        public void RemoveEdge(Edge AEdge) => Edges.Remove(AEdge);
    }
}
