using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace idemery
{
    public partial class ChessBoardCtrl : UserControl
    {
        public ChessBoardCtrl()
        {
            InitializeComponent();
        }

        private void ChessBoardCtrl_Load(object sender, EventArgs e)
        {

        }

        Board board = null;

        private void CreateCells(PlayerColor color)
        {
            board = Board.InitBoard(Color.Blue, Color.Black);

            ICell[,] cells = null;
            DialogResult result = MessageBox.Show("Do you want white color?", "White Or Black", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                cells = board.StartGame(Color.Blue);
            }
            else
            {
                cells = board.StartGame(Color.Black);
            }

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    panelBoard.Controls.Add((Control)cells[x, y]);
                    cells[x, y].Click += new EventHandler(Cell_Click);
                }
            }
        }

        void Cell_Click(object sender, EventArgs e)
        {
            board.ClickCell((sender as Cell).Position.X, (sender as Cell).Position.Y);
        }
    }

    public enum PlayerColor
    {
        White,
        Black
    }
}
