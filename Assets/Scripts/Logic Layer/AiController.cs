using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class AiController : MonoBehaviour
{
    [Header("MonoBehaviour Script References")]
    [SerializeField] private BitboardManager bitboard;
    [SerializeField] private Solver solver;
    [SerializeField] Logger logger;

    public bool aiThinking { get; private set; } = false;
    public int[] scores { get; private set; }
    private Task<int[]> aiTask;
    private Action<int[]> callback;

    private void Update()
    {
        if (!aiThinking || aiTask == null) return;

        if (aiTask.IsCompleted) {
            aiThinking = false;
            int[] scores = aiTask.Result;
            aiTask = null;

            callback?.Invoke(scores);
        }
    }

    public void StartSolving(Action<int[]> onMoveReady) => StartSolving(bitboard.GetPos(), bitboard.GetBoard(), onMoveReady);
    
    public void StartSolving(ulong pos, ulong board, Action<int[]> onMoveReady)
    {
        if (aiThinking) return;

        aiThinking = true;
        callback = onMoveReady;

        aiTask = Task.Run(() =>
        {
            return solver.ReturnScores(pos, board);
        });
    }

    public int ReturnBestMove(int[] scores)
    {
        return solver.ChooseBestMove(scores);
    }

    public int ReturnMoveByDifficulty(int[] scores)
    {
        IAiStrategy aiStrategy;

        switch (GameConfig.aiDifficulty) {
            case "easy":
                aiStrategy = new EasyAi(solver);
                break;
            case "medium":
                aiStrategy = new MediumAi(solver);
                break;
            case "hard":
                aiStrategy = new HardAi(solver);
                break;
            case "impossible":
                aiStrategy = new ImpossibleAi(solver); ;
                break;
            default:
                aiStrategy = new ImpossibleAi(solver);
                break;
        }

        return aiStrategy.ChooseMove(scores);
    }
}
