using UnityEngine;

public class SolverScript : MonoBehaviour
{
    [Header("Monobehaviour Script References")]
    [SerializeField] private GameLogicScript gameLogic;
    [SerializeField] private LoggerScript logger;

    private float startTime;
    private float timeLimit = 3f;
    int[] moveOrder = {3, 2, 4, 1, 5, 0, 6};

    public int Negamax(ulong pos, ulong board, int depth, int alpha, int beta)
    {
        if (gameLogic.CheckTie(board)) return 0;

        // If the player who just moved won, current side-to-move is losing
        if (gameLogic.CheckWin(pos ^ board)) return -1;

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

        return value;
    }


    public int IterativeDeepeningBestMoveTimed(ulong pos, ulong board, float timeLimitSeconds)
    {
        startTime = Time.realtimeSinceStartup;
        timeLimit = timeLimitSeconds;

        int bestMove = 0;

        for (int depth = 1; depth <= 42; depth++) {
            int bestScore = -10000;
            int currentBestMove = bestMove;

            foreach (int i in moveOrder) {
                if (!gameLogic.CanPlayColumn(i, board)) continue;

                ulong newBoard = gameLogic.PlayMove(i, board);
                ulong newPos = pos ^ newBoard;

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
