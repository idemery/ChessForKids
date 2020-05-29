using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public interface ICell
    {
        IPiece Piece { get; set; }
        bool IsEmpty { get; set; }
        Position Position { get; set; }
        string Text { get; set; }
        Font Font { get; set; }
        Color ForeColor { get; set; }
        Color BackColor { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        object Tag { get; set; }
        Point Location { get; set; }
        event System.EventHandler Click;

        void RemovePiece();
    }

    public struct Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Board
    {
        private readonly PrivateFontCollection mFontCollection = new PrivateFontCollection();
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
        //SINGLETON
        protected Board()
        {
            // load font
            Stream fontStream = new MemoryStream(Properties.Resources.chessmerida_webfont);
            int fontLength = Properties.Resources.chessmerida_webfont.Length;
            byte[] fontdata = Properties.Resources.chessmerida_webfont;
            System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);
            Marshal.Copy(fontdata, 0, data, fontLength);
            mFontCollection.AddMemoryFont(data, fontLength);
        }

        #region Private Fields
        private static Board _board;
        private bool cellClicked = false;
        private ICell[,] cells = new ICell[8, 8];
        private List<Position> availableMoves = new List<Position>();

        private Color colorTurn = Color.White;
        private Color whiteColor = Color.White;
        private Color blackColor = Color.White;

        private int sourceX = 0;
        private int sourceY = 0;
        #endregion

        #region Properties
        private bool CellClicked
        {
            get { return cellClicked; }
            set
            {
                if (value == true)
                {
                    CalcAvailableMoves(sourceX, sourceY);
                    if (availableMoves.Count < 1)
                    {
                        value = false;
                    }
                }
                else
                {
                    for (int i = 0; i < availableMoves.Count; i++)
                    {
                        cells[availableMoves[i].X, availableMoves[i].Y].BackColor = (Color)cells[availableMoves[i].X, availableMoves[i].Y].Tag;
                    }
                    availableMoves.Clear();
                }
                cellClicked = value;
            }
        }
        public ICell[,] Cells { get { return cells; } }
        public static Color ImBlackOrWhite { get; set; }
        public Color ColorTurn { get { return colorTurn; } set { colorTurn = value; } }
        #endregion

        #region Private Methods
        private void CalcAvailableMoves(int sourceX, int sourceY)
        {
            availableMoves = cells[sourceX, sourceY].Piece.GetAvailableMoves(ref cells, sourceX, sourceY);
        }

        // 8, 8, 5, 5, 5, 5, 12
        private ICell[,] CreateCells<T>(int xColumns, int yRows, int startX, int startY,
            int width, int height, int increment, Color black, Color white, Color imBlackOrWhite) where T : ICell
        {

            bool whiteCell = true;

            for (int _x = 0; _x < xColumns; _x++)
            {
                for (int _y = 0; _y < yRows; _y++)
                {
                    T cell = System.Activator.CreateInstance<T>();

                    cell.Width = width;
                    cell.Height = height;

                    cell.Location = new System.Drawing.Point(startX + (_x * width), startY + (_y * height));
                    cell.Position = new Position() { X = _x, Y = _y };

                    //===========================================================================//
                    cell.Piece = GetOriginalPieceByPosition(_x, _y, black, white, imBlackOrWhite);
                    cell.IsEmpty = (cell.Piece == null);
                    if (!cell.IsEmpty)
                    {
                        cell.Font = new Font(mFontCollection.Families[0], 50f);
                        cell.Text = cell.Piece.Color == white ? cell.Piece.WhiteName : cell.Piece.BlackName;
                    }
                    //===========================================================================//

                    cell.BackColor = whiteCell == true ? Color.White : Color.Gold;
                    //Set original color
                    cell.Tag = whiteCell == true ? Color.White : Color.Gold;

                    cells[_x, _y] = cell;
                    whiteCell = !whiteCell;
                }
                whiteCell = !whiteCell;
            }
            return cells;
        }

        private Piece GetOriginalPieceByPosition(int x, int y, Color black, Color white, Color imBlackOrWhite)
        {
            if (imBlackOrWhite != white)
            {
                return GetOriginalPieceByPosition(x, y, white, black, imBlackOrWhite);
            }

            Piece piece = null;
            //ROOK
            if (x == 0 || x == 7)
            {
                if (y == 0)
                {
                    piece = new Rook();
                    piece.Color = black;
                }
                if (y == 7)
                {
                    piece = new Rook();
                    piece.Color = white;
                }
            }

            //KNIGHT
            if (x == 1 || x == 6)
            {
                if (y == 0)
                {
                    piece = new Knight();
                    piece.Color = black;
                }
                if (y == 7)
                {
                    piece = new Knight();
                    piece.Color = white;
                }
            }

            //BISHOP
            if (x == 2 || x == 5)
            {
                if (y == 0)
                {
                    piece = new Bishop();
                    piece.Color = black;
                }
                if (y == 7)
                {
                    piece = new Bishop();
                    piece.Color = white;
                }
            }

            //QUEEN 
            if (x == 3)
            {
                if (y == 0)
                {
                    piece = new Queen();
                    piece.Color = black;
                }
                if (y == 7)
                {
                    piece = new Queen();
                    piece.Color = white;
                }
            }

            //King
            if (x == 4)
            {
                if (y == 0)
                {
                    piece = new King();
                    piece.Color = black;
                }
                if (y == 7)
                {
                    piece = new King();
                    piece.Color = white;
                }
            }

            //PAWN
            if (y == 1)
            {
                piece = new Pawn();
                piece.Color = black;
            }
            if (y == 6)
            {
                piece = new Pawn();
                piece.Color = white;
            }

            return piece;
        }
        private void Move(int targetX, int targetY)
        {
            if (!availableMoves.Contains(new Position() { X = targetX, Y = targetY }))
            {
                return;
            }

            cells[targetX, targetY].Piece = cells[sourceX, sourceY].Piece;

            //====================================================//
            //3ashan el pawn yemshi 5atwa wa7da bs ba3d awel 5atwa
            if (cells[sourceX, sourceY].Piece is Pawn) // todo if Piece is Pawn
            {
                Pawn pawn = (Pawn)cells[sourceX, sourceY].Piece;
                pawn.IsFirstMove = false;
            }
            //====================================================//

            cells[sourceX, sourceY].RemovePiece();

            if (ColorTurn == whiteColor)
            {
                ColorTurn = blackColor;
            }
            else
            {
                ColorTurn = whiteColor;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize a new instance of game board if it is only not initialized yet
        /// </summary>
        /// <param name="whiteColor">The White color of board cells</param>
        /// <param name="blackColor">The Black color of board cells</param>
        /// <returns></returns>
        public static Board InitBoard(Color whiteColor, Color blackColor)
        {
            if (_board == null)
            {
                _board = new Board();
                _board.CellClicked = false;
                _board.whiteColor = whiteColor;
                _board.blackColor = blackColor;
                _board.ColorTurn = whiteColor;
            }
            return _board;
        }

        /// <summary>
        /// Starts the game and creates board cells
        /// </summary>
        /// <param name="imBlackOrWhite">Choose white or black</param>
        /// <returns>List of cells created</returns>
        public ICell[,] StartGame(Color imBlackOrWhite)
        {
            Board.ImBlackOrWhite = imBlackOrWhite;
            return CreateCells<Cell>(8, 8, 5, 5, 90, 90, 12, blackColor, whiteColor, imBlackOrWhite);
        }

        /// <summary>
        /// Call this method in the cell_clicked event handler
        /// </summary>
        /// <param name="x">The x position of sender clicked cell</param>
        /// <param name="y">The y position of sender clicked cell</param>
        public void ClickCell(int x, int y)
        {
            if (CellClicked)
            {
                if (Cells[sourceX, sourceY].Piece.Color != ColorTurn)
                {
                    return;
                }
                UnhoverCell(x, y);
                Move(x, y);
                CellClicked = false;
            }
            else
            {
                if (Cells[x, y].Piece != null)
                {
                    if (Cells[x, y].Piece.Color != ColorTurn)
                    {
                        return;
                    }
                    sourceX = x;
                    sourceY = y;
                    CellClicked = true;
                }
            }
        }



        public void HoverCell(int x, int y)
        {
            if (!CellClicked
                || Cells[x, y].Piece == null
                || Cells[x, y].Piece.Color == ColorTurn
                || !availableMoves.Any(p => p.X == x && p.Y == y))
            {
                return;
            }

            foreach (var cl in Cells)
            {
                if (cl.Piece == null || cl.Piece.Color == ColorTurn)
                {
                    continue;
                }

                List<Position> positions = cl.Piece.GetAvailableMoves(ref cells, cl.Position.X, cl.Position.Y, Cells[x, y]);
                if (positions.Any(p => p.X == x && p.Y == y))
                {
                    (cl as Button).FlatStyle = FlatStyle.Flat;
                    (cl as Button).FlatAppearance.BorderColor = Color.Red;
                    (cl as Button).FlatAppearance.BorderSize = 6;
                    //cl.ForeColor = Color.White;
                }
            }
        }

        public void UnhoverCell(int x, int y)
        {
            if (!CellClicked
                || Cells[x, y].Piece == null
                || Cells[x, y].Piece.Color == ColorTurn
                || !availableMoves.Any(p => p.X == x && p.Y == y))
            {
                return;
            }

            foreach (var cl in Cells)
            {
                if (cl.Piece == null || cl.Piece.Color == ColorTurn)
                {
                    continue;
                }

                List<Position> positions = cl.Piece.GetAvailableMoves(ref cells, cl.Position.X, cl.Position.Y, Cells[x, y]);
                if (positions.Any(p => p.X == x && p.Y == y))
                {
                    //cl.BackColor = (Color)cl.Tag;
                    //cl.ForeColor = Color.Black;
                    (cl as Button).FlatStyle = FlatStyle.Standard;
                }
            }

        }
        #endregion
    }

    //public interface IBoard
    //{
    //    Board InitBoard(Color whiteColor, Color blackColor);

    //    Cell[,] StartGame(Color imBlackOrWhite);

    //    void ClickCell(int x, int y);
    //}

    public interface IGame
    {
        void Join();

        void ClickCell(int x, int y);
    }

    public interface IGameCallback
    {
        void UserJoined();

        void StartGame(ChessColor color);

        void ColorTurnChanged(ChessColor color);

        void ClickCellCallback(int x, int y);
    }

    public enum ChessColor
    {
        White, Black
    }
}