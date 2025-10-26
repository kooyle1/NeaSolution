using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManagerScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Button button;
    [SerializeField] Image board;
    [SerializeField] Canvas canvas;

    [Header("Offsets and padding")]
    [SerializeField] int padding; //gap between cells
    [SerializeField] int xoffset; //horizontal offset from right board edge
    [SerializeField] int yoffset; //vertical offset from bottom board edge

    List<GameObject> columnList; //list of columns under board
    
    void Start()
    {
        columnList = new List<GameObject>();

        foreach (Transform child in board.transform) {
            columnList.Add(child.gameObject);         
        }
        CreateBoard(7,6);

    }

    /// <summary>
    ///  Instantiates buttons in a grid (width, height) that fits inside the board. Also destroys old board buttons.
    /// </summary>
    public void CreateBoard(int width, int height)
    {
        
        RectTransform buttonTransform = button.GetComponent<RectTransform>();
        RectTransform boardTransform = board.GetComponent<RectTransform>();

        //Destroy old buttons for recreating
        foreach (GameObject col in columnList) {
            foreach (Transform child in col.transform)
                Destroy(child.gameObject);
        }

        //Calculate and set new button diameter
        float widthDiameter = (boardTransform.rect.width - padding) / width;
        float heightDiameter = (boardTransform.rect.height - padding) / height;
        float buttonDiameter = Mathf.Min(widthDiameter, heightDiameter);
        buttonTransform.sizeDelta = new Vector2(buttonDiameter, buttonDiameter);

        //Create start position using position of top left corner of board
        Vector2 startPos = new Vector2(
            boardTransform.rect.xMin + xoffset - widthDiameter,
            boardTransform.rect.yMax + yoffset
        );

        //Instantiate columns of buttons from the start position
        Transform column;
        for (int i = 0; i < width;  i++) {
            startPos.x += widthDiameter + padding/width;
            startPos.y = boardTransform.rect.yMax + yoffset;
            column = columnList[i].transform;  

            for (int j = 0; j < height; j++) {
                startPos.y -= heightDiameter + padding/height;
                Instantiate(button, startPos, Quaternion.identity).transform.SetParent(column, false);
            }
        }
               
    }
}
