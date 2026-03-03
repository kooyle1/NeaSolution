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

    /// <summary>
    ///   Recursive search function to find the score of a position in a given alpha-beta window
    /// </summary>
    private int negamax(ulong pos, ulong board, int alpha, int beta, int depth)
    {
        exploredNodes++;

        if (bitboard.CheckTie(board)) {
            return 0;
        }

        int alphaOrig = alpha; //Store original alpha value for storing in transposition table
        ulong key = transpositionTable.MakeKey(pos, board);
        //Transposition table lookup
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

        //Check if this position is a winning position for the current player
        foreach (int i in moveOrder) {
            if (!bitboard.CanPlayColumn(i, board)) {
                continue;
            }
            if (bitboard.IsWinningMove(i, pos, board)) {
                return (depth + 1) / 2; //Current player wins next move
            }
        }

        //Tighten the lower bound
        int min = -depth / 2;
        if (alpha < min) {
            if (min >= beta) { 
                return min; //Will lead to alpha-beta cutoff, so cutoff here by returning min
            }
            alpha = min;
        }

        //Tighten the upper bound
        int max = (depth + 1) / 2;
        if (beta > max) {
            if (alpha >= max) {
                return max; //Will lead to alpha-beta cutoff, so cutoff here by returning max
            }
            beta = max;
        }

        int score = -200; //-200 used instead of -inf
        foreach (int move in moveOrder) {
            if (!bitboard.CanPlayColumn(move, board)) {
                continue;
            }

            ulong newBoard = bitboard.PlayMove(move, board);
            ulong newPos = pos ^ newBoard;

            //Solve this next position recursively.
            //Uses the zero-sum property of connect 4: Score(current player) = -Score(opposite player)
            int value = -negamax(newPos, newBoard, -beta, -alpha, depth - 1);
            score = Math.Max(score, value);
            alpha = Math.Max(alpha, score);

            //Alpha-beta cutoff
            if (alpha >= beta)
                break;
        }

        //Store position in the transposition table
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
    ///   Returns the score of a position using Negamax and zero-window search.   
    /// </summary>
    private int solve(ulong pos, ulong board)
    {
        //Lookup position in opening book
        //Not needed in negamax since we look it up here before searching
        if (usingOpeningBook && book.TryGetScore(bitboard.GetKey(pos, board), out sbyte bookScore)) {
            return (int)bookScore;
        }

        //Initial binary search window
        int remaining = bitboard.GetRemainingMoves(board);
        int min = -remaining / 2;
        int max = (remaining + 1) / 2;

        //Binary search to find the true score
        while (min < max) {
            int mid = min + (max - min) / 2;
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
    ///   Returns an array of scores corresponding to the score in each column. 
    ///   A positive score x means this move leads to a win with the player's x last coin. (1 = last coin, 2 = 2nd to last coin, etc.)
    ///   A negative score x means this move leads to the opposite player will win with their x last coin.
    ///   A score of 0 means this move leads to a tie.
    ///   A score of -200 means the column is full.
    /// </summary>
    public int[] ReturnScores(ulong pos, ulong board)
    {
        transpositionTable.Clear();
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
    ///   If there are multiple columns with the best score, chooses the leftmost column.
    /// </summary>
    public int ChooseBestMove(int[] scores)
    {
        int[] sorted = (int[])scores.Clone();
        int[] unsorted = (int[])scores.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);

        return Array.IndexOf(unsorted, sorted[0]);
    }

    /// <summary>
    ///   Returns a move by modelling scores as a probability distribution where higher scores are more likely.
    ///   Higher weight causes the probablity distribution to even out, causing bad moves to become more likely.
    ///   I.e, higher weight = smarter AI.
    /// </summary>
    public int ChooseWeightedMove(int[] scores, int weight)
    {
        double[] distribution = new double[scores.Length];
        double[] numerator = new double[scores.Length];
        double denominator = 0D;
        //Loop through scores and create numerator and denominator
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

        //Loop through each numerator and divide by the denominator
        for (int i = 0; i < numerator.Length; i++) {
            double score = numerator[i];
            if (score <= -100) {
                distribution[i] = 0; //Should not be possible to pick an unplayable column
            }
            else {
                distribution[i] = score / denominator;
            }
        }

        System.Random rand = new System.Random();
        double randint = rand.NextDouble();
        double cumulative = 0;
        int weightedMove = 0;

        //Choose a move from the distribution
        for (int i = 0; i < distribution.Length; i++) {
            double score = distribution[i];
            cumulative += score;
            if (cumulative >= randint) {
                weightedMove = i;
                break;
            }
        }

        return weightedMove;
    }
}

public class TranspositionTable
{

    private TTEntry[] table;
    private int capacity;

    /// <summary>
    ///   A large prime number should be used to minimise collisions. Default param is the optimal capacity.
    ///   Memory usage (MB) is roughly = (capacity * 24)/(1024^2)
    /// </summary>
    public TranspositionTable(int _capacity = (1 << 23) + 9)
    {
        capacity = _capacity;
        table = new TTEntry[capacity];
    }

    /// <summary>
    ///   Creates a transposition table key from a pos, board combination.
    /// </summary>
    public ulong MakeKey(ulong pos, ulong board)
    {
        ulong key = pos;
        key ^= board * 0x9E3779B97F4A7C15; // golden ratio * 2^64
        return key;
    }

    /// <summary>
    ///   Clear the transposition table.
    /// </summary>
    public void Clear()
    {
        Array.Clear(table, 0, table.Length);
    }

    /// <summary>
    ///   Store an entry in the transposition table.
    /// </summary>
    public void Store(TTEntry entry)
    {
        int index = (int)(entry.key % (ulong)capacity);
        table[index] = entry;
    }

    /// <summary>
    ///   Looks up a position in the transposition table.
    ///   Requires a key which should be created using the transposition table's MakeKey function.
    /// </summary>
    public bool TryLookup(ulong key, out TTEntry entry)
    {
        int index = (int)(key % (ulong)capacity);
        entry = table[index];
        return entry.key == key;
    }
}

public class OpeningBook
{
    public Dictionary<ulong, sbyte> Map = new();

    /// <summary>
    ///   Load the opening book from storage manager.
    /// </summary>
    public void Load()
    {
        StorageManager.instance.LoadBook(book => {
            Map = book;
        });
    }

    /// <summary>
    ///   Looks up a position in the opening book.
    ///   Requires a key which should be created from the bitboard's GetKey function.
    /// </summary>
    public bool TryGetScore(ulong key, out sbyte score)
    {
        return Map.TryGetValue(key, out score);
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
    //Total size of an entry = 20 bytes, so rounds to 24 bytes.
    public ulong key; //8 bytes
    public int value; //4 bytes
    public int depth; //4 bytes
    public TTFlag flag; //4 bytes
}

