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
        public List<Node> BorderNodes { get; set; }

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
                // pega o node da direita
                Node Node1 = GetNextNode(node, Axis.X, Sentido.Maior);

                // pega o node da esquerda
                Node Node2 = GetNextNode(node, Axis.X, Sentido.Menor);

                // pega o node de baixo
                Node Node3 = GetNextNode(node, Axis.Y, Sentido.Maior);

                // pega o node de cima
                Node Node4 = GetNextNode(node, Axis.Y, Sentido.Menor);

                // se encontrou o node, adiciona uma aresta pra ele
                if (Node1 != null) node.AddEdge(new Edge() { From = node, To = Node1, Cost = 0 });
                if (Node2 != null) node.AddEdge(new Edge() { From = node, To = Node2, Cost = 0 });
                if (Node3 != null) node.AddEdge(new Edge() { From = node, To = Node3, Cost = 0 });
                if (Node4 != null) node.AddEdge(new Edge() { From = node, To = Node4, Cost = 0 });
            });
        }

        // retorna o proximo node em alguma direção
        private Node GetNextNode(Node ANode, Axis nextAxis, Sentido sentido)
        {
            List<Node> LNodes = new List<Node>();

            if (nextAxis == Axis.X) // se quiser pegar na mesma linha
            {
                // pega os nodes da msm linha
                LNodes = Nodes.Where(t => ((Pixel)t.Info).Position.Y == ((Pixel)ANode.Info).Position.Y).ToList();

                // se quiser o proximo da direita
                if (sentido == Sentido.Maior)
                {
                    // pega os de X maior que o atual e ordena
                    LNodes = LNodes.Where(t => ((Pixel)t.Info).Position.X > ((Pixel)ANode.Info).Position.X)
                                   .OrderBy(t => ((Pixel)t.Info).Position.X).ToList();
                }
                else // se quiser o da esquerda
                {
                    // pega os de X menor e coloca na ordem inversa (pq quero o ultimo)
                    LNodes = LNodes.Where(t => ((Pixel)t.Info).Position.X < ((Pixel)ANode.Info).Position.X)
                                   .OrderByDescending(t => ((Pixel)t.Info).Position.X).ToList();
                }
            }
            else // se quiser pegar na mesma coluna
            {

                // pego os nodes da msm coluna
                LNodes = Nodes.Where(t => ((Pixel)t.Info).Position.X == ((Pixel)ANode.Info).Position.X).ToList();

                // se quiser o de baixo
                if (sentido == Sentido.Maior)
                {
                    // pego os de Y maior e ordeno
                    LNodes = LNodes.Where(t => ((Pixel)t.Info).Position.Y > ((Pixel)ANode.Info).Position.Y)
                                   .OrderBy(t => ((Pixel)t.Info).Position.Y).ToList();
                }
                else // se quiser o de cima
                {
                    // pego o de Y menor e ordeno ao contrario, quero o ultimo da lista
                    LNodes = LNodes.Where(t => ((Pixel)t.Info).Position.Y < ((Pixel)ANode.Info).Position.Y)
                                   .OrderByDescending(t => ((Pixel)t.Info).Position.Y).ToList();
                }
            }

            return LNodes.FirstOrDefault();
        }

        // defini o tipo do node, chao, comida, snake, cabeça
        public void SetTypes(Bitmap AImage)
        {
            Nodes.ForEach(node =>
            {
                node.NodeType = GetNodeType(node, AImage);
            });
        }

        public NodeType GetNodeType(Node ANode, Bitmap AImage)
        {
            // pega a posição do node
            int x = ((Pixel)ANode.Info).Position.X;
            int y = ((Pixel)ANode.Info).Position.Y;
            int num = 5;

            // pego alguns pixels em volta da posição do node
            for (int i = x - num; i < x + num; i++)
            {
                for (int j = y - num; j < y + num; j++)
                {
                    // como eu faço uma subtração ali em cima, se der menor que zero ia bugar, entao se for menor que zero eu mudo pra 0
                    Color LColor = AImage.GetPixel(x > 0 ? x : 0, y > 0 ? y : 0);

                    // comparo as cores que eu for encontrando, nao verifico a cabeça da snake aqui
                    if (CompareColors(LColor, Color.FromArgb(255, 169, 208, 73), 50)) return NodeType.Chao;

                    if (CompareColors(LColor, Color.FromArgb(255, 232, 72, 29), 50)) return NodeType.Comida;

                    if (CompareColors(LColor, Color.FromArgb(255, 71, 117, 234), 50)) return NodeType.Snake;
                }
            }

            return NodeType.Chao;
        }

        // verifico se em um print tem algum pixel branco, se tiver sei que é o olho da snake e então a cabeça esta ali
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

        // compara o R, G e B das duas cores passadas com uma margem de erro (Range), para caso haja alguma mudança de tom eu ainda consigo pegar a cor
        public bool CompareColors(Color AColor, Color TargetColor, int ARange)
        {
            bool R = AColor.R > (TargetColor.R - ARange) && AColor.R < (TargetColor.R + ARange);
            bool G = AColor.G > (TargetColor.G - ARange) && AColor.G < (TargetColor.G + ARange);
            bool B = AColor.B > (TargetColor.B - ARange) && AColor.B < (TargetColor.B + ARange);

            // somente se os tres forem true, retorno true
            return R && G && B;
        }

        // procuro a cabeça da snake na imagem
        public Node GetSnakeHead(Bitmap AImage)
        {
            // quando chamo este metodo, ja tenho os nodes onde a snake esta, entao só passo por eles
            foreach (Node snakeNode in Nodes.Where(t => t.NodeType.Equals(NodeType.Snake)))
            {
                if (!isSnakeHead(AImage, (Pixel)snakeNode.Info)) continue;

                // se for a cabeça, defino o tipo dela e retorno o node
                snakeNode.NodeType = NodeType.SnakeHead;

                return snakeNode;
            }

            return null;
        }

        // ignora esse aqui por enquanto, to testando
        public void FindBorderNodes()
        {
            BorderNodes = new List<Node>();

            // cima
            BorderNodes.AddRange(Nodes.Where(t => (t.Info as Pixel).Row == 0));

            //baixo
            BorderNodes.AddRange(Nodes.Where(t => (t.Info as Pixel).Row == 14));

            // esquerda
            BorderNodes.AddRange(Nodes.Where(t => (t.Info as Pixel).Col == 0));

            // direita
            BorderNodes.AddRange(Nodes.Where(t => (t.Info as Pixel).Col == 16));
        }
    }
}
