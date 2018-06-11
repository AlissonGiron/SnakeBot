using EDA;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Models.Enums;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace SnakeBOT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private PlayManager FPlayManager;
        private List<PathNode> PrincipalPath;
        private List<PathNode> AuxPath;
        public Graph Graph { get; set; }

        public List<Node> DirectionBorder { get; set; }

        private void btnGerarGrafo_Click(object sender, EventArgs e)
        {
            TileFactory LFactory = new TileFactory();

            // crio o mapa do jogo
            List<Pixel> LMap = LFactory.CreateSnakeMap(this);
            Graph LGraph = new Graph();
            Graph = LGraph;

            // transformo os pixels do mapa em Nodes do grafo
            foreach (var p in LMap)
            {
                LGraph.AddNode(new Node() { Info = p });
            }

            AuxPath = new List<PathNode>();

            // Crio as arestas
            LGraph.CreateEdges();

            // define os tipos de cada quadrado do mapa
            LGraph.SetTypes(LFactory.Image);

            // encontra a cabeça da snake
            LGraph.GetSnakeHead(LFactory.Image);

            // ignora por enquanto, to testando
            LGraph.FindBorderNodes();

            DirectionBorder = LGraph.BorderNodes;

            FPlayManager = new PlayManager(LGraph);
            PrincipalPath = AdjustPath(FPlayManager.GetPath());

            Task.Run(() =>
            {
                while (true)
                {
                    FollowPath(LFactory, LGraph);

                    PrincipalPath = AdjustPath(AuxPath);

                    if (PrincipalPath.Count == 0)
                    {
                        break;
                    }

                    AuxPath.Clear();
                }
            });
        }

        private string GetPathString(List<PathNode> Path)
        {
            string text = "";

            foreach (var item in Path)
            {
                text += (" -> " + item.nextNodeDir);
            }

            return text.Substring(3);
        }

        private List<PathNode> AdjustPath(List<PathNode> APathToAdjust)
        {
            Direction LLastDirection = Direction.Right;
            List<PathNode> LPathNode = new List<PathNode>();

            for (int i = 0; i < APathToAdjust.Count; i++)
            {
                if (i == 0 || i == APathToAdjust.Count - 1 || APathToAdjust[i].nextNodeDir != LLastDirection)
                {
                    LLastDirection = APathToAdjust[i].nextNodeDir;
                    LPathNode.Add(APathToAdjust[i]);
                }
            }

            return LPathNode;
        }

        private void GetNextPath(TileFactory AFactory, Graph AGraph)
        {
            Thread.Sleep(200);

            while (AuxPath.Count == 0)
            {
                AFactory.GetImage(this);
                AGraph.SetTypes(AFactory.Image);
                AGraph.GetSnakeHead(AFactory.Image);
                AuxPath = FPlayManager.GetPath((Node)PrincipalPath.LastOrDefault()?.nextNode);
            }
        }

        Rectangle LRect = new Rectangle(new Point(0, 0), new Size(40, 40));

        private void FollowPath(TileFactory AFactory, Graph AGraph)
        {
            bool FoodAlive = true;

            wbSnake.BeginInvoke((Action)(() =>
            {
                wbSnake.Document.Focus();
            }));

            while (PrincipalPath.Count > 0)
            {
                var LCurrPixel = ((Pixel)((Node)PrincipalPath.FirstOrDefault().currNode).Info);
                LRect.X = LCurrPixel.Position.X - 20;
                LRect.Y = LCurrPixel.Position.Y - 20;

                Bitmap LBitmap = GetPixels(LRect);

                if (!isSnakeHead(LBitmap))
                {
                    continue;
                }

                SendKeys.SendWait(FPlayManager.ControlSnakeToTarget(PrincipalPath.FirstOrDefault()));

                if (FoodAlive && GotFood(AGraph))
                {
                    FoodAlive = false;

                    var t = new Thread(() => GetNextPath(AFactory, AGraph));
                    t.Start();
                }

                PrincipalPath.RemoveAt(0);
            }
        }

        private Node FindSnakeHead(Graph AGraph)
        {
            foreach (var LNode in AGraph.Nodes)
            {
                var LTile = GetPixels(new Rectangle((LNode.Info as Pixel).Position.X - 20, (LNode.Info as Pixel).Position.Y - 20, 40, 40));
                if (isSnakeHead(LTile)) return LNode;
            }

            return null;
        }

        Rectangle Lrects2 = new Rectangle(new Point(0, 0), new Size(20, 20));


        private Node SnakeInBorder(Graph AGraph)
        {
            foreach (var node in AGraph.BorderNodes)
            {
                var LCurrPixel = node.Info as Pixel;
                Lrects2.X = LCurrPixel.Position.X - 20;
                Lrects2.Y = LCurrPixel.Position.Y - 20;

                Bitmap LBitmap = GetPixels(Lrects2);

                if (isSnakeHead(LBitmap))
                {
                    return node;
                }
            }

            return null;
        }

        Rectangle Lrects = new Rectangle(new Point(0, 0), new Size(10, 10));

        private bool GotFood(Graph AGraph)
        {
            Pixel FoodPixel = ((Pixel)FPlayManager.Food.Info);
            Lrects.X = FoodPixel.Position.X - 5;
            Lrects.Y = FoodPixel.Position.Y - 5;

            Bitmap LBitmap = GetPixels(Lrects);

            for (int x = 0; x < LBitmap.Height; x++)
            {
                for (int y = 0; y < LBitmap.Width; y++)
                {
                    if (AGraph.CompareColors(LBitmap.GetPixel(x, y), Color.FromArgb(255, 232, 72, 29), 50)) return false;
                }
            }

            return true;
        }

        private bool isSnakeHead(Bitmap ABitmap)
        {
            for (int x = 0; x < ABitmap.Height; x++)
            {
                for (int y = 0; y < ABitmap.Width; y++)
                {
                    if (ABitmap.GetPixel(x, y).Equals(Color.FromArgb(255, 255, 255, 255))) return true;
                }
            }

            return false;
        }

        private void DrawPath(List<PathNode> Path)
        {
            if (Path == null) return;

            SolidBrush myBrush = new SolidBrush(Color.Yellow);
            Graphics formGraphics;
            formGraphics = this.CreateGraphics();

            foreach (var node in Path)
            {
                Pixel currPixel = (Pixel)((Node)node.currNode).Info;

                formGraphics.FillRectangle(myBrush, new Rectangle(currPixel.Position.X - 5 - wbSnake.Left, currPixel.Position.Y - 5 - wbSnake.Top, 5, 5));
            }

            myBrush.Dispose();
            formGraphics.Dispose();
        }

        private Bitmap GetPixels(Rectangle ARectangle)
        {
            Bitmap bmp = new Bitmap(ARectangle.Width, ARectangle.Height);

            using (Graphics g = Graphics.FromImage(bmp))
                g.CopyFromScreen(ARectangle.Location, Point.Empty, ARectangle.Size);

            return bmp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void wbSnake_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {

            }
        }
    }
}
//menor + (maior - menor ) / 2
