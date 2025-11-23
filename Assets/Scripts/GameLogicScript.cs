using UnityEngine;

public class GameLogicScript : MonoBehaviour
{
    private int colCount = 7;
    private int rowCount = 6;
    private ulong currentPosition = 0UL;
    private ulong fullBoard = 0UL;
    private ulong tieCheckMask;

    private void Start()
    {
        UpdateTieCheckMask();
    }

    private void UpdateTieCheckMask()
    {
        ulong columnMask = ((1UL << rowCount) - 1) << 1;
        ulong result = 0UL;

        for (int i = 0; i < colCount; i++) {
            result |= columnMask << (i * (rowCount + 1));
        }

        tieCheckMask = result >> 1;
        Debug.Log(tieCheckMask);
    }

    /// <summary>
    ///  Checks if column is full in the current board.
    /// </summary>
    public bool CanPlayColumn(int column) => CanPlayColumn(column, fullBoard);

    /// <summary>
    ///  Checks if a win has ocurred in the current position.
    /// </summary>
    public bool CheckWin() => CheckWin(currentPosition);

    /// <summary>
    ///  Checks if a tie has ocurred in the current board.
    /// </summary>
    public bool CheckTie() => CheckTie(fullBoard);

    /// <summary>
    ///  Resets board to be empty.
    /// </summary>
    public void ResetBoard()
    {
        currentPosition = 0UL;
        fullBoard = 0Ul;
    }

    /// <summary>
    ///  Updates the object's internal variable tracking the position of each player in the current game.
    /// </summary>
    public void PlayMove(int column)
    {
        fullBoard |= fullBoard + (1UL << (column * (rowCount + 1)));
        currentPosition ^= fullBoard;
    }

    /// <summary>
    ///  Plays move in the given column on the given board and returns the new board.
    /// </summary>
    public ulong PlayMove(int column, ulong board)
    {
        board |= board + (1UL << (column * (rowCount + 1)));
        return board;
    }

    /// <summary>
    ///  Checks if column is full in the given board.
    /// </summary>
    public bool CanPlayColumn(int column, ulong board)
    {
        return (board & ((1UL << (rowCount - 1)) << column * (rowCount + 1))) == 0;
    }

    /// <summary>
    ///  Checks if a tie has ocurred in the given board.
    /// </summary>
    public bool CheckTie(ulong board)
    {
        return board == tieCheckMask;
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
