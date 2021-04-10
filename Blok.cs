using System.Drawing;
using System.Windows.Forms;

namespace AstarPF
{
    public class Blok
    {
        public Blok(int x, int y)
        {
            punkt = new Vector2(x, y);
            this.picBlok = new PictureBox
            {
                Name = $"pictureBox {x} | {y}",
                Size = new Size(19, 19),
                Location = new Point(x * 20, y * 20),
                BackColor = Color.Blue,
                Visible = true
            };
        }

        static public Vector2 pStart, pMeta;
        static public int iloscMeta = 0;
        static public int iloscStart = 0;

        public PictureBox picBlok;
        public Vector2 punkt;

        public bool Sciana
        {
            get { return _sciana; }
            set
            {
                if (value)
                {
                    if (_meta)
                    {
                        _meta = false;
                        iloscMeta--;
                    }
                    else if (_start)
                    {
                        _start = false;
                        iloscStart--;
                    }

                    _sciana = !_sciana;
                    picBlok.BackColor = _sciana ? Color.White : Color.Blue;
                }
            }
        }

        public bool Start
        {
            get { return _start; }
            set
            {
                if (value)
                {
                    if (_meta)
                        iloscMeta = 0;
                    iloscStart++;
                    _meta = false;
                    _start = true;
                    picBlok.BackColor = Color.Orange;
                    pStart = punkt;
                }
            }
        }

        public bool Meta
        {
            get { return _meta; }
            set
            {
                if (value)
                {
                    if (_start)
                        iloscStart = 0;
                    iloscMeta++;
                    _start = false;
                    _meta = true;
                    picBlok.BackColor = Color.Green;
                    pMeta = punkt;
                }
            }
        }

        private bool _sciana;
        private bool _start;
        private bool _meta;
    }
}
