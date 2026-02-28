using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public abstract class BaseUiManager : MonoBehaviour
{
    [Header("Base Class Gameobject References")]
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected List<Image> ButtonColorList;
    [SerializeField] protected List<Image> BackgroundColorList;
    [SerializeField] protected List<GameObject> OutlineColorList;
    [SerializeField] protected List<TMP_Text> TextColorList;


    [Header("MonoBehaviour Script References")]
    [SerializeField] protected Logger logger;

    protected List<Button> buttonList;
    protected List<TMP_Text> textList;
    protected List<Outline> outlineList;

    /// <summary>
    ///  Adds all buttons in canvas to buttonList and all text in canvas to textList and then calls SetColors().
    /// </summary>
    protected virtual void Start()
    {
        SetWindowed(StorageManager.instance.settings.windowedMode);
        
        buttonList = canvas.GetComponentsInChildren<Button>(true).ToList();
        foreach (Button button in buttonList) {
            ButtonColorList.Add(button.image);
        }
        textList = canvas.GetComponentsInChildren<TMP_Text>(true).ToList();
        TextColorList.AddRange(textList);
        outlineList = canvas.GetComponentsInChildren<Outline>(true).ToList();
        foreach (Outline outline in outlineList) {
            OutlineColorList.Add(outline.gameObject);
        }

        SetColors();

    }

    /// <summary>
    ///  Sets colours of background and all buttons, text, dividers, and dropdowns in canvas.
    /// </summary>
    public virtual void SetColors()
    {
        UiColors uiColors = StorageManager.instance.settings.uiColors;
        
        foreach (Image img in ButtonColorList) {
            img.color = uiColors.buttonColor;
        }

        foreach (TMP_Text text in TextColorList) {
            text.color = uiColors.textColor;
        }

        foreach (GameObject obj in OutlineColorList) {
            Outline outline = obj.GetComponent<Outline>();
            Image image = obj.GetComponent<Image>();
            if (outline) {
                outline.effectColor = uiColors.outlineColor;
            }
            else if (image) {
                image.color = uiColors.outlineColor;
            }
        }

        foreach (Image img in BackgroundColorList) {
            img.color = uiColors.backgroundColor;   
        }
        
    }

    public void SetWindowed(bool windowed)
    {
        if (windowed && !StorageManager.instance.settings.windowedMode) {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        }
        else if (windowed) {
            Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.Windowed);
        }
        else {
            Screen.SetResolution(
            Screen.currentResolution.width,
            Screen.currentResolution.height,
            FullScreenMode.FullScreenWindow
        );
        }
    }
    public void EnableObject(GameObject uiObject)
    {
        logger.Log($"Enabled '{uiObject.name}'.");
        uiObject.SetActive(true);
    }

    public void DisableObject(GameObject uiObject)
    {
        logger.Log($"Disabled '{uiObject.name}'.");
        uiObject.SetActive(false);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadAnalysisScene()
    {
        SceneManager.LoadScene(1);
    }
    public void LoadSettingsScene()
    {
        SceneManager.LoadScene(2);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
