using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class Solver : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int TTCapacity;
    [SerializeField] bool usingOpeningBook;

    [Header("Monobehaviour Script References")]
    [SerializeField] private BitboardManager bitboard;
    [SerializeField] private UnityEngine.Logger logger;

    private TranspositionTable transpositionTable;
    private OpeningBook book = new OpeningBook();
    private Stopwatch stopwatch = new Stopwatch();

    public double timeTaken { get; private set; }
    public int exploredNodes { get; private set; }

    private int[] moveOrder = { 3, 2, 4, 1, 5, 0, 6 };

    private void Start()
    {
        transpositionTable = new TranspositionTable(TTCapacity);
        if (usingOpeningBook) {
            book.Load();
        }
    }

    private int negamax(ulong pos, ulong board, int alpha, int beta, int depth)
    {
        exploredNodes++;

        if (bitboard.CheckTie(board)) {
            return 0;
        }

        int alphaOrig = alpha;
        ulong key = transpositionTable.MakeKey(pos, board);

        if (transpositionTable.TryLookup(key, out TTEntry entry) && entry.depth >= depth) {
            if (entry.flag == TTFlag.EXACT) {
                return entry.value;
            }
            else if (entry.flag == TTFlag.LOWERBOUND && entry.value >= beta) {
                return entry.value;
            }
            else if (entry.flag == TTFlag.UPPERBOUND && entry.value <= alpha) {
                return entry.value;
            }
        }

        foreach (int i in moveOrder) {
            if (!bitboard.CanPlayColumn(i, board)) {
                continue;
            }
            if (bitboard.IsWinningMove(i, pos, board)) {
                return (depth + 1) / 2; //I win next move
            }
        }

        int min = -depth / 2;
        if (alpha < min) {
            if (min >= beta) {
                return min;
            }
            alpha = min;
        }

        int max = (depth + 1) / 2;
        if (beta > max) {
            if (alpha >= max) {
                return max;
            }
            beta = max;
        }

        int score = -100;
        foreach (int move in moveOrder) {
            if (!bitboard.CanPlayColumn(move, board)) {
                continue;
            }

            ulong newBoard = bitboard.PlayMove(move, board);
            ulong newPos = pos ^ newBoard;

            int value = -negamax(newPos, newBoard, -beta, -alpha, depth - 1);
            score = Math.Max(score, value);
            alpha = Math.Max(alpha, score);

            if (alpha >= beta)
                break;
        }

        TTEntry newEntry;

        newEntry.value = score;
        if (score <= alphaOrig) {
            newEntry.flag = TTFlag.UPPERBOUND;
        }
        else if (score >= beta) {
            newEntry.flag = TTFlag.LOWERBOUND;
        }
        else {
            newEntry.flag = TTFlag.EXACT;
        }
        newEntry.depth = depth;
        newEntry.key = key;
        transpositionTable.Store(newEntry);

        return score;
    }

    /// <summary>
    ///   Returns the score of a position.
    /// </summary>
    private int solve(ulong pos, ulong board)
    {
        int remaining = bitboard.GetRemainingMoves(board);
        int min = -remaining / 2;
        int max = (remaining + 1) / 2;

        if (usingOpeningBook && book.TryGetScore(bitboard.GetKey(pos, board), out sbyte bookScore)) {
            return (int)bookScore;
        }

        while (min < max) {
            int mid = min + (max - min) / 2;
            if (mid <= 0 && min / 2 < mid) {
                mid = min / 2;
            }
            else if (mid >= 0 && max / 2 > mid) {
                mid = max / 2;
            }

            int score = negamax(pos, board, mid, mid + 1, remaining);

            if (score <= mid) {
                max = score;
            }
            else {
                min = score;
            }
        }

        return min;
    }


    /// <summary>
    ///   Returns an array of scores corresponding to the score in each column. Index 0 = Column 0, Index 1 = Column 1, etc. A large negative score <=-200 represents a full column.
    /// </summary>
    public int[] ReturnScores(ulong pos, ulong board)
    {
        stopwatch.Restart();
        exploredNodes = 0;
        int[] scores = { -200, -200, -200, -200, -200, -200, -200 };
        foreach (int i in moveOrder) {
            if (!bitboard.CanPlayColumn(i, board)) {
                continue;
            }

            if (bitboard.IsWinningMove(i, pos, board)) {
                scores[i] = (GameConfig.rows * GameConfig.cols - bitboard.GetMoves(board) + 1) / 2;
                continue;
            }

            ulong newBoard = bitboard.PlayMove(i, board); //AI player plays move 
            ulong newPos = pos ^ newBoard; //Now AI player's pos

            int score = -solve(newPos, newBoard);
            scores[i] = score;
        }

        stopwatch.Stop();
        timeTaken = stopwatch.Elapsed.TotalMilliseconds;
        return scores;
    }

    /// <summary>
    ///   Returns the best column to play in from a given set of scores.
    /// </summary>
    public int ChooseBestMove(int[] scores)
    {
        int[] sorted = (int[])scores.Clone();
        int[] unsorted = (int[])scores.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);

        return Array.IndexOf(unsorted, sorted[0]);
    }

    public int ChooseWeightedMove(int[] scores, int weight)
    {
        double[] distribution = new double[scores.Length];
        double[] numerator = new double[scores.Length];
        double denominator = 0D;
        for (int i = 0; i < scores.Length; i++) {
            int score = scores[i];
            if (score <= -100) {
                numerator[i] = score;
            }
            else {
                numerator[i] = Mathf.Exp(score / weight);
                denominator += Mathf.Exp(score / weight);
            }
        }

        for (int i = 0; i < numerator.Length; i++) {
            double score = numerator[i];
            if (score <= -100) {
                distribution[i] = 0;
            }
            else {
                distribution[i] = score / denominator;
            }
        }

        System.Random rand = new System.Random();
        double randint = rand.NextDouble();
        double cumulative = 0;

        for (int i = 0; i < distribution.Length; i++) {
            double score = distribution[i];
            cumulative += score;
            if (cumulative > randint) {
                return i;
            }
        }

        return GameConfig.cols - 1;

    }
}

public class TranspositionTable
{

    private TTEntry[] table;
    private int capacity;
    private BitboardManager bitboard;

    public TranspositionTable(int power)
    {
        capacity = (1 << power) + 9;
        table = new TTEntry[capacity];
    }

    public ulong MakeKey(ulong pos, ulong board)
    {
        ulong key = pos;
        key ^= board * 0x9E3779B97F4A7C15; // golden ratio * 2^64
        return key;
    }

    public void Clear()
    {
        Array.Clear(table, 0, table.Length);
    }   

    public void Store(TTEntry entry)
    {
        int index = (int)(entry.key % (ulong)capacity);
        table[index] = entry;
    }

    public bool TryLookup(ulong key, out TTEntry entry)
    {
        int index = (int)(key % (ulong)capacity);
        entry = table[index];
        return entry.key == key;
    }
}
public enum TTFlag
{
    EXACT,
    UPPERBOUND,
    LOWERBOUND
}

public struct TTEntry
{
    public ulong key;
    public int value;
    public int depth;
    public TTFlag flag;
}

public class OpeningBook
{
    public Dictionary<ulong, sbyte> Map = new();

    public void Load()
    {
        StorageManager.instance.LoadBook(book => {
            Map = book;
        });
    }

    public bool TryGetScore(ulong key, out sbyte score)
    {
        return Map.TryGetValue(key, out score);
    }

}

