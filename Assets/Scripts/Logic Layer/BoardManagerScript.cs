using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [Header("Gameobject References")]
    [SerializeField] Button button;
    [SerializeField] Image board;
    [SerializeField] Sprite yellowAltSprite;
    [SerializeField] Sprite redAltSprite;
    [SerializeField] Sprite defaultSprite;

    [Header("MonoBehaviour Script References")]
    [SerializeField] Logger logger;

    [Header("Settings")]
    [SerializeField] int padding; //gap between cells
    [SerializeField] private bool showingMovePreview;

    public List<GameObject> columnList { get; private set; }

    //Variables for storing raycast info from the mouse
    private GraphicRaycaster uiRaycaster;
    private PointerEventData pointerEventData;
    private List<RaycastResult> currentUiResults = new List<RaycastResult>();
    private RaycastResult[] prevUiResults;

    //Variables for storing objects the mouse is hovering over
    private Button currentlyPreviewedSlot = null;
    private GameObject currentObject;
    private GameObject prevObject;

    private void Update()
    {
        if (showingMovePreview) {
            ShowMovePreview();
        }
    }

    private void Start()
    {
        columnList = new List<GameObject>();
        uiRaycaster = FindFirstObjectByType<GraphicRaycaster>();

        foreach (Transform child in board.transform) {
            columnList.Add(child.gameObject);
        }
        GameConfig.rows = 6;
        GameConfig.cols = 7;
        SetBoardColors();
        CreateBoard();
    }

    /// <summary>
    ///  Fills in the coin at the bottom of the given column. Uses GameState to fill according to player turn.
    /// </summary>
    public void PlayMove(int columnIndex)
    {
        GameObject column = columnList[columnIndex];
        Button slot = GetBottomSlot(column);

        if (GameState.redTurn) {
            slot.image.color = StorageManager.instance.boardColors.fullRedColor;
            if (StorageManager.instance.settings.symbolMode) {
                slot.image.sprite = redAltSprite; //Swaps sprite if in colourblind mode
            }
            logger.Log($"Placed red coin in column {columnIndex}.");
        }
        else {
            slot.image.color = StorageManager.instance.boardColors.fullYellowColor;
            if (StorageManager.instance.settings.symbolMode) {
                slot.image.sprite = yellowAltSprite; //Swaps sprite if in colourblind mode
            }
            logger.Log($"Placed yellow coin in column {columnIndex}.");
        }
    }

    /// <summary>
    ///  Removes coin at the top of a column.
    /// </summary>
    public void RemoveCoin(int columnIndex)
    {
        GameObject column = columnList[columnIndex];
        Button slot = GetBottomSlot(column, true);
        slot.image.sprite = defaultSprite;

        logger.Log($"Removed coin in column {columnIndex}.");
        slot.image.color = StorageManager.instance.boardColors.buttonColor;
    }

    /// <summary>
    ///  Get the first empty slot in a column. 
    ///  Set getFirstFilledSlot to true to get first filled slot in a column.
    /// </summary>
    private Button GetBottomSlot(GameObject column, bool getFirstFilledSlot = false)
    {
        Color currentColor;
        Button targetButton = null;

        //Loop through each slot in the column until the target slot is found.
        foreach (Transform child in column.transform) {
            currentColor = child.GetComponent<Button>().image.color;
            if (currentColor == StorageManager.instance.boardColors.fullRedColor || currentColor == StorageManager.instance.boardColors.fullYellowColor) {
                if (getFirstFilledSlot) {
                    targetButton = child.GetComponent<Button>();
                }
                break;
            }
            targetButton = child.GetComponent<Button>();
        }

        if (targetButton != null) {
            return targetButton;
        }

        return null;
    }

    /// <summary>
    ///  Displays a preview of the move that can be made when the mouse is hovered over the respective column.
    /// </summary>
    private void ShowMovePreview()
    {
        Vector2 mousePos = Input.mousePosition;
        pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = mousePos;

        //Store previous frame results and get current frame results
        prevUiResults = currentUiResults.ToArray();
        currentUiResults.Clear();
        uiRaycaster.Raycast(pointerEventData, currentUiResults);

        if (currentUiResults.Count == 0) {
            currentObject = null;
        }
        else {
            currentObject = currentUiResults[0].gameObject;
        }

        if (prevUiResults.Length == 0) {
            prevObject = null;
        }
        else {
            prevObject = prevUiResults[0].gameObject;
        }

        //Check if mouse is hovering over new object or nothing. If it is, make sure to stop previewing the move for the last selected slot.
        if (currentObject != prevObject || currentUiResults.Count == 0) {
            var currentSlotImage = currentlyPreviewedSlot?.image;
            if (currentSlotImage && (currentSlotImage.color == StorageManager.instance.boardColors.previewRedColor || currentSlotImage.color == StorageManager.instance.boardColors.previewYellowColor)) {
                logger.LogFrame("Stopped previewing a move.");
                currentSlotImage.color = StorageManager.instance.boardColors.buttonColor;
                currentSlotImage.sprite = defaultSprite;
            }
            return;
        }

        //Check if mouse is hovering over a button before proceding
        if (!currentObject.CompareTag("BoardButton")) {
            logger.LogFrame("Mouse is not hovering over a board button.");
            return;
        }

        Color previewColor;
        Sprite previewSprite;

        if (GameState.redTurn) {
            previewColor = StorageManager.instance.boardColors.previewRedColor;
            if (StorageManager.instance.settings.symbolMode) {
                previewSprite = redAltSprite;
            }
            else {
                previewSprite = defaultSprite;
            }
        }
        else {
            previewColor = StorageManager.instance.boardColors.previewYellowColor;
            if (StorageManager.instance.settings.symbolMode) {
                previewSprite = yellowAltSprite;
            }
            else {
                previewSprite = defaultSprite;
            }
        }

        Transform parent = currentObject.transform.parent;
        Button targetButton = null;
        Color currentColor;

        //Loop through each button in the column, and stop once a played move is reached. Store the last empty slot before breaking.
        foreach (Transform child in parent) {
            currentColor = child.GetComponent<Button>().image.color;
            if (currentColor == StorageManager.instance.boardColors.fullRedColor || currentColor == StorageManager.instance.boardColors.fullYellowColor)
                break;
            targetButton = child.GetComponent<Button>();
        }

        //If there is an empty slot, set its color to preview color
        if (targetButton != null) {
            currentlyPreviewedSlot = targetButton;
            currentlyPreviewedSlot.image.color = previewColor;
            currentlyPreviewedSlot.image.sprite = previewSprite;
            logger.LogFrame("Currently previewing a move");
            return;
        }
        logger.LogFrame("Attempted to preview a move in a full column");
    }

    /// <summary>
    ///  Instantiates buttons in a grid that fits inside the board. Buttons are instantiated into columns as children from top to down (so highest button is first and lowest is last). Also destroys old board buttons.
    /// </summary>
    public void CreateBoard()
    {
        RectTransform buttonTransform = button.GetComponent<RectTransform>();
        RectTransform boardTransform = board.GetComponent<RectTransform>();

        //Destroy old buttons for recreating
        foreach (GameObject col in columnList) {
            foreach (Transform child in col.transform) {
                Destroy(child.gameObject);
            }
        }

        //Calculate and set new button diameter
        float widthDiameter = (boardTransform.rect.width - padding) / GameConfig.cols;
        float heightDiameter = (boardTransform.rect.height - padding) / GameConfig.rows;
        float buttonDiameter = Mathf.Min(widthDiameter, heightDiameter);
        buttonTransform.sizeDelta = new Vector2(buttonDiameter, buttonDiameter);

        //Create start position using position of top left corner of board

        // Starting position (top-left corner)
        Vector2 startPos = new Vector2(boardTransform.rect.xMin - widthDiameter, boardTransform.rect.yMax);

        //Instantiate columns of buttons from the start position
        Transform column;
        for (int i = 0; i < GameConfig.cols; i++) {
            startPos.x += widthDiameter + padding / GameConfig.cols;
            startPos.y = boardTransform.rect.yMax;
            column = columnList[i].transform;

            for (int j = 0; j < GameConfig.rows; j++) {
                startPos.y -= heightDiameter + padding / GameConfig.rows;
                Instantiate(button, startPos, Quaternion.identity).transform.SetParent(column, false);
            }
        }

    }

    /// <summary>
    ///  Set colors of board and board buttons.
    /// </summary>
    public void SetBoardColors()
    {
        BoardColors boardColors = StorageManager.instance.boardColors;

        board.color = boardColors.boardColor;
        board.GetComponent<Outline>().effectColor = boardColors.boardOutlineColor;
        button.image.color = boardColors.buttonColor;
    }
}
