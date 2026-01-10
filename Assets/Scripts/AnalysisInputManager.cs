using UnityEngine;
using UnityEngine.EventSystems;

public class AnalysisInputManager : MonoBehaviour
{
    [Header("MonoBehaviour Script References")]
    [SerializeField] private BoardManagerScript boardManager;
    [SerializeField] private AnalysisUiManagerScript analysisUiManager;
    [SerializeField] private StorageManagerScript storageManager;
    [SerializeField] private GameLogicScript gameLogic;
    [SerializeField] private SolverScript solver;
    [SerializeField] LoggerScript logger;

    private int[] currentGame;
    private int index = -1;

    private bool isRed = true;

    public void OpenGameReplay()
    {
        GetGame();
        boardManager.CreateBoard();
        index = -1;
    }

    private void GetGame()
    {
        int index = int.Parse(EventSystem.current.currentSelectedGameObject.name[0].ToString());
        currentGame = analysisUiManager.GetGameByIndex(index);
        logger.Log(string.Join(", ", currentGame));
        logger.Log(index);
    }

    public void PlayNextMove()
    {
        if (index >= currentGame.Length - 1) {
            return;
        }
        AudioManager.instance.PlayMoveSFX();
        index++;
        int columnIndex = currentGame[index];
        boardManager.PlayMove(columnIndex);
        gameLogic.PlayMove(columnIndex);
        isRed = !isRed;
        boardManager.UpdateTurn(isRed);
        analysisUiManager.UpdateTurnIndicator(isRed);
    }

    public void UndoMove()
    {
        if (index < 0) {
            return;
        }
        AudioManager.instance.PlayMoveSFX();
        int columnIndex = currentGame[index];
        boardManager.RemoveCoin(columnIndex);
        gameLogic.UndoMove(columnIndex);
        isRed = !isRed;
        boardManager.UpdateTurn(isRed);
        analysisUiManager.UpdateTurnIndicator(isRed);
        index--;

    }

    public void ShowSolution()
    {
        int solution = solver.ReturnBestMove(gameLogic.GetPos(), gameLogic.GetBoard());
        analysisUiManager.DisplaySolution(solution);
    }
}
