using UnityEngine;

public class SolverScript : MonoBehaviour
{
    [Header("Monobehaviour Script References")]
    [SerializeField] private GameLogicScript gameLogic;
    [SerializeField] private LoggerScript logger;

    //colour 1 = red(p1), colour -1 = yellow(p2)
    public int Negamax(ulong pos, ulong board, int depth, int alpha, int beta, int color)
    {
        if (gameLogic.CheckTie(board)) {
            return 0;
        }

        if (gameLogic.CheckWin(pos)) { //Assuming solver is playing as red
            if (color == 1) {
                return 1;
            }
            return -1;
        }

        int value = -10000;
        
        for (int i = 0; i < 7; i++) {
            if (!gameLogic.CanPlayColumn(i, board)){
                continue;
            }
            ulong newBoard = gameLogic.PlayMove(i, board);
            ulong newPos = pos ^ newBoard;
            value = Mathf.Max(value, -Negamax(newPos, newBoard, depth, alpha, beta, -color));
            alpha = Mathf.Max(alpha, value);
            if (alpha >= beta) {
                break;
            }
        }

        return value;
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
            int score = Negamax(newPos, newBoard, 0,  -100, 100, 1); //Assuming we're playing red
            if (score > bestScore) {
                    bestScore = score;
                    bestMove = i;
                }
        }

        
        return bestMove;

    }
}
