using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerScript : MonoBehaviour
{
    //For the slider, saves the volume when you close the game via PlayerPrefs

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private StorageManagerScript storageManager;

    private void Awake()
    {
        Load();
    }

    public void SetVolume(float level)
    {
        audioMixer.SetFloat("Master", level);
        storageManager.ChangeVolume(level);
    }

    private void Load()
    {
        volumeSlider.value = storageManager.LoadSettings().volume;
    }
}
