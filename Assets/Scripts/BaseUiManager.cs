using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseUiManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] UiColors colors;
    [SerializeField] Canvas canvas;
    [SerializeField] Image background;

    private List<Button> buttonList;
    private List<TMP_Text> textList;

    protected virtual void Start()
    {
        buttonList = new List<Button>();
        textList = new List<TMP_Text>();

        foreach (Transform child in canvas.transform) {
            Button button = child.GetComponent<Button>();
            if (button != null) {
                buttonList.Add(button);
                continue;
            }
            TMP_Text text = child.GetComponent<TMP_Text>();
            if (text != null) {
                textList.Add(text);
                continue;
            }
        }
        Debug.Log(textList.Count);
        SetColors();
    }

    public virtual void SetColors()
    {
        foreach (Button button in buttonList) {
            button.image.color = colors.buttonColor;
            button.GetComponent<Outline>().effectColor = colors.outlineColor;
            button.transform.GetChild(0).GetComponent<TMP_Text>().color = colors.textColor;
        }
        foreach(TMP_Text text in textList) {
            text.color = colors.textColor; 
        }
        background.color = colors.backgroundColor;

    }

}
