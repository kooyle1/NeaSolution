using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public abstract class BaseUiManager : MonoBehaviour
{
    [Header("Base Class References")]
    [SerializeField] protected UiColors colors;
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected Image background;
    [SerializeField] protected List<Image> dividerList;

    protected List<Button> buttonList;
    protected List<TMP_Text> textList;

    protected virtual void Start()
    {
        buttonList = canvas.GetComponentsInChildren<Button>(true).ToList();
        textList = canvas.GetComponentsInChildren<TMP_Text>(true).ToList();
        SetColors();
    }

    public virtual void SetColors()
    {
        foreach (Button button in buttonList) {
            if (button.GetComponent<Outline>() == null) {
                continue;
            }           
            button.image.color = colors.buttonColor;
            button.GetComponent<Outline>().effectColor = colors.outlineColor;
        }

        foreach (TMP_Text text in textList) {
            text.color = colors.textColor; 
        }

        foreach (Image divider in dividerList) {
            divider.color = colors.outlineColor;
        }

        background.color = colors.backgroundColor;

    }

    protected virtual void SetDropdownColors()
    {
        foreach (Image image in canvas.GetComponentsInChildren<Image>(true).ToList()) {
            if (image.CompareTag("Dropdown")) {
                image.color = colors.buttonColor;
            }
        }
    }

    public virtual void LoadGameScene()
    {
        SceneManager.LoadScene(0);
    }

    public virtual void LoadAnalysisScene()
    {
        SceneManager.LoadScene(1);
    }
    public virtual void LoadSettingsScene()
    {
        SceneManager.LoadScene(2);
    }

    public virtual void ExitGame()
    {
        ExitGame();
    }

}
