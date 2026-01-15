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

    private int[] currentMoveList;
    private int index = -1;

    public void OpenGameReplay()
    {
        GetGame();
        boardManager.CreateBoard();
        analysisUiManager.UpdateTurnIndicator();
        index = -1;
    }

    private void GetGame()
    {
        int index = int.Parse(EventSystem.current.currentSelectedGameObject.name[0].ToString());
        Game currentGame = analysisUiManager.GetGameByIndex(index);
        currentMoveList = currentGame.moveList.ToArray();
        StaticData.rows = currentGame.rows;
        StaticData.cols = currentGame.cols;
        logger.Log(string.Join(", ", currentGame));
        logger.Log(index);
    }

    public void PlayNextMove()
    {
        if (index >= currentMoveList.Length - 1) {
            return;
        }
        AudioManager.instance.PlayMoveSFX();
        index++;
        int columnIndex = currentMoveList[index];
        boardManager.PlayMove(columnIndex);
        gameLogic.PlayMove(columnIndex);
        StaticData.redTurn = !StaticData.redTurn;
        analysisUiManager.UpdateTurnIndicator();
    }

    public void UndoMove()
    {
        if (index < 0) {
            return;
        }
        AudioManager.instance.PlayMoveSFX();
        int columnIndex = currentMoveList[index];
        boardManager.RemoveCoin(columnIndex);
        gameLogic.UndoMove(columnIndex);
        StaticData.redTurn = !StaticData.redTurn;
        analysisUiManager.UpdateTurnIndicator();
        index--;

    }

    public void ShowSolution()
    {
        int solution = solver.ReturnBestMove(gameLogic.GetPos(), gameLogic.GetBoard());
        analysisUiManager.DisplaySolution(solution);
    }
}
