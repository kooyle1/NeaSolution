using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManagerScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoardManagerScript boardManager;
    [SerializeField] private GameUiManager gameUiManager;

    public bool isRed { get; private set; } = true;

    private Button GetBottomSlot(GameObject column)
    {
        Color currentColor;
        Button targetButton = null;
        foreach (Transform child in column.transform) {
            currentColor = child.GetComponent<Button>().image.color;
            if (currentColor == boardManager.BoardColors.fullRedColor || currentColor == boardManager.BoardColors.fullYellowColor) {
                break;
            }                       
            targetButton = child.GetComponent<Button>();
        }

        if (targetButton != null) {
            return targetButton;
        }
        return null;
    }

    public void PlayMoveOnClick()
    {
        GameObject column = EventSystem.current.currentSelectedGameObject.transform.parent.gameObject;
        Button slot = GetBottomSlot(column);

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
