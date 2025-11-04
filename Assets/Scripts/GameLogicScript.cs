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

    /// <summary>
    ///  Updates the object's internal variable tracking the position of each player in the current game.
    /// </summary>
    public void PlayMove(int col)
    {
        mask |= mask + (1UL << (col * (rowCount + 1)));
        currentPosition ^= mask;
        moveCount++;
    }

    /// <summary>
    ///  Checks if a win has ocurred in the current position.
    /// </summary>
    public bool CheckWin() => CheckWin(currentPosition);

    public bool CheckTie()
    {
        if (moveCount == 42) {
            return true;
        }
        return false;
    }

    /// <summary>
    ///  Checks if a win has ocurred in the given position. 
    /// </summary>
    public bool CheckWin(ulong pos)
    {
        ulong checkMask;

        //Horizontal 
        checkMask = pos & (pos >> (rowCount + 1));
        if ((checkMask & (checkMask >> (2 * (rowCount + 1)))) != 0) {
            Debug.Log("horizontal");
            return true;
        }

        //Vertical
        checkMask = pos & (pos >> 1);
        if ((checkMask & (checkMask >> 2)) != 0) {
            Debug.Log("vertical");
            return true;
        }

        //Diagonal (down-right/up-left)
        checkMask = pos & (pos >> rowCount);
        if ((checkMask & (checkMask >> (2 * (rowCount)))) != 0) {
            Debug.Log("down right");
            return true;
        }

        //Diagonal (up-right/down-left)
        checkMask = pos & (pos >> rowCount + 2);
        if ((checkMask & (checkMask >> (2 * (rowCount + 2)))) != 0) {
            Debug.Log("up right");
            return true;
        }

        return false;
    }

}
