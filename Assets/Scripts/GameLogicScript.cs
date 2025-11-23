using UnityEngine;

public class GameLogicScript : MonoBehaviour
{
    private int colCount = 7;
    private int rowCount = 6;
    private int moveCount = 0;
    private ulong currentPosition = 0UL;
    private ulong fullBoard = 0UL;

    /// <summary>
    ///  Resets board to be empty.
    /// </summary>
    public void ResetBoard()
    {
        currentPosition = 0UL;
        fullBoard = 0Ul;
        moveCount = 0;
    }

    /// <summary>
    ///  Updates the object's internal variable tracking the position of each player in the current game.
    /// </summary>
    public void PlayMove(int column)
    {
        fullBoard |= fullBoard + (1UL << (column * (rowCount + 1)));
        currentPosition ^= fullBoard;
        moveCount++;
    }

    /// <summary>
    ///  Checks if column is full.
    /// </summary>
    public bool CanPlayColumn(int column)
    {
        return (fullBoard & ((1UL << (rowCount - 1)) << column * (rowCount + 1))) == 0;
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
            return true;
        }

        //Vertical
        checkMask = pos & (pos >> 1);
        if ((checkMask & (checkMask >> 2)) != 0) {
            return true;
        }

        //Diagonal (down-right/up-left)
        checkMask = pos & (pos >> rowCount);
        if ((checkMask & (checkMask >> (2 * (rowCount)))) != 0) {
            return true;
        }

        //Diagonal (up-right/down-left)
        checkMask = pos & (pos >> rowCount + 2);
        if ((checkMask & (checkMask >> (2 * (rowCount + 2)))) != 0) {
            return true;
        }

        return false;
    }

}
