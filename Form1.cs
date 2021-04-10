using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AstarPF
{
    /*
     * Start - pomaranczowy
     * Koniec - Zielony
     * Sciana - Bialy
     * 
     * Przyciski:
     * P - Zaczyna szukanie trasy
     * CTRL - Trzeba kliknac aby postawic meta/start
     */

    // Syf jeszcze mniejszy nawet mozna powiedziesc ze 0.4v XD

    public partial class Form1 : Form
    {
        public static List<List<Node>> grid = new List<List<Node>>();
        public static List<Node> UkonczoneSciezka;

        List<List<Blok>> bloki = new List<List<Blok>>();
        bool wkliknietyCTRL = false;
        public const int WielkoscX = 20;
        public const int WielkoscY = 20;
        public const int DelayWPokazywaniuTrasy = 50;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Tworzy liste 2D blokow
            for (int x = 0; x < WielkoscX; x++)
            {
                List<Blok> tempBloki = new List<Blok>();
                for (int y = 0; y < WielkoscY; y++)
                {
                    var temp = new Blok(x, y);
                    tempBloki.Add(temp);
                }
                bloki.Add(tempBloki);
            }

            // Dodaje do kazdego bloku z listy event i kontrolke
            for (int x = 0; x < WielkoscX; x++)
            {
                for (int y = 0; y < WielkoscY; y++)
                {
                    bloki[x][y].picBlok.Click += Form1_Click;
                    this.Controls.Add(bloki[x][y].picBlok);
                }
            }

            // Nadaje Wielkosc w klasie node
            Node.WielkoscX = WielkoscX;
            Node.WielkoscY = WielkoscY;
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            MouseEventArgs tempMouse = (MouseEventArgs)e;
            var temp = sender as PictureBox;
            if (temp != null)
            {
                var tempX = temp.Location.X / 20;
                var tempY = temp.Location.Y / 20;
                if (tempX >= 0 && tempX < WielkoscX && tempY >= 0 && tempY < WielkoscY)
                {
                    if (tempMouse.Button == MouseButtons.Left)
                        bloki[tempX][tempY].Sciana = true;
                    else if (tempMouse.Button == MouseButtons.Right && wkliknietyCTRL)
                    {
                        wkliknietyCTRL = false;
                        if (bloki[tempX][tempY].Sciana)
                            bloki[tempX][tempY].Sciana = true;

                        if (Blok.iloscStart == 0)
                            bloki[tempX][tempY].Start = true;
                        else if (Blok.iloscMeta == 0)
                            bloki[tempX][tempY].Meta = true;
                    }
                }
            }
        }

        async private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
                wkliknietyCTRL = true;
            else if (e.KeyCode == Keys.P && Blok.iloscMeta == 1 && Blok.iloscStart == 1)
            {
                grid = new List<List<Node>>();
                for (int i = 0; i < WielkoscX; i++)
                {
                    List<Node> tempGrid = new List<Node>();
                    for (int j = 0; j < WielkoscY; j++)
                    {
                        tempGrid.Add(new Node(bloki[i][j].punkt, !bloki[i][j].Sciana));
                    }
                    grid.Add(tempGrid);
                }

                AStar.FindPath(grid[Blok.pStart.x][Blok.pStart.y], grid[Blok.pMeta.x][Blok.pMeta.y], grid);
                UkonczoneSciezka = AStar.UkonczonaSciezka;
                if (UkonczoneSciezka == null)
                    return;

                foreach (var i in UkonczoneSciezka)
                {
                    bloki[i.polozenie.x][i.polozenie.y].picBlok.BackColor = Color.Red;
                    await Task.Delay(DelayWPokazywaniuTrasy);
                }
            }
        }
    }
}