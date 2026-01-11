using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardManagerScript : MonoBehaviour
{
    [Header("Gameobject References")]
    [SerializeField] Button button;
    [SerializeField] Image board;
    [SerializeField] Sprite triangleSprite;
    [SerializeField] Sprite squareSprite;


    [Header("MonoBehaviour Script References")]
    [SerializeField] LoggerScript logger;


    [Header("Settings")]
    [SerializeField] int padding; //gap between cells
    [SerializeField] private bool showingMovePreview;

    private int rowCount = 6;
    private int columnCount = 7;
    public List<GameObject> columnList { get; private set; }

    private GraphicRaycaster uiRaycaster;
    private PointerEventData pointerEventData;
    private List<RaycastResult> currentUiResults = new List<RaycastResult>();
    private RaycastResult[] prevUiResults;

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
        SetBoardColors();
        CreateBoard();

    }

    /// <summary>
    ///  Fills in the colour of the played slot (assuming given slot is the played slot).
    /// </summary>
    public void PlayMove(int columnIndex)
    {
        GameObject column = columnList[columnIndex];
        Button slot = GetBottomSlot(column);

        if (StaticData.redTurn) {
            logger.Log($"Placed red coin in column {columnIndex}.");
            slot.image.color = StaticData.settings.boardColors.fullRedColor;
            if (StaticData.settings.symbolMode) {
                slot.image.sprite = squareSprite;
            }    
        }
        else {
            logger.Log($"Placed yellow coin in column {columnIndex}.");
            slot.image.color = StaticData.settings.boardColors.fullYellowColor;
            if (StaticData.settings.symbolMode) {
                slot.image.sprite = triangleSprite;
            }
        }
    }

    public void RemoveCoin(int columnIndex)
    {
        GameObject column = columnList[columnIndex];
        Button slot = GetBottomSlot(column, true);

        logger.Log($"Removed coin in column {columnIndex}.");
        slot.image.color = StaticData.settings.boardColors.buttonColor;
    }

    private Button GetBottomSlot(GameObject column, bool getFirstFilledSlot = false)
    {
        Color currentColor;
        Button targetButton = null;
        foreach (Transform child in column.transform) {
            currentColor = child.GetComponent<Button>().image.color;
            if (currentColor == StaticData.settings.boardColors.fullRedColor || currentColor == StaticData.settings.boardColors.fullYellowColor) {
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

        currentObject = currentUiResults.Count > 0 ? currentUiResults[0].gameObject : null;
        prevObject = prevUiResults.Length > 0 ? prevUiResults[0].gameObject : null;
        
        //Check if mouse is hovering over new object or nothing. If it is, make sure to stop previewing the move for the last selected slot.
        if (currentObject != prevObject || currentUiResults.Count == 0) {
            var currentSlotImage = currentlyPreviewedSlot?.image;
            if (currentSlotImage && (currentSlotImage.color == StaticData.settings.boardColors.previewRedColor || currentSlotImage.color == StaticData.settings.boardColors.previewYellowColor)) {
                logger.LogFrame("Stopped previewing a move.");
                currentSlotImage.color = StaticData.settings.boardColors.buttonColor;
            }             
            return;
        }

        //Check if mouse is hovering over a button before proceding
        if (!currentObject.CompareTag("BoardButton")) {
            logger.LogFrame("Mouse is not hovering over a board button.");
            return;
        }

        Color previewColor = StaticData.redTurn ? StaticData.settings.boardColors.previewRedColor : StaticData.settings.boardColors.previewYellowColor;

        Transform parent = currentObject.transform.parent;
        Button targetButton = null;
        Color currentColor;

        //Loop through each button in the column, and stop once a played move is reached. Store the last empty slot before breaking.
        foreach (Transform child in parent) {
            currentColor = child.GetComponent<Button>().image.color;
            if (currentColor == StaticData.settings.boardColors.fullRedColor || currentColor == StaticData.settings.boardColors.fullYellowColor)
                break;
            targetButton = child.GetComponent<Button>();
        }

        //If there is an empty slot, set its color to preview color
        if (targetButton != null) {
            currentlyPreviewedSlot = targetButton;
            currentlyPreviewedSlot.image.color = previewColor;
            logger.LogFrame("Currently previewing a move");
            return;
        }
        logger.LogFrame("Attempted to preview a move in a full column");
    }

    public void UpdateRowCount(int index)
    {
        rowCount = index + 6;
    }

    public void UpdateColumnCount(int index)
    {
        columnCount = index + 6;
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
            foreach (Transform child in col.transform)
                Destroy(child.gameObject);
        }

        //Calculate and set new button diameter
        float widthDiameter = (boardTransform.rect.width - padding) / columnCount;
        float heightDiameter = (boardTransform.rect.height - padding) / rowCount;
        float buttonDiameter = Mathf.Min(widthDiameter, heightDiameter);
        buttonTransform.sizeDelta = new Vector2(buttonDiameter, buttonDiameter);

        //Create start position using position of top left corner of board

        // Starting position (top-left corner)
        Vector2 startPos = new Vector2(
            boardTransform.rect.xMin - widthDiameter,
            boardTransform.rect.yMax 
        );

        //Instantiate columns of buttons from the start position
        Transform column;
        for (int i = 0; i < columnCount;  i++) {
            startPos.x += widthDiameter + padding/columnCount;
            startPos.y = boardTransform.rect.yMax;
            column = columnList[i].transform;  

            for (int j = 0; j < rowCount; j++) {
                startPos.y -= heightDiameter + padding/rowCount;
                Instantiate(button, startPos, Quaternion.identity).transform.SetParent(column, false);
            }
        }
               
    }

    /// <summary>
    ///  Set colors of board and board buttons.
    /// </summary>
    public void SetBoardColors()
    {
        board.color = StaticData.settings.boardColors.boardColor;
        board.GetComponent<Outline>().effectColor = StaticData.settings.boardColors.boardOutlineColor;
        button.image.color = StaticData.settings.boardColors.buttonColor;
    }
}
