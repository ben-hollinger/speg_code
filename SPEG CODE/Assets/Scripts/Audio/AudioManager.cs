using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource dialogueSource;

    [Header("Global Volumes")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;

    [SerializeField] private AudioClip currentMusicClip;

    private Coroutine _musicFadeRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(Instance.gameObject);

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;

        if (dialogueSource == null)
            dialogueSource = gameObject.AddComponent<AudioSource>();
        dialogueSource.playOnAwake = false;
        dialogueSource.loop = false;
        dialogueSource.volume = 6;

        if (currentMusicClip != null)
        {
            PlayMusic(currentMusicClip);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void FadeOutMusic(float duration)
    {
        if (musicSource == null)
            return;

        if (_musicFadeRoutine != null)
            StopCoroutine(_musicFadeRoutine);

        _musicFadeRoutine = StartCoroutine(FadeOutMusicRoutine(duration));
    }

    private IEnumerator FadeOutMusicRoutine(float duration)
    {
        float start = musicSource.volume;

        if (duration <= 0f)
        {
            musicSource.volume = 0f;
            musicSource.Stop();
            currentMusicClip = null;
            _musicFadeRoutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(start, 0f, elapsed / duration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
        currentMusicClip = null;
        _musicFadeRoutine = null;
    }

    private IEnumerator FadeInMusicRoutine(float duration)
    {
        if (duration <= 0f)
        {
            musicSource.volume = musicVolume;
            _musicFadeRoutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, musicVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = musicVolume;
        _musicFadeRoutine = null;
    }

    public void PlayMusic(AudioClip clip, float fadeInDuration = 2f)
    {
        if (_musicFadeRoutine != null)
        {
            StopCoroutine(_musicFadeRoutine);
            _musicFadeRoutine = null;
        }

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
        musicSource.Play();
        musicSource.volume = 0f;

        if (fadeInDuration <= 0f)
        {
            musicSource.volume = musicVolume;
            return;
        }

        _musicFadeRoutine = StartCoroutine(FadeInMusicRoutine(fadeInDuration));
    }

    public void StopMusic()
    {
        if (_musicFadeRoutine != null)
        {
            StopCoroutine(_musicFadeRoutine);
            _musicFadeRoutine = null;
        }

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

        float finalVolume = Mathf.Max(0f, sfxVolume * volumeMultiplier);
        sfxSource.PlayOneShot(clip, finalVolume);
    }

    public void PlayDialogue(AudioClip clip, float volumeMultiplier = 1f)
    {
        float finalVolume = Mathf.Max(0f, sfxVolume * volumeMultiplier);
        dialogueSource.Stop();
        dialogueSource.clip = clip;
        dialogueSource.volume = 6f;
        dialogueSource.Play();
    }

    public void StopDialogue()
    {
        if (dialogueSource != null)
            dialogueSource.Stop();
    }
}
