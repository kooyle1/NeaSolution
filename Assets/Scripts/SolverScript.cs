using UnityEngine;

public class SolverScript : MonoBehaviour
{
    [Header("Monobehaviour Script References")]
    [SerializeField] private GameLogicScript gameLogic;
    [SerializeField] private LoggerScript logger;

    public int AlphaBeta(ulong pos, ulong board, int depth, int alpha, int beta, bool maximizing)
    {
        if (gameLogic.CheckTie(board)) {
            return 0;
        }

        if (gameLogic.CheckWin(pos)) { //Assuming solver is playing as red
            if (maximizing) {
                return 1;
            }
            return -1;
        }

        if (maximizing) {
            int bestScore = -100;
            for (int i = 0; i < 7; i++) {
                if (!gameLogic.CanPlayColumn(i)) {
                    continue;
                }           
                ulong newBoard = gameLogic.PlayMove(i, board);
                ulong newPos = pos ^ newBoard;
                bestScore = Mathf.Max(bestScore, AlphaBeta(newPos, newBoard, depth, alpha, beta, false));

                if (bestScore >= beta) {
                    break;
                }

                alpha = Mathf.Max(alpha, bestScore);
            }

            return bestScore;
        }

        else {
            int bestScore = 100;
            for (int i = 0; i < 7; i++) {
                if (!gameLogic.CanPlayColumn(i, board)) {
                    continue;
                }
                ulong newBoard = gameLogic.PlayMove(i, board);
                ulong newPos = pos ^ newBoard;
                bestScore = Mathf.Min(bestScore, AlphaBeta(newPos, newBoard, depth, alpha, beta, true));

                if (bestScore <= alpha) {
                    break;
                }

                beta = Mathf.Min(beta, bestScore);
            }

            return bestScore;
        }
    }

    public int ReturnBestMove(ulong pos, ulong board)
    {
        int bestScore = -100;
        int bestMove = 0;

        for (int i = 0; i < 7; i++) {
            if (gameLogic.CanPlayColumn(i, board)) {
                continue;
            }

            ulong newBoard = gameLogic.PlayMove(i, board);
            ulong newPos = pos ^ newBoard;
            int score = AlphaBeta(newPos, newBoard, 13,  -100, 100, true);
            if (score > bestScore) {
                    bestScore = score;
                    bestMove = i;
                }
        }

        
        return bestMove;

    }
}
