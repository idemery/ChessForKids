using System.Collections.Generic;
using System.Drawing;

namespace idemery
{
    public interface IPiece
    {
        string WhiteName { get; }
        string BlackName { get; }

        ICell ContainerCell { get; set; }

        System.Drawing.Color Color { get; set; }

        List<Position> GetAvailableMoves(ref ICell[,] cells, int srcX, int srcY, ICell exclude = null);
    }

    public abstract class Piece : IPiece
    {
        #region IPiece Members

        public System.Drawing.Color Color
        {
            get;
            set;
        }

        public ICell ContainerCell
        {
            get;
            set;
        }

        public abstract string WhiteName { get; }
        public abstract string BlackName { get; }

        abstract public List<Position> GetAvailableMoves(ref ICell[,] cells, int srcX, int srcY, ICell exclude = null);

        protected bool CheckCell(ref ICell[,] cells, ref List<Position> availableMoves, int x, int y, ICell exclude)
        {
            if (x > 7 || y > 7 || x < 0 || y < 0)
                return false;

            if (cells[x, y].IsEmpty)
            {
                if (exclude == null) cells[x, y].BackColor = Color.LimeGreen;
                availableMoves.Add(new Position() { X = x, Y = y });
                return true;
            }
            if (cells[x, y].Piece != null)
            {
                if (this.Color == cells[x, y].Piece.Color)
                {
                    if (exclude != null && exclude.Position.X == x && exclude.Position.Y == y)
                    {
                        availableMoves.Add(new Position() { X = x, Y = y });
                    }
                    return false;
                }
                else
                {
                    if (exclude == null) cells[x, y].BackColor = Color.Red;
                    availableMoves.Add(new Position() { X = x, Y = y });
                    return false;
                }
            }
            return false;
        }

        protected void LoopOnCells(MoveDirection direction, ref ICell[,] cells, ref List<Position> availableMoves, int srcX, int srcY, ICell exclude)
        {
            int x = srcX;
            int y = srcY;

            switch (direction)
            {
                case MoveDirection.Up:
                    while (MoveCalculation.MoveDown(ref x, ref y))
                    {
                        if (CheckCell(ref cells, ref availableMoves, x, y, exclude))
                            continue;
                        else
                            break;
                    }
                    break;
                case MoveDirection.Down:
                    while (MoveCalculation.MoveUp(ref x, ref y))
                    {
                        if (CheckCell(ref cells, ref availableMoves, x, y, exclude))
                            continue;
                        else
                            break;
                    }
                    break;
                case MoveDirection.Left:
                    while (MoveCalculation.MoveLeft(ref x, ref y))
                    {
                        if (CheckCell(ref cells, ref availableMoves, x, y, exclude))
                            continue;
                        else
                            break;
                    }
                    break;
                case MoveDirection.Right:
                    while (MoveCalculation.MoveRight(ref x, ref y))
                    {
                        if (CheckCell(ref cells, ref availableMoves, x, y, exclude))
                            continue;
                        else
                            break;
                    }
                    break;
                case MoveDirection.UpLeft:
                    while (MoveCalculation.MoveLeftDown(ref x, ref y))
                    {
                        if (CheckCell(ref cells, ref availableMoves, x, y, exclude))
                            continue;
                        else
                            break;
                    }
                    break;
                case MoveDirection.UpRight:
                    while (MoveCalculation.MoveRightDown(ref x, ref y))
                    {
                        if (CheckCell(ref cells, ref availableMoves, x, y, exclude))
                            continue;
                        else
                            break;
                    }
                    break;
                case MoveDirection.DownLeft:
                    while (MoveCalculation.MoveLeftUp(ref x, ref y))
                    {
                        if (CheckCell(ref cells, ref availableMoves, x, y, exclude))
                            continue;
                        else
                            break;
                    }
                    break;
                case MoveDirection.DownRight:
                    while (MoveCalculation.MoveRightUp(ref x, ref y))
                    {
                        if (CheckCell(ref cells, ref availableMoves, x, y, exclude))
                            continue;
                        else
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        protected void LoopOnCellsBySteps(int steps, MoveDirection direction, ref ICell[,] cells, ref List<Position> availableMoves, int srcX, int srcY, ICell exclude)
        {
            for (int i = 0; i < steps; i++)
            {
                switch (direction)
                {
                    case MoveDirection.Up:
                        MoveCalculation.MoveDown(ref srcX, ref srcY);
                        CheckCell(ref cells, ref availableMoves, srcX, srcY, exclude);
                        break;
                    case MoveDirection.Down:
                        MoveCalculation.MoveUp(ref srcX, ref srcY);
                        CheckCell(ref cells, ref availableMoves, srcX, srcY, exclude);
                        break;
                    case MoveDirection.Left:
                        MoveCalculation.MoveLeft(ref srcX, ref srcY);
                        CheckCell(ref cells, ref availableMoves, srcX, srcY, exclude);
                        break;
                    case MoveDirection.Right:
                        MoveCalculation.MoveRight(ref srcX, ref srcY);
                        CheckCell(ref cells, ref availableMoves, srcX, srcY, exclude);
                        break;
                    case MoveDirection.UpLeft:
                        MoveCalculation.MoveLeftDown(ref srcX, ref srcY);
                        CheckCell(ref cells, ref availableMoves, srcX, srcY, exclude);
                        break;
                    case MoveDirection.UpRight:
                        MoveCalculation.MoveRightDown(ref srcX, ref srcY);
                        CheckCell(ref cells, ref availableMoves, srcX, srcY, exclude);
                        break;
                    case MoveDirection.DownLeft:
                        MoveCalculation.MoveLeftUp(ref srcX, ref srcY);
                        CheckCell(ref cells, ref availableMoves, srcX, srcY, exclude);
                        break;
                    case MoveDirection.DownRight:
                        MoveCalculation.MoveRightUp(ref srcX, ref srcY);
                        CheckCell(ref cells, ref availableMoves, srcX, srcY, exclude);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }

    public class Pawn : Piece
    {
        public override string WhiteName => "p";
        public override string BlackName => "o";

        private bool isFirstMove = true;
        //elly beyset IsFirstMove = false howa el engine.Move()
        public bool IsFirstMove { get { return isFirstMove; } set { isFirstMove = value; } }
        public override List<Position> GetAvailableMoves(ref ICell[,] cells, int srcX, int srcY, ICell exclude = null)
        {
            List<Position> availableMoves = new List<Position>();
            int x = srcX;
            int y = srcY;

            if (cells[srcX, srcY].Piece.Color == Board.ImBlackOrWhite)
            {
                if (isFirstMove)
                {
                    MoveCalculation.MoveDown(ref x, ref y);
                    if (!cells[x, y].IsEmpty)
                        goto line1;

                    CheckCell(ref cells, ref availableMoves, x, y, exclude);
                }

                MoveCalculation.MoveDown(ref x, ref y);
                if (x < 8 && x > -1 && y < 8 && y > -1)
                {
                    if (cells[x, y].IsEmpty)
                    {
                        CheckCell(ref cells, ref availableMoves, x, y, exclude);
                    }
                }

                //move ma2asat
            line1:
                x = srcX;
                y = srcY;

                MoveCalculation.MoveLeftDown(ref x, ref y);
                if (x < 8 && x > -1 && y < 8 && y > -1)
                {
                    if (!cells[x, y].IsEmpty)
                    {
                        if (cells[x, y].Piece.Color != cells[srcX, srcY].Piece.Color)
                        {
                            CheckCell(ref cells, ref availableMoves, x, y, exclude);
                        }
                    }
                }

                x = srcX;
                y = srcY;

                MoveCalculation.MoveRightDown(ref x, ref y);

                if (x < 8 && x > -1 && y < 8 && y > -1)
                    if (!cells[x, y].IsEmpty)
                        if (cells[x, y].Piece.Color != cells[srcX, srcY].Piece.Color)
                            CheckCell(ref cells, ref availableMoves, x, y, exclude);
            }

            else
            {
                if (isFirstMove)
                {
                    MoveCalculation.MoveUp(ref x, ref y);
                    if (!cells[x, y].IsEmpty)
                        return availableMoves;

                    CheckCell(ref cells, ref availableMoves, x, y, exclude);
                }

                MoveCalculation.MoveUp(ref x, ref y);
                if (x < 8 && x > -1 && y < 8 && y > -1)
                {
                    if (cells[x, y].IsEmpty)
                    {
                        CheckCell(ref cells, ref availableMoves, x, y, exclude);
                    }
                }


                x = srcX;
                y = srcY;

                MoveCalculation.MoveLeftUp(ref x, ref y);
                if (x < 8 && x > -1 && y < 8 && y > -1)
                {
                    if (!cells[x, y].IsEmpty)
                    {
                        if (cells[x, y].Piece.Color != cells[srcX, srcY].Piece.Color)
                        {
                            CheckCell(ref cells, ref availableMoves, x, y, exclude);
                        }
                    }
                }


                x = srcX;
                y = srcY;

                MoveCalculation.MoveRightUp(ref x, ref y);

                if (x < 8 && x > -1 && y < 8 && y > -1)
                {
                    if (!cells[x, y].IsEmpty)
                    {
                        if (cells[x, y].Piece.Color != cells[srcX, srcY].Piece.Color)
                        {
                            CheckCell(ref cells, ref availableMoves, x, y, exclude);
                        }
                    }
                }
            }

            

            return availableMoves;
        }
    }

    public class Rook : Piece
    {
        public override string WhiteName => "r";
        public override string BlackName => "t";

        public override List<Position> GetAvailableMoves(ref ICell[,] cells, int srcX, int srcY, ICell exclude = null)
        {
            List<Position> availableMoves = new List<Position>();

            LoopOnCells(MoveDirection.Left, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCells(MoveDirection.Right, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCells(MoveDirection.Up, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCells(MoveDirection.Down, ref cells, ref availableMoves, srcX, srcY, exclude);

            return availableMoves;
        }

    }

    public class Knight : Piece
    {
        public override string WhiteName => "n";
        public override string BlackName => "m";

        public override List<Position> GetAvailableMoves(ref ICell[,] cells, int srcX, int srcY, ICell exclude = null)
        {
            List<Position> availableMoves = new List<Position>();

            int x = srcX;
            int y = srcY;
            MoveCalculation.MoveDown(ref x, ref y);
            MoveCalculation.MoveDown(ref x, ref y);
            int xT = x;
            int yT = y;
            MoveCalculation.MoveRight(ref x, ref y);
            CheckCell(ref cells, ref availableMoves, x, y, exclude);
            MoveCalculation.MoveLeft(ref xT, ref yT);
            CheckCell(ref cells, ref availableMoves, xT, yT, exclude);


            x = srcX;
            y = srcY;
            MoveCalculation.MoveUp(ref x, ref y);
            MoveCalculation.MoveUp(ref x, ref y);
            int xR = x;
            int yR = y;
            MoveCalculation.MoveRight(ref x, ref y);
            CheckCell(ref cells, ref availableMoves, x, y, exclude);
            MoveCalculation.MoveLeft(ref xR, ref yR);
            CheckCell(ref cells, ref availableMoves, xR, yR, exclude);


            x = srcX;
            y = srcY;
            MoveCalculation.MoveRight(ref x, ref y);
            MoveCalculation.MoveRight(ref x, ref y);
            int xE = x;
            int yE = y;
            MoveCalculation.MoveUp(ref x, ref y);
            CheckCell(ref cells, ref availableMoves, x, y, exclude);
            MoveCalculation.MoveDown(ref xE, ref yE);
            CheckCell(ref cells, ref availableMoves, xE, yE, exclude);

            x = srcX;
            y = srcY;
            MoveCalculation.MoveLeft(ref x, ref y);
            MoveCalculation.MoveLeft(ref x, ref y);
            int xW = x;
            int yW = y;
            MoveCalculation.MoveUp(ref x, ref y);
            CheckCell(ref cells, ref availableMoves, x, y, exclude);
            MoveCalculation.MoveDown(ref xW, ref yW);
            CheckCell(ref cells, ref availableMoves, xW, yW, exclude);

            
            return availableMoves;
        }
    }

    public class Bishop : Piece
    {
        public override string WhiteName => "b";
        public override string BlackName => "v";

        public override List<Position> GetAvailableMoves(ref ICell[,] cells, int srcX, int srcY, ICell exclude = null)
        {
            List<Position> availableMoves = new List<Position>();

            LoopOnCells(MoveDirection.DownLeft, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCells(MoveDirection.DownRight, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCells(MoveDirection.UpLeft, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCells(MoveDirection.UpRight, ref cells, ref availableMoves, srcX, srcY, exclude);

            return availableMoves;
        }
    }

    public class Queen : Piece
    {
        public override string WhiteName => "q";
        public override string BlackName => "w";

        public override List<Position> GetAvailableMoves(ref ICell[,] cells, int srcX, int srcY, ICell exclude = null)
        {
            List<Position> availableMoves = new List<Position>();

            LoopOnCells(MoveDirection.DownLeft, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCells(MoveDirection.DownRight, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCells(MoveDirection.UpLeft, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCells(MoveDirection.UpRight, ref cells, ref availableMoves, srcX, srcY, exclude);



            LoopOnCells(MoveDirection.Left, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCells(MoveDirection.Right, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCells(MoveDirection.Up, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCells(MoveDirection.Down, ref cells, ref availableMoves, srcX, srcY, exclude);

            return availableMoves;
        }
    }

    public class King : Piece
    {
        public override string WhiteName => "k";
        public override string BlackName => "l";

        public override List<Position> GetAvailableMoves(ref ICell[,] cells, int srcX, int srcY, ICell exclude = null)
        {
            List<Position> availableMoves = new List<Position>();

            LoopOnCellsBySteps(1, MoveDirection.Down, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCellsBySteps(1, MoveDirection.DownLeft, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCellsBySteps(1, MoveDirection.DownRight, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCellsBySteps(1, MoveDirection.Left, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCellsBySteps(1, MoveDirection.Right, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCellsBySteps(1, MoveDirection.Up, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCellsBySteps(1, MoveDirection.UpLeft, ref cells, ref availableMoves, srcX, srcY, exclude);

            LoopOnCellsBySteps(1, MoveDirection.UpRight, ref cells, ref availableMoves, srcX, srcY, exclude);

            return availableMoves;
        }
    }

    public enum MoveDirection
    {
        Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight
    }

    public enum CheckCellResult
    {
        Friend, Opponent, OpponentKing, OpponentLocation
    }

}
