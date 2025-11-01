using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameLogicScript : MonoBehaviour
{
    private int colCount = 7;
    private int rowCount = 6;
    private int moveCount = 0;
    private ulong currentPosition = 0UL;
    private ulong mask = 0UL;

    private ulong BottomMask(int col)
    {
        return 1UL << (col *  rowCount);
    }

    public void PlayMove(int col)
    {
        mask |= mask + BottomMask(col);
        currentPosition ^= mask;
        moveCount++;
    }

    public bool CheckWin() => CheckWin(currentPosition);

    public bool CheckTie()
    {
        if (moveCount == 42) {
            return true;
        }
        return false;
    }

    public bool CheckWin(ulong pos)
    {
        ulong checkMask;

        //Horizontal 
        checkMask = pos & (pos >> (rowCount));
        if ((checkMask & (checkMask >> (2 * (rowCount)))) != 0)
            return true;

        //Vertical
        checkMask = pos & (pos >> 1);
        if ((checkMask & (checkMask >> 2)) != 0)
            return true;

        //Diagonal (down-right/up-left)
        checkMask = pos & (pos >> rowCount - 1);
        if ((checkMask & (checkMask >> (2 * (rowCount - 1)))) != 0)
            return true;

        //Diagonal (up-right/down-left)
        checkMask = pos & (pos >> (rowCount + 1));
        if ((checkMask & (checkMask >> (2 * (rowCount + 1)))) != 0)
            return true;

        return false;
    }

}
