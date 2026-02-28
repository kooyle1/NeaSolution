using System;
using UnityEngine;

public class BitboardManager : MonoBehaviour
{
    private ulong currentPosition = 0UL;
    private ulong fullBoard = 0UL;
    private ulong tieCheckMask;

    private string list = "{";

    private void Start()
    {
        UpdateTieCheckMask();
    }

    public void AddPosBoard()
    {
        list += $"({currentPosition}, {fullBoard}), ";
        Debug.Log(list);
    }

    /// <summary>
    ///  Updates the mask used to check for ties to fit board size.
    /// </summary>
    public void UpdateTieCheckMask()
    {
        ulong columnMask = ((1UL << GameConfig.rows) - 1) << 1;
        ulong result = 0UL;

        for (int i = 0; i < GameConfig.cols; i++) {
            result |= columnMask << (i * (GameConfig.rows + 1));
        }

        tieCheckMask = result >> 1;
    }

    /// <summary>
    ///  Returns number of played moves the given board.
    /// </summary>
    public int GetMoves(ulong board)
    {
        int count = 0;
        while (board != 0) {
            board &= board - 1;
            count++;
        }
        return count;
    }

    /// <summary>
    ///  Returns number of remaining moves the given board.
    /// </summary>
    public int GetRemainingMoves(ulong board)
    {
        return GameConfig.rows * GameConfig.cols - GetMoves(board);
    }

    /// <summary>
    ///  Returns ulong of the position for the current player.
    /// </summary>
    public ulong GetPos()
    {
        return currentPosition;
    }

    /// <summary>
    ///  Returns ulong of the entire bitboard.
    /// </summary>
    public ulong GetBoard()
    {
        return fullBoard;   
    }

    /// <summary>
    ///  Checks if column is full in the current board.
    /// </summary>
    public bool CanPlayColumn(int column) => CanPlayColumn(column, fullBoard);

    /// <summary>
    ///  Checks if column is full in the given board.
    /// </summary>
    public bool CanPlayColumn(int column, ulong board)
    {
        return (board & ((1UL << (GameConfig.rows - 1)) << column * (GameConfig.rows + 1))) == 0;
    }

    /// <summary>
    ///  Checks if a win has ocurred in the current position.
    /// </summary>
    public bool CheckWin() => CheckWin(currentPosition);

    /// <summary>
    ///  Checks if a win has ocurred in the given position. 
    /// </summary>
    public bool CheckWin(ulong pos)
    {
        ulong checkMask;

        //Horizontal 
        checkMask = pos & (pos >> (GameConfig.rows + 1));
        if ((checkMask & (checkMask >> (2 * (GameConfig.rows + 1)))) != 0) {
            return true;
        }

        //Vertical
        checkMask = pos & (pos >> 1);
        if ((checkMask & (checkMask >> 2)) != 0) {
            return true;
        }

        //Diagonal (down-right/up-left)
        checkMask = pos & (pos >> GameConfig.rows);
        if ((checkMask & (checkMask >> (2 * (GameConfig.rows)))) != 0) {
            return true;
        }

        //Diagonal (up-right/down-left)
        checkMask = pos & (pos >> GameConfig.rows + 2);
        if ((checkMask & (checkMask >> (2 * (GameConfig.rows + 2)))) != 0) {
            return true;
        }
        return false;
    }

    /// <summary>
    ///  Checks if a tie has ocurred in the current board.
    /// </summary>
    public bool CheckTie() => CheckTie(fullBoard);

    /// <summary>
    ///  Checks if a tie has ocurred in the given board.
    /// </summary>
    public bool CheckTie(ulong board)
    {
        return board == tieCheckMask;
    }

    /// <summary>
    ///  Resets current board and position to be empty.
    /// </summary>
    public void ResetBitboard()
    {
        currentPosition = 0UL;
        fullBoard = 0Ul;
    }

    /// <summary>
    ///  Plays a move in the bitboard in the given column.
    /// </summary>
    public void PlayMove(int column)
    {
        //  Debug.Log($"Board before: {fullBoard}");
        //  Debug.Log($"Pos before: {currentPosition}");

        fullBoard |= fullBoard + (1UL << (column * (GameConfig.rows + 1)));
        currentPosition ^= fullBoard;

        //Debug.Log($"Board after: {fullBoard}");
       // Debug.Log($"Pos after: {currentPosition}");
    }

    /// <summary>
    ///  Plays move in the given column on the given board and returns the new board.
    /// </summary>
    public ulong PlayMove(int column, ulong board)
    {
        board |= board + (1UL << (column * (GameConfig.rows + 1)));
        return board;
    }
    
    /// <summary>
    ///  Removes a move from the bitboard in the given column.
    /// </summary>
    public void UndoMove(int column)
    {
        for (int row = GameConfig.rows - 1; row >= 0; row--) {
            ulong bit = 1UL << (column * (GameConfig.rows + 1) + row);
            if ((fullBoard & bit) == 0) continue;

            fullBoard ^= bit;                 
            if ((currentPosition & bit) != 0) 
                currentPosition ^= bit;

            
            currentPosition = fullBoard ^ currentPosition;
            return;
        }
    }

    /// <summary>
    ///   Simulates a move in the given column and checks if it wins.
    /// </summary>
    public bool IsWinningMove(int column, ulong pos, ulong board)
    {
        ulong newBoard = PlayMove(column, board);
        ulong newPos = pos ^ newBoard;
        return CheckWin(newPos);
    }

    private ulong ColumnMask(int col)
    {
        return ((1UL << (GameConfig.rows + 1)) - 1)
               << (col * (GameConfig.rows + 1));
    }

    /// <summary>
    ///   Return key for the opening book.
    /// </summary>
    public ulong GetKey(ulong pos, ulong board)
    {
        pos ^= board;
        ulong key = pos + board;

        var (mirroredPos, mirroredMask) = GetMirroredBitmasks(pos, board);
        ulong mirroredKey = mirroredPos + mirroredMask;

        return Math.Min(key, mirroredKey);
    }

    private (ulong, ulong) GetMirroredBitmasks(ulong pos, ulong board)
    {
        ulong mirroredPosition = 0;
        ulong mirroredMask = 0;
        int centre = GameConfig.cols / 2;

        for (int col = 0; col < centre; col++) {
            int mirroredCol = GameConfig.cols - 1 - col;
            int shift = (mirroredCol - col) * (GameConfig.rows + 1);

            ulong colMask = ColumnMask(col);
            ulong mirroredColMask = ColumnMask(mirroredCol);

            mirroredPosition |=
                ((pos & colMask) << shift) |
                ((pos & mirroredColMask) >> shift);

            mirroredMask |=
                ((board & colMask) << shift) |
                ((board & mirroredColMask) >> shift);
        }

        if (GameConfig.cols % 2 == 1) {
            mirroredPosition |= pos & ColumnMask(3);
            mirroredMask |= board & ColumnMask(3);
        }

        return (mirroredPosition, mirroredMask);
    }

}
