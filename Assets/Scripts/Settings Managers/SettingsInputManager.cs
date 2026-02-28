using UnityEngine;

public class SettingsInputManager : MonoBehaviour
{
    [Header("Monobehaviour Script References")]
    [SerializeField] SettingsUiManager settingsUiManager;

    public void SetVolume(float volume)
    {
        AudioManager.instance.SetVolume(volume);
        StorageManager.instance.ChangeSetting(s => s.volume = volume);
    }

    public void SwitchWindowedMode(bool mode)
    {
        settingsUiManager.SetWindowed(mode);
        StorageManager.instance.ChangeSetting(s => s.windowedMode = mode);
    }

    public void SwitchColorblindMode(bool mode)
    {
        StorageManager.instance.ChangeSetting(s => s.symbolMode = mode);
    }

    public void ChangeUiTheme(int index)
    {
        StorageManager.instance.ChangeSetting(s => s.uiColors = StorageManager.instance.uiColorsList[index]);
    }

    public void ChangeBoardTheme(int index)
    {
        StorageManager.instance.ChangeSetting(s => s.boardColors = StorageManager.instance.boardColorsList[index]);
    }
}
