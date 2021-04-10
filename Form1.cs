using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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

    // Syf nadal ale mniejszy wiec dobrze

    public partial class Form1 : Form
    {
        public static List<List<Node>> grid = new List<List<Node>>();
        public static List<Node> UkonczoneSciezka;

        List<List<blok>> bloki = new List<List<blok>>();
        bool wkliknietyCTRL = false;
        Vector2 pStart, pMeta;
        public const int WielkoscX = 20;
        public const int WielkoscY = 20;
        public const int DelayWPokazywaniuTrasy = 50;

        class blok
        {
            public blok(int x, int y)
            {
                punkt = new Point(x, y);
                this.picBlok = new PictureBox
                {
                    Name = $"pictureBox {x} | {y}",
                    Size = new Size(19, 19),
                    Location = new Point(x * 20, y * 20),
                    BackColor = Color.Blue,
                    Visible = true
                };
            }

            static public int iloscMeta = 0;
            static public int iloscStart = 0;

            public Point punkt;
            public bool sciana;
            public bool start;
            public bool meta;
            public PictureBox picBlok;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Tworzy liste 2D blokow
            for (int x = 0; x < WielkoscX; x++)
            {
                List<blok> tempBloki = new List<blok>();
                for (int y = 0; y < WielkoscY; y++)
                {
                    var temp = new blok(x, y);
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
                    {
                        if (bloki[tempX][tempY].meta)
                        {
                            bloki[tempX][tempY].meta = false;
                            blok.iloscMeta--;
                        }
                        else if (bloki[tempX][tempY].start)
                        {
                            bloki[tempX][tempY].start = false;
                            blok.iloscStart--;
                        }

                        bloki[tempX][tempY].sciana = !bloki[tempX][tempY].sciana;
                        bloki[tempX][tempY].picBlok.BackColor = bloki[tempX][tempY].sciana ? Color.White : Color.Blue;
                    }
                    else if (tempMouse.Button == MouseButtons.Right && wkliknietyCTRL)
                    {
                        wkliknietyCTRL = false;
                        if(bloki[tempX][tempY].sciana)
                        {
                            bloki[tempX][tempY].sciana = false;
                            bloki[tempX][tempY].picBlok.BackColor = Color.Blue;
                        }

                        if(blok.iloscStart == 0)
                        {
                            if (bloki[tempX][tempY].meta)
                                blok.iloscMeta = 0;
                            blok.iloscStart++;
                            bloki[tempX][tempY].meta = false;
                            bloki[tempX][tempY].start = true;
                            bloki[tempX][tempY].picBlok.BackColor = Color.Orange;
                            pStart = new Vector2(tempX, tempY);
                        }
                        else if (blok.iloscMeta == 0)
                        {
                            if(bloki[tempX][tempY].start)
                                blok.iloscStart = 0;
                            blok.iloscMeta++;
                            bloki[tempX][tempY].meta = true;
                            bloki[tempX][tempY].start = false;
                            bloki[tempX][tempY].picBlok.BackColor = Color.Green;
                            pMeta = new Vector2(tempX, tempY);
                        }
                    }
                }
            }
        }

        async private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var tempKey = (KeyEventArgs)e;

            if (tempKey.KeyCode == Keys.ControlKey)
                wkliknietyCTRL = true;
            else if (tempKey.KeyCode == Keys.P && blok.iloscMeta == 1 && blok.iloscStart == 1)
            {
                grid = new List<List<Node>>();
                for (int i = 0; i < WielkoscX; i++)
                {
                    List<Node> tempGrid = new List<Node>();
                    for (int j = 0; j < WielkoscY; j++)
                    {
                        tempGrid.Add(new Node(new Vector2(bloki[i][j].punkt.X, bloki[i][j].punkt.Y), !bloki[i][j].sciana));
                    }
                    grid.Add(tempGrid);
                }
                AStar.FindPath(grid[pStart.x][pStart.y], grid[pMeta.x][pMeta.y], grid);
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