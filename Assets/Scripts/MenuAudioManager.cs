using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource menuAudioSource;
    [SerializeField] private AudioClip menuClip;
    private static bool isMenuMusicPlaying = false;

    void Start()
    {
        // Chỉ phát nhạc menu nếu chưa có nhạc nào đang phát
        if (!isMenuMusicPlaying)
        {
            PlayMenuBackgroundMusic();
        }
    }

    void OnDestroy()
    {
        // Đánh dấu nhạc menu đã dừng khi object bị hủy
        isMenuMusicPlaying = false;
    }

    public void PlayMenuBackgroundMusic()
    {
        if (menuAudioSource != null && menuClip != null)
        {
            // Dừng nhạc game nếu đang phát
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.StopBackgroundMusic();
            }

            // Phát nhạc menu
            menuAudioSource.clip = menuClip;
            menuAudioSource.Play();
            isMenuMusicPlaying = true;
        }
    }

    public void StopMenuBackgroundMusic()
    {
        if (menuAudioSource != null)
        {
            menuAudioSource.Stop();
            isMenuMusicPlaying = false;
        }
    }
}
