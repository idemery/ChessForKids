using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace idemery
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public static Color WhiteColor => Color.White;
        public static Color BlackColor => Color.Black;

        private Board board = null;
        private Color myColor = Color.White;

        private void Form1_Load(object sender, EventArgs e)
        {
            board = Board.InitBoard(WhiteColor, BlackColor);

            ICell[,] cells = null;
            DialogResult result = MessageBox.Show("Do you want white color?", "White Or Black", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                myColor = WhiteColor;
            }
            else
            {
                myColor = BlackColor;
            }

            cells = board.StartGame(myColor);

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    panelBoard.Controls.Add((Control)cells[x, y]);
                    cells[x, y].Click += new EventHandler(Form1_Click);
                    (cells[x, y] as Control).MouseEnter += Form1_MouseEnter;
                    (cells[x, y] as Control).MouseLeave += Form1_MouseLeave;
                }
            }
        }

        private void Form1_MouseLeave(object sender, EventArgs e)
        {
            board.UnhoverCell((sender as Cell).Position.X, (sender as Cell).Position.Y);
        }

        private void Form1_MouseEnter(object sender, EventArgs e)
        {
            board.HoverCell((sender as Cell).Position.X, (sender as Cell).Position.Y);
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            //if (board.ColorTurn != myColor)
            //    return;

            board.ClickCell((sender as Cell).Position.X, (sender as Cell).Position.Y);
        }
    }


    public class Cell : Button, ICell
    {
        private readonly PrivateFontCollection mFontCollection = new PrivateFontCollection();
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);

        public Cell()
        {
            // load font
            Stream fontStream = new MemoryStream(Properties.Resources.chessmerida_webfont);
            int fontLength = Properties.Resources.chessmerida_webfont.Length;
            byte[] fontdata = Properties.Resources.chessmerida_webfont;
            System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);
            Marshal.Copy(fontdata, 0, data, fontLength);
            mFontCollection.AddMemoryFont(data, fontLength);

            this.UseCompatibleTextRendering = true;
        }
        private IPiece piece = null;
        public IPiece Piece
        {
            get
            {
                return piece;
            }
            set
            {
                piece = value;
                if (value != null)
                {
                    this.Font = new Font(mFontCollection.Families[0], 50f);
                    this.Text = value.Color == MainForm.WhiteColor ? value.WhiteName : value.BlackName;
                    IsEmpty = false;
                }
                else
                {
                    IsEmpty = true;
                }
            }
        }

        public bool IsEmpty { get; set; }

        public Position Position { get; set; }

        public void RemovePiece()
        {
            this.Piece = null;
            this.Text = string.Empty;
        }
    }
}
