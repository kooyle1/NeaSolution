using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SolverScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int TTCapacity;
    [SerializeField] private float timeLimit;
    
    [Header("Monobehaviour Script References")]
    [SerializeField] private GameLogicScript gameLogic;
    [SerializeField] private LoggerScript logger;

    private TranspositionTable transpositionTable = new TranspositionTable();

    private float startTime;
    int[] moveOrder = {3, 2, 4, 1, 5, 0, 6};

    private void Start()
    {
        transpositionTable.SetSize(TTCapacity);
    }
    public int Negamax(ulong pos, ulong board, int depth, int alpha, int beta)
    {
        if (Time.realtimeSinceStartup - startTime >= timeLimit)
            return 0; 

        int alphaOrig = alpha;

        if (transpositionTable.TryLookup(pos, board, out TranspositionTable.TTEntry entry) && entry.depth >= depth) {
            if (entry.flag == TranspositionTable.TTFlag.EXACT) {
                return entry.value;
            }
            if (entry.flag == TranspositionTable.TTFlag.LOWERBOUND && entry.value >= beta) {
                return entry.value;
            }
            else if (entry.flag == TranspositionTable.TTFlag.UPPERBOUND && entry.value <= alpha) {
                return entry.value;
            }
        }

        if (gameLogic.CheckTie(board)) return 0;

        // Go through all moves and see if they lead to a win
        foreach (int i in moveOrder) {
            if (!gameLogic.CanPlayColumn(i, board)) {
                continue;
            }
            if (gameLogic.IsWinningMove(i, pos, board)) {
                return 1;
            }
        }
        if (depth == 0) return 0; 

        int value = -1;

        foreach (int i in moveOrder) {
            if (!gameLogic.CanPlayColumn(i, board)) continue;

            ulong newBoard = gameLogic.PlayMove(i, board);
            ulong newPos   = pos ^ newBoard;

            int score = -Negamax(newPos, newBoard, depth - 1, -beta, -alpha);
            value = Mathf.Max(value, score);
            alpha = Mathf.Max(alpha, value);
            if (alpha >= beta) break;
        }

        TranspositionTable.TTEntry newEntry;

        newEntry.value = value;
        if (value <= alphaOrig) {
            newEntry.flag = TranspositionTable.TTFlag.UPPERBOUND;
        }
        else if (value >= beta) {
            newEntry.flag = TranspositionTable.TTFlag.LOWERBOUND;
        }
        else {
            newEntry.flag = TranspositionTable.TTFlag.EXACT;
        }
        newEntry.depth = depth;
        transpositionTable.Store(pos, board, newEntry); 

        return value; 
    }

    public int ReturnBestMove(ulong pos, ulong board)
    {
        transpositionTable.Clear();
        
        startTime = Time.realtimeSinceStartup;

        int bestMove = 0;

        for (int depth = 1; depth <= 42; depth++) {
            int bestScore = -1;
            int currentBestMove = bestMove;

            foreach (int i in moveOrder) {
                logger.Log("hi");
                if (!gameLogic.CanPlayColumn(i, board)) {
                    continue;
                }


                ulong newBoard = gameLogic.PlayMove(i, board);
                ulong newPos = pos ^ newBoard;

                if (gameLogic.CheckWin(newPos)) {
                    return i;
                }
                int score = -Negamax(newPos, newBoard, depth - 1, -1, 1);
                if (Time.realtimeSinceStartup - startTime >= timeLimit) return bestMove;

                if (score > bestScore) {
                    bestScore = score;
                    currentBestMove = i;
                }
            }

            bestMove = currentBestMove;
        }

        return bestMove;
    }



}
public class TranspositionTable
{

    private Dictionary<(ulong, ulong), TTEntry> table;
    private int capacity;

    public void SetSize(int _capacity)
    {
        capacity = _capacity;
        table = new Dictionary<(ulong, ulong), TTEntry>(capacity);
    }

    public void Clear()
    {
        table.Clear();
    }

    public enum TTFlag
    {
        EXACT,
        UPPERBOUND,
        LOWERBOUND
    }

    public struct TTEntry
    {
        public int value;
        public int depth;
        public TTFlag flag;
    }

    public void Store(ulong pos, ulong board, TTEntry entry)
    {
        if (table.Count >= capacity) {
            var oldestKey = table.Keys.First();
            table.Remove(oldestKey);
        }

        table[(pos, board)] = entry;
    }

    public bool TryLookup(ulong pos, ulong board, out TTEntry entry)
    {
        return table.TryGetValue((pos, board), out entry);
    }

}

