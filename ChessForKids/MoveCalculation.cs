using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace idemery
{
    static public class MoveCalculation
    {
        #region Moves Direction Caculations
        static public bool MoveRight(ref int x, ref int y)
        {
            x++;
            return true;
        }

        static public bool MoveLeft(ref int x, ref int y)
        {
            x--;
            return true;
        }

        static public bool MoveUp(ref int x, ref int y)
        {
            y++;
            return true;
        }

        static public bool MoveDown(ref int x, ref int y)
        {
            y--;
            return true;
        }

        static public bool MoveRightUp(ref int x, ref int y)
        {
            x++;
            y++;
            return true;
        }

        static public bool MoveLeftUp(ref int x, ref int y)
        {
            x--;
            y++;
            return true;
        }

        static public bool MoveRightDown(ref int x, ref int y)
        {
            x++;
            y--;
            return true;
        }

        static public bool MoveLeftDown(ref int x, ref int y)
        {
            x--;
            y--;
            return true;
        }
        #endregion
    }
}
