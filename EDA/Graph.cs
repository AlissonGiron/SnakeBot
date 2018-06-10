using SnakeBOT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Enums;

namespace EDA
{
    public class Graph
    {
        public List<Node> Nodes { get; set; }

        public Graph()
        {
            Nodes = new List<Node>();
        }

        public void AddNode(Node ANode) => Nodes.Add(ANode);
        public void RemoveNode(Node ANode) => Nodes.Remove(ANode);

        public void CreateEdges()
        {
            Nodes.ForEach(node =>
            {
                Node Node1 = GetNextNode(node, Axis.X, Sentido.Maior);
                Node Node2 = GetNextNode(node, Axis.X, Sentido.Menor);
                Node Node3 = GetNextNode(node, Axis.Y, Sentido.Maior);
                Node Node4 = GetNextNode(node, Axis.Y, Sentido.Menor);

                if (Node1 != null) node.AddEdge(new Edge() { From = node, To = Node1, Cost = 0 });
                if (Node2 != null) node.AddEdge(new Edge() { From = node, To = Node2, Cost = 0 });
                if (Node3 != null) node.AddEdge(new Edge() { From = node, To = Node3, Cost = 0 });
                if (Node4 != null) node.AddEdge(new Edge() { From = node, To = Node4, Cost = 0 });
            });
        }

        private Node GetNextNode(Node ANode, Axis nextAxis, Sentido sentido)
        {
            List<Node> LNodes = new List<Node>();

            if (nextAxis == Axis.X)
            {
                LNodes = Nodes.Where(t => ((Pixel)t.Info).Position.Y == ((Pixel)ANode.Info).Position.Y).ToList();

                if (sentido == Sentido.Maior)
                {
                    LNodes = LNodes.Where(t => ((Pixel)t.Info).Position.X > ((Pixel)ANode.Info).Position.X)
                                   .OrderBy(t => ((Pixel)t.Info).Position.X).ToList();
                }
                else
                {
                    LNodes = LNodes.Where(t => ((Pixel)t.Info).Position.X < ((Pixel)ANode.Info).Position.X)
                                   .OrderByDescending(t => ((Pixel)t.Info).Position.X).ToList();
                }
            }
            else
            {
                LNodes = Nodes.Where(t => ((Pixel)t.Info).Position.X == ((Pixel)ANode.Info).Position.X).ToList();

                if (sentido == Sentido.Maior)
                {
                    LNodes = LNodes.Where(t => ((Pixel)t.Info).Position.Y > ((Pixel)ANode.Info).Position.Y)
                                   .OrderBy(t => ((Pixel)t.Info).Position.Y).ToList();
                }
                else
                {
                    LNodes = LNodes.Where(t => ((Pixel)t.Info).Position.Y < ((Pixel)ANode.Info).Position.Y)
                                   .OrderByDescending(t => ((Pixel)t.Info).Position.Y).ToList();
                }
            }

            return LNodes.FirstOrDefault();
        }


        public void SetTypes(Bitmap AImage)
        {
            Nodes.ForEach(node =>
            {
                node.NodeType = GetNodeType(node, AImage);
            });
        }

        public NodeType GetNodeType(Node ANode, Bitmap AImage)
        {
            int x = ((Pixel)ANode.Info).Position.X;
            int y = ((Pixel)ANode.Info).Position.Y;
            int num = 5;

            for (int i = x - num; i < x + num; i++)
            {
                for (int j = y - num; j < y + num; j++)
                {
                    Color LColor = AImage.GetPixel(x > 0 ? x : 0, y > 0 ? y : 0);

                    if (CompareColors(LColor, Color.FromArgb(255, 169, 208, 73), 50)) return NodeType.Chao;

                    if (CompareColors(LColor, Color.FromArgb(255, 232, 72, 29), 50)) return NodeType.Comida;

                    if (CompareColors(LColor, Color.FromArgb(255, 71, 117, 234), 50)) return NodeType.Snake;
                }
            }

            return NodeType.Chao;
        }

        public bool isSnakeHead(Bitmap AImage, Pixel APixel, int range = 10)
        {
            int x = APixel.Position.X;
            int y = APixel.Position.Y;

            List<Pixel> LPixels = new List<Pixel>();

            for (int i = x - range; i < x + range; i++)
            {
                for (int j = y - range; j < y + range; j++)
                {
                    if (AImage.GetPixel(i, j).Equals(Color.FromArgb(255, 255, 255, 255))) return true;
                }
            }

            return false;
        }

        public bool CompareColors(Color AColor, Color TargetColor, int ARange)
        {
            bool R = AColor.R > (TargetColor.R - ARange) && AColor.R < (TargetColor.R + ARange);
            bool G = AColor.G > (TargetColor.G - ARange) && AColor.G < (TargetColor.G + ARange);
            bool B = AColor.B > (TargetColor.B - ARange) && AColor.B < (TargetColor.B + ARange);

            return R && G && B;
        }

        public Node GetSnakeHead(Bitmap AImage)
        {
            foreach (Node snakeNode in Nodes.Where(t => t.NodeType.Equals(NodeType.Snake)))
            {
                if (!isSnakeHead(AImage, (Pixel) snakeNode.Info)) continue;

                snakeNode.NodeType = NodeType.SnakeHead;

                return snakeNode;
            }

            return null;
        }
}
}
