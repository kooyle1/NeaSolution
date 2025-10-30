using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardManagerScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button button;
    [SerializeField] Image board;
    [SerializeField] BoardColors boardColors;

    public BoardColors BoardColors { get { return boardColors; } }

    [Header("Offsets and padding")]
    [SerializeField] int padding; //gap between cells
    [SerializeField] int xoffset; //horizontal offset from right board edge
    [SerializeField] int yoffset; //vertical offset from bottom board edge

    [Header("Move preview variables and references")]
    [SerializeField] private bool showingMovePreview;

    private int rowCount = 6;
    private int columnCount = 7;
    public List<GameObject> columnList { get; private set; } //list of columns under board

    private GraphicRaycaster uiRaycaster;
    private PointerEventData pointerEventData;
    private List<RaycastResult> currentUiResults = new List<RaycastResult>();
    private RaycastResult[] prevUiResults;

    private Button currentlyPreviewedSlot;
    private GameObject currentObject;
    private GameObject prevObject;

    private bool isRed;


    private void Update()
    {
        if (showingMovePreview) {
            ShowMovePreview();
        }
    }

    void Start()
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
            if (currentSlotImage && (currentSlotImage.color == boardColors.previewRedColor || currentSlotImage.color == boardColors.previewYellowColor))
                currentSlotImage.color = boardColors.buttonColor;
            return;
        }

        //Check if mouse is hovering over a button before proceding
        if (!currentObject.CompareTag("BoardButton")) { 
            return;
        }

        Color previewColor = isRed ? boardColors.previewRedColor : boardColors.previewYellowColor;

        Transform parent = currentObject.transform.parent;
        Button targetButton = null;
        Color currentColor;

        //Loop through each button in the column, and stop once a played move is reached. Store the last empty slot before breaking.
        foreach (Transform child in parent) {
            currentColor = child.GetComponent<Button>().image.color;
            if (currentColor == boardColors.fullRedColor || currentColor == boardColors.fullYellowColor)
                break;

            targetButton = child.GetComponent<Button>();
        }

        //If there is an empty slot, set its color to preview color
        if (targetButton != null) {
            currentlyPreviewedSlot = targetButton;
            currentlyPreviewedSlot.image.color = previewColor;
        }
    }


    /// <summary>
    ///  Updates the internal variable tracking the current turn.
    /// </summary>
    public void UpdateTurn(bool redPlayedTurn)
    {
        if (redPlayedTurn) {
            isRed = false;
        }
        else {
            isRed = true;
        }
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
        Vector2 startPos = new Vector2(
            boardTransform.rect.xMin + xoffset - widthDiameter,
            boardTransform.rect.yMax + yoffset
        );

        //Instantiate columns of buttons from the start position
        Transform column;
        for (int i = 0; i < columnCount;  i++) {
            startPos.x += widthDiameter + padding/columnCount;
            startPos.y = boardTransform.rect.yMax + yoffset;
            column = columnList[i].transform;  

            for (int j = 0; j < rowCount; j++) {
                startPos.y -= heightDiameter + padding/rowCount;
                Instantiate(button, startPos, Quaternion.identity).transform.SetParent(column, false);
            }
        }
               
    }

    public void SetBoardColors()
    {
        board.color = boardColors.boardColor;
        button.image.color = boardColors.buttonColor;
    }
}
