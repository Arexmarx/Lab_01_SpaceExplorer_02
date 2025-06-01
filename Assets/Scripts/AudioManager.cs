using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource backgroundAudioSource;

    [SerializeField] private AudioSource effectAudioSource;

    [SerializeField] private AudioClip backgroundClip;

    [SerializeField] private AudioClip shotClip;
    [SerializeField] private AudioClip starClip;
    [SerializeField] private AudioClip explosionClip;

    public static AudioManager GetInstance()
    {
        if (Instance == null)
        {
            // Tìm AudioManager trong scene
            Instance = FindObjectOfType<AudioManager>();

            // Nếu không tìm thấy, tạo mới
            if (Instance == null)
            {
                GameObject audioManagerObj = new GameObject("AudioManager");
                Instance = audioManagerObj.AddComponent<AudioManager>();
            }
        }
        return Instance;
    }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            
            // Đảm bảo các AudioSource component tồn tại
            if (backgroundAudioSource == null)
            {
                backgroundAudioSource = gameObject.AddComponent<AudioSource>();
                backgroundAudioSource.loop = true;
                backgroundAudioSource.playOnAwake = false;
            }
            
            if (effectAudioSource == null)
            {
                effectAudioSource = gameObject.AddComponent<AudioSource>();
                effectAudioSource.loop = false;
                effectAudioSource.playOnAwake = false;
            }

            DontDestroyOnLoad(gameObject);
            PlayBackgroundMusic();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnValidate()
    {
        // Kiểm tra và thêm AudioSource nếu chưa có trong Editor
        if (backgroundAudioSource == null)
        {
            backgroundAudioSource = gameObject.AddComponent<AudioSource>();
            backgroundAudioSource.loop = true;
            backgroundAudioSource.playOnAwake = false;
        }
        
        if (effectAudioSource == null)
        {
            effectAudioSource = gameObject.AddComponent<AudioSource>();
            effectAudioSource.loop = false;
            effectAudioSource.playOnAwake = false;
        }
    }

    public void StopBackgroundMusic()
    {
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.Stop();
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundAudioSource != null && !backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.clip = backgroundClip;
            backgroundAudioSource.Play();
        }
    }

    public void PlayStarSound()
    {
        if (effectAudioSource != null && starClip != null)
        {
            effectAudioSource.PlayOneShot(starClip);
        }
    }

    public void PlayShotSound()
    {
        if (effectAudioSource != null && shotClip != null)
        {
            effectAudioSource.PlayOneShot(shotClip);
        }
    }

    public void PlayExplosionSound()
    {
        if (effectAudioSource != null && explosionClip != null)
        {
            effectAudioSource.PlayOneShot(explosionClip);
        }
    }

    public void ResetAudio()
    {
        // Dừng tất cả âm thanh
        if (effectAudioSource != null)
        {
            effectAudioSource.Stop();
        }
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.Stop();
            // Phát lại nhạc nền
            PlayBackgroundMusic();
        }
    }
}
