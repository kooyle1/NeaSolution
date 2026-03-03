using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class AnalysisInputManager : MonoBehaviour
{
    [Header("MonoBehaviour Script References")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private AnalysisUiManagerScript analysisUiManager;
    [SerializeField] private BitboardManager bitboardManager;
    [SerializeField] Logger logger;
    [SerializeField] GameController gameController;
    [SerializeField] AiController aiController;

    private MoveRater moveRater = new MoveRater();
    private int[] currentMoveList;
    private int index = -1;

    /// <summary>
    ///  Gets game info from selected game and prepares the game replay screen.
    /// </summary>
    public void OpenGameReplay()
    {
        GetGame();
        gameController.ResetGame();
        boardManager.CreateBoard();
        analysisUiManager.UpdateTurnIndicator();
        index = -1;

    }

    /// <summary>
    ///  Get game associated with currently selected game object.
    /// </summary>
    private void GetGame()
    {
        Game currentGame = analysisUiManager.GetGameByIndex(EventSystem.current.currentSelectedGameObject);
        currentMoveList = currentGame.moveList.ToArray();
        GameConfig.rows = currentGame.rows;
        GameConfig.cols = currentGame.cols;
        logger.Log("MoveList: " + string.Join(", ", currentGame.moveList));
    }

    public void PlayNextMove()
    {
        if (index >= currentMoveList.Length - 1) {
            return;
        }
        aiController.StopSolving();
        index++;
        gameController.TryMakeMove(currentMoveList[index]);
        analysisUiManager.UpdateTurnIndicator();
    }

    public void UndoMove()
    {
        if (index < 0) {
            return;
        }
        aiController.StopSolving();
        gameController.TryUndoMove(currentMoveList[index]);
        analysisUiManager.UpdateTurnIndicator();
        index--;

    }

    /// <summary>
    ///  Calls AI controller to solve position, then calls move rater to give a rating.
    /// </summary>
    public void ShowSolution()
    {
        string rating;
        int bestMove = -1;
        
        if (StorageManager.instance.isLoading) {
            rating = "AI still loading...";
            analysisUiManager.DisplaySolution(bestMove, rating);
            return;
        }
        
        if (GameState.redWon || GameState.yellowWon || GameState.isTie) {
            rating = "Solution not available, game ended.";
            analysisUiManager.DisplaySolution(bestMove, rating);
            return;
        }

        if (index == -1) {
            rating = "Solution not available, empty board";
            analysisUiManager.DisplaySolution(bestMove, rating);
            return;
        }

        if (GameConfig.rows != 6 || GameConfig.cols != 7) {
            rating = "Solution not available for this board size.";
            analysisUiManager.DisplaySolution(bestMove, rating);
            return;
        }

        int playedMove = currentMoveList[index];
        bitboardManager.UndoMove(playedMove);
        aiController.StartSolving(scores => {
            logger.Log("Unsorted Scores: {" + String.Join(", ", scores) + "}");
            int bestMove = aiController.ReturnBestMove(scores);
            string rating = moveRater.ReturnRating(playedMove, scores);
            analysisUiManager.DisplaySolution(bestMove, rating);
        });
        bitboardManager.PlayMove(playedMove);
    }

}

public class MoveRater
{
    private string bestRating = "Best";
    private string goodRating = "Good";
    private string badRating = "Bad";
    private string worstRating = "Worst";
    private string forcedRating = "Forced (All moves lead to the same outcome)";

    public string ReturnRating(int playedMove, int[] scores)
    {
        string rating = "";
        int playedMoveScore = scores[playedMove];
        Array.Sort(scores);
        Array.Reverse(scores);

        int bestScore = scores[0];

        int lastIndex = 1;
        int worstScore = scores[^lastIndex];
        while (worstScore <= -100) {
            lastIndex++;
            worstScore = scores[^lastIndex];
        }
        if (bestScore == worstScore) {
            rating = forcedRating;
        }

        else if (playedMoveScore == bestScore) {
            rating = bestRating;
        }

        else if (playedMoveScore == worstScore) {
            rating = worstRating;
        }

        else if (bestScore > 0 && playedMoveScore > 0 || bestScore < 0 && playedMoveScore < 0 || bestScore == 0 && playedMoveScore == 0) {
            rating = goodRating;
        }

        else if (bestScore > 0 && playedMoveScore <= 0 || bestScore == 0 && playedMoveScore < 0) {
            rating = badRating;
        }

        return rating;
    }
}
