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

    public partial class Form1 : Form// syf sie zadzial
    {
        List<List<blok>> bloki = new List<List<blok>>();
        bool wkliknietyCTRL = false;
        int iloscMeta = 0;
        int iloscStart = 0;
        Point pStart, pMeta;
        public const int WielkoscX = 20;
        public const int WielkoscY = 20;
        public const int DelayWPokazywaniuTrasy = 100;
        public static List<List<Node>> grid = new List<List<Node>>();
        public static List<Node> UkonczoneSciezka;

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
                            iloscMeta--;
                        }
                        else if (bloki[tempX][tempY].start)
                        {
                            bloki[tempX][tempY].start = false;
                            iloscStart--;
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

                        if(iloscStart == 0)
                        {
                            if (bloki[tempX][tempY].meta)
                                iloscMeta = 0;
                            iloscStart++;
                            bloki[tempX][tempY].meta = false;
                            bloki[tempX][tempY].start = true;
                            bloki[tempX][tempY].picBlok.BackColor = Color.Orange;
                            pStart = new Point(tempX, tempY);
                        }
                        else if (iloscMeta == 0)
                        {
                            if(bloki[tempX][tempY].start)
                                iloscStart = 0;
                            iloscMeta++;
                            bloki[tempX][tempY].meta = true;
                            bloki[tempX][tempY].start = false;
                            bloki[tempX][tempY].picBlok.BackColor = Color.Green;
                            pMeta = new Point(tempX, tempY);
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
            else if (tempKey.KeyCode == Keys.P && iloscMeta == 1 && iloscStart == 1)
            {
                for (int i = 0; i < WielkoscX; i++)
                {
                    List<Node> tempGrid = new List<Node>();
                    for (int j = 0; j < WielkoscY; j++)
                    {
                        tempGrid.Add(new Node(new Vector2(bloki[i][j].punkt.X, bloki[i][j].punkt.Y), !bloki[i][j].sciana));
                    }
                    grid.Add(tempGrid);
                }
                FindPath(grid[pStart.X][pStart.Y], grid[pMeta.X][pMeta.Y]);
                foreach (var i in UkonczoneSciezka)
                {
                    bloki[i.polozenie.x][i.polozenie.y].picBlok.BackColor = Color.Red;
                    await Task.Delay(DelayWPokazywaniuTrasy);
                }
            }
        }

        static void FindPath(Node startNode, Node koniecNode)
        {
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node node = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fKoszt < node.fKoszt || openSet[i].fKoszt == node.fKoszt)
                    {
                        if (openSet[i].hKoszt < node.hKoszt)
                            node = openSet[i];
                    }
                }

                openSet.Remove(node);
                closedSet.Add(node);

                if (node == koniecNode)
                {
                    RetracePath(startNode, koniecNode);
                    return;
                }

                foreach (Node neighbour in Node.ZdobadziSasiadow(node))
                {
                    if (!neighbour.moznaChodzic || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newCostToNeighbour = node.gKoszt + GetDistance(node, neighbour);
                    if (newCostToNeighbour < neighbour.gKoszt || !openSet.Contains(neighbour))
                    {
                        neighbour.gKoszt = newCostToNeighbour;
                        neighbour.hKoszt = GetDistance(neighbour, koniecNode);
                        neighbour.rodzic = node;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }

        static void RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.rodzic;
            }
            path.Reverse();

            UkonczoneSciezka = path;
        }

        static int GetDistance(Node nodeA, Node nodeB)
        {
            int dstX = Math.Abs(nodeA.polozenie.x - nodeB.polozenie.x);
            int dstY = Math.Abs(nodeA.polozenie.y - nodeB.polozenie.y);

            if (dstX > dstY)
                return 14 * dstY + 10 * (dstX - dstY);
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }

    public class Vector2
    {
        public int x { get; set; }
        public int y { get; set; }

        public Vector2(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    public class Node
    {
        public Vector2 polozenie;
        public bool moznaChodzic;

        public int gKoszt;
        public int hKoszt;
        public Node rodzic;

        public Node(Vector2 _polozenie, bool _moznaChodzic)
        {
            polozenie = _polozenie;
            moznaChodzic = _moznaChodzic;
        }

        public int fKoszt { get { return gKoszt + hKoszt; } }

        public static List<Node> ZdobadziSasiadow(Node node)
        {
            List<Node> nody = new List<Node>();

            if (node.polozenie.x < Form1.WielkoscX - 1)
            {
                nody.Add(Form1.grid[node.polozenie.x + 1][node.polozenie.y]);
            }
            if (node.polozenie.x > 0)
            {
                nody.Add(Form1.grid[node.polozenie.x - 1][node.polozenie.y]);
            }
            if (node.polozenie.y < Form1.WielkoscY - 1)
            {
                nody.Add(Form1.grid[node.polozenie.x][node.polozenie.y + 1]);
            }
            if (node.polozenie.y > 0)
            {
                nody.Add(Form1.grid[node.polozenie.x][node.polozenie.y - 1]);
            }

            return nody;
        }
    }
}
