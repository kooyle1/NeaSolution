using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    //Singleton for playing SFX

    public static AudioManager instance { get; private set; }

    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioClip moveSFX;
    [SerializeField] private float volume;
    [SerializeField] private AudioMixer audioMixer;
    private void Start()
    {
        Load();
    }

    public void SetVolume(float level)
    {
        audioMixer.SetFloat("Master", level);
    }

    private void Load()
    {
        float volume = StorageManager.instance.settings.volume;

        audioMixer.SetFloat("Master", volume);
    }

    private void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else {
            Destroy(gameObject);
        }
    }

    public void PlayMoveSFX()
    {
        AudioSource audioSource = Instantiate(soundFXObject);
        audioSource.clip = moveSFX;
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(audioSource.gameObject, audioSource.clip.length);
    }
}
