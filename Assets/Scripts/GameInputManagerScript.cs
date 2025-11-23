using UnityEngine;
using UnityEngine.EventSystems;

public class GameInputManagerScript : MonoBehaviour
{
    [Header("MonoBehaviour Script References")]
    [SerializeField] private BoardManagerScript boardManager;
    [SerializeField] private GameUiManagerScript gameUiManager;
    [SerializeField] private GameLogicScript gameLogicManager;

    private bool isRed = true;
    private bool yellowWon = false;
    private bool redWon = false;
    private bool isTie = false;

    public void PlayMoveOnClick()
    {
        if (yellowWon || redWon || isTie) {
            return;
        }

        GameObject column = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        int columnIndex = boardManager.columnList.IndexOf(column);

        if (!gameLogicManager.CanPlayColumn(columnIndex)) {
            return;
        }     
        gameLogicManager.PlayMove(columnIndex);

        //On player's turn, change slot color and assume they won before updating turn indicator text
        boardManager.PlayMove(column);
        boardManager.UpdateTurn(isRed);

        if (isRed) {
            isRed = false;
            redWon = true;
        }
        else {
            isRed = true;
            yellowWon = true;
        }

        //Display text based on what happened after the move (win, tie, or nothing)
        if (gameLogicManager.CheckWin()) {
            gameUiManager.DisplayWin(redWon);
        }
        else if (gameLogicManager.CheckTie()) {
            gameUiManager.DisplayTie();
            isTie = true;
        }
        else {
            gameUiManager.UpdateTurnIndicator(isRed);
            redWon = false;
            yellowWon = false;
        }
        
            
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
