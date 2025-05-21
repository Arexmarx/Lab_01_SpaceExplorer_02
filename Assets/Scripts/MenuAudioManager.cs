using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource menuAudioSource;

    [SerializeField] private AudioClip menuClip;
    void Start()
    {
        PlayMenuBackgroundMusic();
    }

    public void PlayMenuBackgroundMusic()
    {
        menuAudioSource.clip = menuClip;
        menuAudioSource.Play();
    }
}
