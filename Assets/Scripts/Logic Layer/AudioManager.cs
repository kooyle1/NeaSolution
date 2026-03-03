using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    [Header("Base Class Gameobject References")]
    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioClip moveSFX;
    [SerializeField] private AudioMixer audioMixer;

    private float volume = 0;
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
        volume = StorageManager.instance.settings.volume;
        audioMixer.SetFloat("Master", volume);
    }

    private void Awake()
    {
        if (instance == null) {
            instance = this;
        }

        else {
            Destroy(gameObject);
        }
    }

    public void PlayMoveSFX()
    {
        AudioSource audioSource = Instantiate(soundFXObject);
        audioSource.clip = moveSFX;
        audioSource.Play();

        Destroy(audioSource.gameObject, audioSource.clip.length);
    }
}
