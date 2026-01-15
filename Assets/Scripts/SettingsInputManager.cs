using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUiManager : BaseUiManagerScript
{
    [SerializeField] StorageManagerScript storageManager;
    [SerializeField] Toggle colorblindCheckbox;
    [SerializeField] Toggle windowedCheckbox;
    [SerializeField] TMP_Dropdown boardColorsDropdown;
    [SerializeField] TMP_Dropdown uiColorsDropdown;
    

    
    protected override void Start()
    {
        base.Start();

        Settings settings = StaticData.settings;
        windowedCheckbox.isOn = settings.windowedMode;
        colorblindCheckbox.isOn = settings.symbolMode;  
        boardColorsDropdown.value = storageManager.boardColorsList.IndexOf(settings.boardColors);
        uiColorsDropdown.value = storageManager.uiColorsList.IndexOf(settings.uiColors);
    }

    public override void SetColors()
    {
        colorblindCheckbox.image.color = StaticData.settings.uiColors.buttonColor;
        windowedCheckbox.image.color = StaticData.settings.uiColors.buttonColor;
        base.SetColors();
    }

    public void SwitchWindowedMode(bool mode)
    {
        storageManager.ChangeWindowedMode(mode);
        base.SetWindowed(mode);
    }

    public void SwitchColorblindMode(bool mode)
    {
        storageManager.ChangeSymbolMode(mode);
    }

    public void ChangeUiTheme(int index)
    {
        storageManager.ChangeUiColors(storageManager.uiColorsList[index]);
    }

    public void ChangeBoardTheme(int index)
    {
        storageManager.ChangeBoardColors(storageManager.boardColorsList[index]);
    }
}
