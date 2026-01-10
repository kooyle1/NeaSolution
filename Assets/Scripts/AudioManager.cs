using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Singleton for playing SFX

    public static AudioManager instance;
    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioClip moveSFX;
    [SerializeField] private float volume;

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

        Destroy(audioSource, 7f);
    }
}
