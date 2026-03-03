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

    //Variables for multithreading in the StartSolving method
    public bool aiThinking { get; private set; } = false;
    public int[] scores { get; private set; }
    private Task<int[]> aiTask;
    private Action<int[]> callback;
    private bool continueSolving = false;

    private void Update()
    {
        if (!aiThinking || aiTask == null) return;

        if (aiTask.IsCompleted) {
            aiThinking = false;
            int[] scores = aiTask.Result;
            aiTask = null;

            if (continueSolving) {
                callback?.Invoke(scores); //Call the action once AI returns scores 

            }
        }
    }

    /// <summary>
    ///  Starts solving the current position in a background thread so as to not clog up the main thread.
    ///  When the AI is finished solving, it returns an array of scores.
    ///  To use this array, input an action of the form scores => { YOUR CODE HERE }.
    /// </summary>
    public void StartSolving(Action<int[]> onMoveReady) => StartSolving(bitboard.GetPos(), bitboard.GetBoard(), onMoveReady);
    
    public void StartSolving(ulong pos, ulong board, Action<int[]> onMoveReady)
    {
        if (aiThinking) return;
        aiThinking = true;
        continueSolving = true;
        callback = onMoveReady;

        aiTask = Task.Run(() =>
        {
            return solver.ReturnScores(pos, board);
        });
    }

    /// <summary>
    ///  Stops the AI Controller invoking the action from the last StartSolving calls.
    /// </summary>
    public void StopSolving()
    {
        continueSolving = false;
    }

    /// <summary>
    ///  Return the best column to play in from a given set of scores.
    /// </summary>
    public int ReturnBestMove(int[] scores)
    {
        return solver.ChooseBestMove(scores);
    }

    /// <summary>
    ///  Return a column to play in using the current AI difficulty from a given set of scores.
    /// </summary>
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
