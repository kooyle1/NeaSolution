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
        colCount = StaticData.cols;
        rowCount = StaticData.rows;
    }

    public void UpdateTieCheckMask()
    {
        ulong columnMask = ((1UL << StaticData.rows) - 1) << 1;
        ulong result = 0UL;

        for (int i = 0; i < StaticData.cols; i++) {
            result |= columnMask << (i * (StaticData.rows + 1));
        }

        tieCheckMask = result >> 1;
    }

    public ulong GetPos()
    {
        return currentPosition;
    }

    public ulong GetBoard()
    {
        return fullBoard;   
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
        fullBoard |= fullBoard + (1UL << (column * (StaticData.rows + 1)));
        currentPosition ^= fullBoard;
    }

    public void UndoMove(int column)
    {
        for (int row = StaticData.rows - 1; row >= 0; row--) {
            ulong bit = 1UL << (column * (StaticData.rows + 1) + row);
            if ((fullBoard & bit) == 0) continue;

            fullBoard ^= bit;                 
            if ((currentPosition & bit) != 0) 
                currentPosition ^= bit;

            
            currentPosition = fullBoard ^ currentPosition;
            return;
        }
    }

    /// <summary>
    ///  Plays move in the given column on the given board and returns the new board.
    /// </summary>
    public ulong PlayMove(int column, ulong board)
    {
        board |= board + (1UL << (column * (StaticData.rows + 1)));
        return board;
    }

    public bool IsWinningMove(int column, ulong pos, ulong board)
    {
        ulong newBoard = PlayMove(column, board);
        ulong playedBit = newBoard ^ board;
        ulong newPos = pos | playedBit;
        return CheckWin(newPos);

    }

    /// <summary>
    ///  Checks if column is full in the given board.
    /// </summary>
    public bool CanPlayColumn(int column, ulong board)
    {
        return (board & ((1UL << (StaticData.rows - 1)) << column * (StaticData.rows + 1))) == 0;
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
        checkMask = pos & (pos >> (StaticData.rows + 1));
        if ((checkMask & (checkMask >> (2 * (StaticData.rows + 1)))) != 0) {
            return true;
        }

        //Vertical
        checkMask = pos & (pos >> 1);
        if ((checkMask & (checkMask >> 2)) != 0) {
            return true;
        }

        //Diagonal (down-right/up-left)
        checkMask = pos & (pos >> StaticData.rows);
        if ((checkMask & (checkMask >> (2 * (StaticData.rows)))) != 0) {
            return true;
        }

        //Diagonal (up-right/down-left)
        checkMask = pos & (pos >> StaticData.rows + 2);
        if ((checkMask & (checkMask >> (2 * (StaticData.rows + 2)))) != 0) {
            return true;
        }

        return false;
    }

}
