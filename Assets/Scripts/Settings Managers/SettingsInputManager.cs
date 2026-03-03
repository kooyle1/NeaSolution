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

    public void SwitchWindowedMode(bool windowed)
    {
        if (windowed) {
            settingsUiManager.SetWindowed();
        }
        else {
            settingsUiManager.SetFullscreen();
        }
        StorageManager.instance.ChangeSetting(s => s.windowedMode = windowed);
    }

    public void SwitchColorblindMode(bool mode)
    {
        StorageManager.instance.ChangeSetting(s => s.symbolMode = mode);
    }

    public void ChangeUiTheme(int index)
    {
        StorageManager.instance.ChangeSetting(s => s.uiColorsIndex = index);
    }

    public void ChangeBoardTheme(int index)
    {
        StorageManager.instance.ChangeSetting(s => s.boardColorsIndex = index);
    }
}
