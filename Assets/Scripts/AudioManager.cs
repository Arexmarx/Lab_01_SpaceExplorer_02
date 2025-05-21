using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundAudioSource;

    [SerializeField] private AudioSource effectAudioSource;

    [SerializeField] private AudioClip backgroundClip;

    [SerializeField] private AudioClip shotClip;
    [SerializeField] private AudioClip starClip;
    [SerializeField] private AudioClip explosionClip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        backgroundAudioSource.clip = backgroundClip;
        backgroundAudioSource.Play();
    }

    public void PlayStarSound()
    {
        effectAudioSource.PlayOneShot(starClip);
    }

    public void PlayShotSound()
    {
        effectAudioSource.PlayOneShot(shotClip);
    }

    public void PlayExplosionSound()
    {
        effectAudioSource.PlayOneShot(explosionClip);
    }
}
