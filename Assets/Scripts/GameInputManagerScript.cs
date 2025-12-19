using UnityEngine;
using UnityEngine.EventSystems;

public class GameInputManagerScript : MonoBehaviour
{
    [Header("MonoBehaviour Script References")]
    [SerializeField] private BoardManagerScript boardManager;
    [SerializeField] private GameUiManagerScript gameUiManager;
    [SerializeField] private GameLogicScript gameLogicManager;

    [Header("MonoBehaviour Script References")]
    [SerializeField] LoggerScript logger;

    private bool isRed = true;
    private bool yellowWon = false;
    private bool redWon = false;
    private bool isTie = false;

    public void PlayMoveOnClick()
    {
        if (yellowWon || redWon || isTie) {
            logger.Log("Player tried moving after the round ended.");
            return;
        }

        GameObject column = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        int columnIndex = boardManager.columnList.IndexOf(column);

        if (!gameLogicManager.CanPlayColumn(columnIndex)) {
            logger.Log("Player tried moving in a full column.");
            return;
        }   
        
        gameLogicManager.PlayMove(columnIndex);
        logger.Log($"Player successfully made a move in column {columnIndex}");

        //Update turn and assume player won before actually checking     
        if (isRed) {
            isRed = false;
            redWon = true;
        }
        else {
            isRed = true;
            yellowWon = true;
        }

        boardManager.PlayMove(column);
        boardManager.UpdateTurn(isRed);

        //Display text based on what happened after the move (win, tie, or nothing)
        if (gameLogicManager.CheckWin()) {
            logger.Log($"A win occured on this turn.");
            gameUiManager.DisplayWin(redWon);
        }
        else if (gameLogicManager.CheckTie()) {
            logger.Log("A tie occured on this turn.");
            gameUiManager.DisplayTie();
            isTie = true;
        }
        else {
            gameUiManager.UpdateTurnIndicator(isRed);
            logger.Log("Game did not end, now other player's turn.");
            redWon = false;
            yellowWon = false;
        }
        
            
    }

    public void StartNextRound()
    {
        isRed = true;
        yellowWon = false;
        redWon = false;
        isTie = false;

        gameUiManager.UpdateTurnIndicator(isRed);
        boardManager.UpdateTurn(isRed);
        gameLogicManager.ResetBoard();
        boardManager.CreateBoard();
    }

    public void StartNewGame()
    {
        StartNextRound();
        gameUiManager.ResetScores();
    }

    /* For analysis screen
    public void PlayMove(int column)
    {
        Button slot = GetBottomSlot(boardManager.columnList[column]);
        if (!slot) {
            return;
        }

        gameUiManager.UpdateTurnIndicator(isRed);
        boardManager.UpdateTurn(isRed);
        if (isRed) {
            slot.image.color = boardManager.BoardColors.fullRedColor;
            isRed = false;
        }
        else {
            slot.image.color = boardManager.BoardColors.fullYellowColor;
            isRed = true;
        }
    }

    public void UndoMove(int column)
    {
        Button slot = GetBottomSlot(boardManager.columnList[column]);
        if (!slot) {
            return;
        }

        gameUiManager.UpdateTurnIndicator(isRed);
        boardManager.UpdateTurn(isRed);
        if (isRed) {
            slot.image.color = boardManager.BoardColors.buttonColor;
            isRed = false;
        }
        else {
            slot.image.color = boardManager.BoardColors.buttonColor;
            isRed = true;
        }
    }
    */
}
