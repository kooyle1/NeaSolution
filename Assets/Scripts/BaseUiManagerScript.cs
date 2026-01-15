using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public abstract class BaseUiManagerScript : MonoBehaviour
{
    [Header("Base Class Gameobject References")]
    [SerializeField] protected Canvas canvas;
    [SerializeField] protected Image background;

    [Header("MonoBehaviour Script References")]
    [SerializeField] protected LoggerScript logger;

    protected List<Image> dropdownList;
    protected List<Image> dividerList;
    protected List<Button> buttonList;
    protected List<TMP_Text> textList;
    protected List<Outline> outlineList;

    /// <summary>
    ///  Adds all buttons in canvas to buttonList and all text in canvas to textList and then calls SetColors().
    /// </summary>
    protected virtual void Start()
    {
        if (StaticData.settings.windowedMode) {
            SetWindowed(true);
        }
        else {
            SetWindowed(false);
        }
        
        buttonList = canvas.GetComponentsInChildren<Button>(true).ToList();
        textList = canvas.GetComponentsInChildren<TMP_Text>(true).ToList();
        outlineList = canvas.GetComponentsInChildren<Outline>(true).ToList();
        dividerList = new List<Image>();
        dropdownList = new List<Image>();

        foreach (Image image in canvas.GetComponentsInChildren<Image>(true).ToList()) {
            if (image.CompareTag("Divider")) {
                dividerList.Add(image);
            }

            else if (image.CompareTag("Dropdown")) {
                dropdownList.Add(image);
            }
        }

        SetColors();

    }

    /// <summary>
    ///  Sets colours of background and all buttons, text, dividers, and dropdowns in canvas.
    /// </summary>
    public virtual void SetColors()
    {
        foreach (Button button in buttonList) {
            if (button.GetComponent<Outline>() == null) {
                continue;
            }           
            logger.Log($"Setting colour of button object '{button.name}'");
            button.image.color = StaticData.settings.uiColors.buttonColor;
        }

        foreach (TMP_Text text in textList) {
            logger.Log($"Setting colour of text object '{text.name}'");
            text.color = StaticData.settings.uiColors.textColor; 
        }

        foreach (Image divider in dividerList) {
            logger.Log($"Setting colour of divider object '{divider.name}'");
            divider.color = StaticData.settings.uiColors.outlineColor;
        }

        foreach (Image dropdown in dropdownList) {
            logger.Log($"Setting colour of dropdown object '{dropdown.name}'");
            dropdown.color = StaticData.settings.uiColors.buttonColor;
        }

        foreach (Outline outline in outlineList) {
            logger.Log($"Setting colour of dropdown object '{outline.name}'");
            outline.effectColor = StaticData.settings.uiColors.outlineColor;
        }

        background.color = StaticData.settings.uiColors.backgroundColor;
        logger.Log($"Setting colour of background object '{background.name}'");

    }

    protected void SetWindowed(bool windowed)
    {
        if (windowed) {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
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
