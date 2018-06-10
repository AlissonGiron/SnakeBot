using EDA;
using Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Enums;

namespace SnakeBOT
{
    public class PlayManager
    {
        public Graph Graph { get; set; }

        public Node SnakeHead { get { return Graph.Nodes.FirstOrDefault(n => n.NodeType.Equals(NodeType.SnakeHead)); } }
        public Node Food { get { return Graph.Nodes.FirstOrDefault(n => n.NodeType.Equals(NodeType.Comida)); } }



        public PlayManager(Graph AGraph)
        {
            Graph = AGraph;
        }

        public List<PathNode> GetPath(Node From = null, Node To = null)
        {
            // Reseto o grafo para pesquisar denovo
            ResetGraph();

            // Faço busca em largura, se eu passei algum node para começar uso ele, senão uso a cabeça da snake
            // se passei algum node para terminar uso ele, senão uso a comida
            return BreadthFirstSearch(From ?? SnakeHead, To ?? Food);
        }
        
        // Retorna a tecla que preciso usar para controlar a snake no momento
        public string ControlSnakeToTarget(PathNode ANode)
        {
            if (ANode.nextNodeDir == Direction.Down)
                return "{S}";
            if (ANode.nextNodeDir == Direction.Up)
                return "{W}";
            if (ANode.nextNodeDir == Direction.Left)
                return "{A}";

            return "{D}";
        }

        private void ResetGraph() => Graph.Nodes.ForEach(node =>
        {
            node.Visited = false;
            node.Parent = null;
        });

        public List<PathNode> BreadthFirstSearch(Node From, Node To)
        {
            List<PathNode> LPathNodes = new List<PathNode>();

            // Se acontecer de não achar a comida ou a cabeça da snake no algoritmo, retorno um caminho vazio, para evitar exceções
            if (From == null || To == null)
            {
                return LPathNodes;
            }

            // variavel que controla quando encontrei a comida
            bool FoodAlreadyInPath = false;

            Queue<Node> LQueue = new Queue<Node>();

            // adiciono o inicio na fila
            From.Visited = true;
            LQueue.Enqueue(From);

            while (LQueue.Count > 0)
            {
                Node LNode = LQueue.Dequeue();

                // Procuro o caminho evitando o corpo da snake e o caminho que está sendo utilizado no momento
                foreach (Edge edge in LNode.Edges.Where(t => !t.To.Visited && t.To.NodeType != NodeType.Snake && !t.To.InPath))
                {
                    edge.To.Visited = true;
                    edge.To.Parent = LNode;

                    // se encontrei o final
                    if (edge.To.Equals(To))
                    {
                        // gero o caminho entre o inicio e o fim
                        LPathNodes = GetPathNodes(From, To);

                        // reseto tudo
                        LQueue.Clear();
                        ResetGraph();

                        // indico que encontrei a comida, para entrar no proximo if
                        FoodAlreadyInPath = true;

                        // adiciono a comida na fila, pq meu caminho não termina na comida, e sim um pouco mais para frente
                        LQueue.Enqueue(edge.To);
                        break;
                    }

                    // se a comida já foi encontrada
                    if (FoodAlreadyInPath)
                    {
                        // faço o caminho do node atual até a comida
                        var AfterFood = GetPathNodes(To, edge.To);

                        // decidi utilizar mais 7 nodes dps da comida, cheguei nesse numero testando
                        if (AfterFood.Count >= 7)
                        {
                            LPathNodes.AddRange(AfterFood);

                            // limpo a fila para parar o algoritmo
                            LQueue.Clear();

                            // saio do loop
                            break;
                        }
                    }

                    // senão sai do loop antes, continuo normalmente
                    LQueue.Enqueue(edge.To);
                }
            }

            // reseto o caminho antigo
            Graph.Nodes.ForEach(o => o.InPath = false);

            // marco o novo caminho, para que não seja utilizado na proxima execução
            LPathNodes.Select(s => s.currNode).ToList().ForEach(t => ((Node)t).InPath = true);

            // retorno o caminho
            return LPathNodes;
        }

        // retorna o caminho entre dois nós
        private List<PathNode> GetPathNodes(Node From, Node To)
        {
            try
            {
                List<PathNode> Path = new List<PathNode>();
                Node LNode = To;

                while (LNode.Parent != null)
                {
                    // marco que está num caminho para não utiliza-lo novamente
                    LNode.InPath = true;

                    // Adiciono um novo objeto, indicando a direção que preciso controlar a snake
                    Path.Add(new PathNode()
                    {
                        currNode = LNode.Parent,
                        nextNode = LNode,
                        nextNodeDir = GetDirection(LNode.Parent, LNode)
                    });

                    // subo para o pai
                    LNode = LNode.Parent;
                }

                // inverto para ter o caminho na ordem certa
                Path.Reverse();
                return Path;
            }
            catch
            {
                return new List<PathNode>();
            }
        }

        // calcula a direção para chegar de um node até outro
        private Direction GetDirection(Node current, Node next)
        {
            Point currPosition = ((Pixel)current.Info).Position;
            Point nextPosition = ((Pixel)next.Info).Position;

            //      não moveu na horizontal      e           Y diminuindo
            if (currPosition.X == nextPosition.X && currPosition.Y > nextPosition.Y) return Direction.Up;

            //      não moveu na horizontal      e           Y aumentando
            if (currPosition.X == nextPosition.X && currPosition.Y < nextPosition.Y) return Direction.Down;

            //      não moveu na vertical        e           X aumentando
            if (currPosition.Y == nextPosition.Y && currPosition.X < nextPosition.X) return Direction.Right;

            return Direction.Left;
        }
    }
}
