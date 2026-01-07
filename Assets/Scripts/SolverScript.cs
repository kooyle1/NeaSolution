using JetBrains.Annotations;
using UnityEngine;

public class SolverScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int TTCapacity;
    
    [Header("Monobehaviour Script References")]
    [SerializeField] private GameLogicScript gameLogic;
    [SerializeField] private LoggerScript logger;
    [SerializeField] private TranspositionTableScript transpositionTable;

    private float startTime;
    private float timeLimit = 2f;
    int[] moveOrder = {3, 2, 4, 1, 5, 0, 6};

    private void Start()
    {
        transpositionTable.SetSize(TTCapacity);
    }

    public int Negamax(ulong pos, ulong board, int depth, int alpha, int beta)
    {
        if (Time.realtimeSinceStartup - startTime >= timeLimit)
            return 0; // TODO heuristic

        int alphaOrig = alpha;

        if (transpositionTable.TryLookup(pos, board, out TranspositionTableScript.TTEntry entry) && entry.depth >= depth) {
            if (entry.flag == TranspositionTableScript.TTFlag.EXACT) return entry.value;
            if (entry.flag == TranspositionTableScript.TTFlag.LOWERBOUND) alpha = Mathf.Max(alpha, entry.value);
            else if (entry.flag == TranspositionTableScript.TTFlag.UPPERBOUND) beta = Mathf.Min(beta, entry.value);
            if (alpha >= beta) return entry.value;
        }

        if (gameLogic.CheckTie(board)) return 0;

        // If the player who just moved won, current side-to-move is losing
        if (gameLogic.CheckWin(pos ^ board)) return -1000 + (42 - depth);

        if (depth == 0) return 0; // TODO heuristic

        int value = -10000;

        foreach (int i in moveOrder) {
            if (!gameLogic.CanPlayColumn(i, board)) continue;

            ulong newBoard = gameLogic.PlayMove(i, board);
            ulong newPos   = pos ^ newBoard;

            int score = -Negamax(newPos, newBoard, depth - 1, -beta, -alpha);
            value = Mathf.Max(value, score);
            alpha = Mathf.Max(alpha, value);
            if (alpha >= beta) break;
        }

        TranspositionTableScript.TTEntry newEntry;

        newEntry.value = value;
        if (value <= alphaOrig) {
            newEntry.flag = TranspositionTableScript.TTFlag.UPPERBOUND;
        }
        else if (value >= beta) {
            newEntry.flag = TranspositionTableScript.TTFlag.LOWERBOUND;
        }
        else {
            newEntry.flag = TranspositionTableScript.TTFlag.EXACT;
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
            int bestScore = -10000;
            int currentBestMove = bestMove;

            foreach (int i in moveOrder) {
                logger.Log("hi");
                if (!gameLogic.CanPlayColumn(i, board)) continue;


                ulong newBoard = gameLogic.PlayMove(i, board);
                ulong newPos = pos ^ newBoard;

                if (gameLogic.CheckWin(newPos)) {
                    return i;
                }
                int score = -Negamax(newPos, newBoard, depth-1, -10000, 10000);
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
