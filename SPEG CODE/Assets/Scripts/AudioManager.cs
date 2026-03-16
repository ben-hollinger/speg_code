using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Global Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    private AudioClip currentMusicClip;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }
    
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            StopMusic();
            return;
        }

        if (clip == currentMusicClip && musicSource.isPlaying)
        {
            return;
        }

        currentMusicClip = clip;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource == null)
        {
            return;
        }

        musicSource.Stop();
        currentMusicClip = null;
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        if (musicSource != null)
        {
            musicSource.volume = musicVolume;
        }
    }

    public void PlaySfx(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        float finalVolume = Mathf.Clamp01(sfxVolume * volumeMultiplier);
        sfxSource.PlayOneShot(clip, finalVolume);
    }
}
