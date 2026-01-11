using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUiManager : BaseUiManagerScript
{
    [SerializeField] StorageManagerScript storageManager;
    [SerializeField] Toggle checkBox;
    [SerializeField] TMP_Dropdown boardColorsDropdown;
    [SerializeField] TMP_Dropdown uiColorsDropdown;
    [SerializeField] List<BoardColors> boardColors;
    [SerializeField] List<UiColors> uiColors;

    
    protected override void Start()
    {
        base.Start();
        Settings settings = StaticData.settings;
        Debug.Log(settings);
        checkBox.isOn = settings.symbolMode;  
        boardColorsDropdown.value = boardColors.IndexOf(settings.boardColors);
        uiColorsDropdown.value = uiColors.IndexOf(settings.uiColors);
    }

    public override void SetColors()
    {
        checkBox.image.color = StaticData.settings.uiColors.buttonColor;
        checkBox.GetComponentInChildren<Outline>().effectColor = StaticData.settings.uiColors.outlineColor;
        base.SetColors();
    }

    public void SwitchColorblindMode(bool mode)
    {
        storageManager.ChangeSymbolMode(mode);
    }

    public void ChangeUiTheme(int index)
    {
        storageManager.ChangeUiColors(uiColors[index]);
    }

    public void ChangeBoardTheme(int index)
    {
        storageManager.ChangeBoardColors(boardColors[index]);
    }
}
