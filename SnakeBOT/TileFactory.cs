using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeBOT
{
    public class TileFactory
    {
        public Size MapSize { get { return new Size(17, 15); }}
        public Bitmap Image { get; set; }
        public List<Pixel> AllPoints { get; set; }

        public List<Pixel> CreateSnakeMap(Form AForm)
        {
            // Tiro print da tela
            GetImage(AForm);

            // Transformo a imagem em objetos do tipo Pixel
            List<Pixel> LPixels = GetGamePixelsImage();

            // Salvo os pixels da imagem numa propriedade
            AllPoints = new List<Pixel>();
            AllPoints.AddRange(LPixels);

            // Com base nos pixels, assumo as posições dos quadrados do mapa
            return GetTilesPixels(LPixels);
        }

        // criei um método para isso pq chamo em outras classes também
        public void GetImage(Form AForm)
        {
            Image = PrintScreen(AForm);
        }

        private Bitmap PrintScreen(Form AForm)
        {
            // criei uma classe para tirar print
            ScreenCapture LScreenCapture = new ScreenCapture();

            return LScreenCapture.PrintScreen(new Point(0,0), AForm.Size);
        }

        // Transforma a imagem em objetos do tipo Pixel
        private List<Pixel> GetGamePixelsImage()
        {
            List<Pixel> AllPoints = new List<Pixel>();

            // Não são verificados todos os pixels da tela, para ficar mais rápido, verificamos um pixel
            // a cada 10
            for (int i = 0; i < Image.Width; i += 10)
            {
                for (int j = 0; j < Image.Height; j += 10)
                {
                    // pego o cor do pixel
                    Color LColor = Image.GetPixel(i, j);

                    // Para facilitar, se a cor não for um dos dois verdes que são usados para criar o
                    // chão, ignoro
                    if (LColor != Color.FromArgb(255, 162, 203, 73) &&
                        LColor != Color.FromArgb(255, 170, 215, 81)) continue;

                    // transformo o pixel da imagem num objeto Pixel e adiciono na lista de pontos
                    Pixel currPixel = new Pixel(new Point(i, j), LColor);

                    AllPoints.Add(currPixel);
                }
            }

            return AllPoints;
        }

        // Retorna os Pixel que estão do lado no pixel passado por parametro
        public List<Pixel> GetPixelsAdjacentes(Pixel currPixel, List<Pixel> allPixels)
        {
            List<Pixel> Adjacentes = new List<Pixel>();
            int x = currPixel.Position.X;
            int y = currPixel.Position.Y;

            // Crio as posições que vou verificar, coloco 10 pq foi usado esse numero na hora de criar os
            // objetos
            List<Point> PossiblePositions = new List<Point>()
            {
                new Point(x + 10, y), // direita
                new Point(x - 10, y), // esquerda
                new Point(x, y - 10), // cima
                new Point(x, y + 10)  // baixo
            };

            // Verifico cada posição que criei
            PossiblePositions.ForEach(pos =>
            {
                // Procuro na lista se essa posição existe e faz parte do mesmo bloco de cor
                Pixel pixel = allPixels.FirstOrDefault(p => p.Position.X == pos.X
                                                            && p.Position.Y == pos.Y
                                                            && p.Color == currPixel.Color);

                if (pixel == null) return;

                allPixels.Remove(pixel);
                Adjacentes.Add(pixel);
            });

            return Adjacentes;
        }

        // Agrupa os pixels de mesma cor
        private List<Pixel> GetTilesPixels(List<Pixel> AllPixels)
        {
            List<Tile> LAllTiles = new List<Tile>();

            Queue<Pixel> PixelsQueue = new Queue<Pixel>();
            Pixel LPixel;

            // Enquanto encontrar algum pixel que ainda não foi visitado
            while ((LPixel = AllPixels.FirstOrDefault(t => !t.Visited)) != null)
            {
                // Cria o objeto que guardara os pixel daquela cor
                LAllTiles.Add(new Tile());
                PixelsQueue.Enqueue(LPixel);

                // mesma ideia do algoritmo do Paint de EDA 1, pega um pixel e vai colocando todos os pixels
                // de mesma cor em volta dele numa lista, até não achar nenhum de mesma cor
                while (PixelsQueue.Count > 0)
                {
                    Pixel currPixel = PixelsQueue.Dequeue();
                    currPixel.Visited = true;

                    // Coloca os pixels adjacentes na fila
                    foreach (var adj in GetPixelsAdjacentes(currPixel, AllPixels))
                    {
                        adj.Visited = true;
                        PixelsQueue.Enqueue(adj);
                    }

                    // adiciona o pixel na lista
                    LAllTiles.LastOrDefault().Pixels.Add(currPixel);
                }
            }

            // Como não preciso trabalhar com tanta precisão, para aumentar a performance
            // vou fazer uma média da posição de cada quadrado e criar um pixel no centro que representa ele

            List<Pixel> LPixelsInCenter = new List<Pixel>();

            // para cada quadrado
            foreach (var LTile in LAllTiles)
            {
                // pego os pixels da borda
                int minX = LTile.Pixels.Min(t => t.Position.X);
                int minY = LTile.Pixels.Min(t => t.Position.Y);
                int maxX = LTile.Pixels.Max(t => t.Position.X);
                int maxY = LTile.Pixels.Max(t => t.Position.Y);

                // faço a posição média
                int x = (minX + maxX) / 2;
                int y = (minY + maxY) / 2;

                // crio um pixel no centro e adiciono em outra lista
                LPixelsInCenter.Add(new Pixel(new Point(x, y), Color.Red));
            }

            // O problema disso é que no começo no jogo, existem elementos dentro do mapa, como a snake e a comida
            // Com esse método, até agora, não conseguimos verificar onde estão TODOS os quadrados do mapa,
            // os quadrados que não estavam vazios não foram reconhecidos.
            // Para contornar isso, olhando o jogo, conseguimos assumir que as bordas SEMPRE vão iniciar o jogo vazias
            // então, sabemos que elas são geradas corretamente.
            // Vamos pegar as posições delas e assumir a posição que todos os outros quadrados devem estar

            // Lista final do mapa
            List<Pixel> LFieldPixels = new List<Pixel>();
            int row, col;
            row = col = 0;

            // pego a borda de cima
            LPixelsInCenter.OrderBy(t => t.Position.Y).Take(MapSize.Width).OrderBy(t => t.Position.Y).ToList().ForEach(topTile =>
            {
                // pego a borda da esquerda
                LPixelsInCenter.OrderBy(t => t.Position.X).Take(MapSize.Height).OrderBy(t => t.Position.X).ToList().ForEach(leftTile =>
                {
                    // completo um retangulo adicionando pixels baseados nas posições das bordas
                    LFieldPixels.Add(new Pixel(new Point(topTile.Position.X, leftTile.Position.Y), Color.Blue, row, col));
                    row++;
                });

                row = 0;
                col++;
            });

            return LFieldPixels;
        }
    }
}
