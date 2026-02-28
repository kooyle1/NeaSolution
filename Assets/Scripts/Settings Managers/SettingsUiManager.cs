using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUiManager : BaseUiManager
{
    [SerializeField] Toggle colorblindCheckbox;
    [SerializeField] Toggle windowedCheckbox;
    [SerializeField] TMP_Dropdown boardColorsDropdown;
    [SerializeField] TMP_Dropdown uiColorsDropdown;
    [SerializeField] Slider volumeSlider;
    
    protected override void Start()
    {
        base.Start();

        Settings settings = StorageManager.instance.settings;
        volumeSlider.value = settings.volume;
        windowedCheckbox.isOn = settings.windowedMode;
        colorblindCheckbox.isOn = settings.symbolMode;  
        boardColorsDropdown.value = StorageManager.instance.boardColorsList.IndexOf(settings.boardColors);
        uiColorsDropdown.value = StorageManager.instance.uiColorsList.IndexOf(settings.uiColors);
    }

}
