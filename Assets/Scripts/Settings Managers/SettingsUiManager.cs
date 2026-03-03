using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUiManager : BaseUiManager
{
    [Header("Subclass Gameobject References")]
    [SerializeField] Toggle colorblindCheckbox;
    [SerializeField] Toggle windowedCheckbox;
    [SerializeField] TMP_Dropdown boardColorsDropdown;
    [SerializeField] TMP_Dropdown uiColorsDropdown;
    [SerializeField] Slider volumeSlider;
    
    protected override void Start()
    {
        base.Start();

        //Set value of all UI objects to their value in settings
        Settings settings = StorageManager.instance.settings;
        volumeSlider.value = settings.volume; 
        windowedCheckbox.isOn = settings.windowedMode;
        colorblindCheckbox.isOn = settings.symbolMode;  
        boardColorsDropdown.value = settings.boardColorsIndex;
        uiColorsDropdown.value = settings.uiColorsIndex;
    }

}
