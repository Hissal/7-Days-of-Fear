using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceController : MonoBehaviour
{
    [field: SerializeField] public float defaultVolume { get; private set; }
    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private AudioClip anbienceDay1;
    [SerializeField] private AudioClip anbienceDay2;
    [SerializeField] private AudioClip anbienceDay3;
    [SerializeField] private AudioClip anbienceDay4;
    [SerializeField] private AudioClip anbienceDay5;
    [SerializeField] private AudioClip anbienceDay6;
    [SerializeField] private AudioClip anbienceDay7;

    [Header("EnmemyActiveAtmosphere")]
    [SerializeField] private AudioSource enemyActiveAtmosphereAudioSource;
    [SerializeField] private AudioClip enemyActiveAtmosphere;
    [SerializeField] private float enemyActiveAtmosphereVolume = 0.1f;

    public static AmbienceController Instance;

    private Coroutine ambienceFadingRoutine = null;
    private Coroutine enemyAtmosphereFadingRoutine = null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        enemyActiveAtmosphereAudioSource.clip = enemyActiveAtmosphere;
        enemyActiveAtmosphereAudioSource.loop = true;
        enemyActiveAtmosphereAudioSource.volume = 0f;
        enemyActiveAtmosphereAudioSource.Play();
    }

    private void OnEnable()
    {
        TimeManager.OnDayChanged += SwitchAmbience;
    }

    private void OnDisable()
    {
        TimeManager.OnDayChanged -= SwitchAmbience;
    }

    public void FadeInScaryAtmosphere()
    {
        if (enemyAtmosphereFadingRoutine != null) StopCoroutine(enemyAtmosphereFadingRoutine);
        enemyAtmosphereFadingRoutine = StartCoroutine(FadeInScaryAtmosphereRoutine());
    }
    public void FadeOutScaryAtmosphere()
    {
        if (enemyAtmosphereFadingRoutine != null) StopCoroutine(enemyAtmosphereFadingRoutine);
        enemyAtmosphereFadingRoutine = StartCoroutine(FadeOutScaryAtmosphereRoutine());
    }

    private IEnumerator FadeInScaryAtmosphereRoutine()
    {
        float fadeTime = 10f;
        float timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            enemyActiveAtmosphereAudioSource.volume = Mathf.Lerp(0f, enemyActiveAtmosphereVolume, timer / fadeTime);
            yield return null;
        }
    }
    private IEnumerator FadeOutScaryAtmosphereRoutine()
    {
        float fadeTime = 10f;
        float timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            enemyActiveAtmosphereAudioSource.volume = Mathf.Lerp(enemyActiveAtmosphereVolume, 0f, timer / fadeTime);
            yield return null;
        }
    }

    private void PlayAmbience()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Play();
        }
    }
    public void PauseAmbience()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Pause();
        }
    }
    public void UnPauseAmbience()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.UnPause();
        }
    }
    private void SetVolume(float volume)
    {
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.volume = volume;
        }
    }

    /// <summary>
    /// Switches the ambience audio clip based on the given day.
    /// </summary>
    /// <param name="day">The day to switch the ambience for.</param>
    public void SwitchAmbience(int day)
    {
        foreach (AudioSource audioSource in audioSources)
        {
            switch (day)
            {
                case 1:
                    audioSource.clip = anbienceDay1;
                    break;
                case 2:
                    audioSource.clip = anbienceDay2;
                    break;
                case 3:
                    audioSource.clip = anbienceDay3;
                    break;
                case 4:
                    audioSource.clip = anbienceDay4;
                    break;
                case 5:
                    audioSource.clip = anbienceDay5;
                    break;
                case 6:
                    audioSource.clip = anbienceDay6;
                    break;
                case 7:
                    audioSource.clip = anbienceDay7;
                    break;
                default:
                    break;
            }
        }

        SetVolume(defaultVolume);
        PlayAmbience();
    }

    /// <summary>
    /// Fades out the ambience audio over a specified duration.
    /// </summary>
    /// <param name="fadeEaseLength">The duration of the fade out effect.</param>
    public void FadeOutAmbience(float fadeEaseLength)
    {
        if (ambienceFadingRoutine != null) StopCoroutine(ambienceFadingRoutine);
        ambienceFadingRoutine = StartCoroutine(FadeOutAmbienceCoroutine(fadeEaseLength));
    }

    /// <summary>
    /// Fades in the ambience audio over a specified duration.
    /// </summary>
    /// <param name="fadeEaseLength">The duration of the fade in effect.</param>
    public void FadeInAmbience(float fadeEaseLength)
    {
        if (ambienceFadingRoutine != null) StopCoroutine(ambienceFadingRoutine);
        ambienceFadingRoutine = StartCoroutine(FadeInAmbienceCoroutine(fadeEaseLength));
    }

    private IEnumerator FadeOutAmbienceCoroutine(float fadeEaseLength)
    {
        float fadeOutTimer = 0f;
        float currentVolume = audioSources[0].volume;
        while (fadeOutTimer < fadeEaseLength)
        {
            fadeOutTimer += Time.deltaTime;
            float volume = Mathf.Lerp(currentVolume, 0f, fadeOutTimer / fadeEaseLength);
            SetVolume(volume);
            yield return null;
        }
       SetVolume(0f);
    }

    private IEnumerator FadeInAmbienceCoroutine(float fadeEaseLength)
    {
        float fadeInTimer = 0f;
        float currentVolume = audioSources[0].volume;
        while (fadeInTimer < fadeEaseLength)
        {
            fadeInTimer += Time.deltaTime;
            float volume = Mathf.Lerp(currentVolume, defaultVolume, fadeInTimer / fadeEaseLength);
            SetVolume(volume);
            yield return null;
        }
        SetVolume(defaultVolume);
    }

    /// <summary>
    /// Lerps the volume of the ambience audio over a specified duration to the desired volume.
    /// </summary>
    /// <param name="lerpingDuration">The duration of the volume lerp effect.</param>
    /// <param name="desiredVolume">The desired volume to lerp to.</param>
    public void LerpAmbienceVolume(float lerpingDuration, float desiredVolume)
    {
        if (ambienceFadingRoutine != null) StopCoroutine(ambienceFadingRoutine);
        ambienceFadingRoutine = StartCoroutine(LerpAmbienceVolumeCoroutine(lerpingDuration, desiredVolume));
    }
    /// <summary>
    /// Lerps the volume of the ambience audio over 0.5 seconds to the desired volume.
    /// </summary>
    /// <param name="desiredVolumePrecentageOfDefault">The desired volume as a percentage of the default volume.</param>
    public void LerpAmbienceVolume(float desiredVolumePrecentageOfDefault)
    {
        float desiredVolume = defaultVolume * desiredVolumePrecentageOfDefault;
        StartCoroutine(LerpAmbienceVolumeCoroutine(0.5f, desiredVolume));
    }

    private IEnumerator LerpAmbienceVolumeCoroutine(float lerpingDuration, float desiredVolume)
    {
        float lerpTimer = 0f;
        float currentVolume = audioSources[0].volume;
        while (lerpTimer < lerpingDuration)
        {
            lerpTimer += Time.deltaTime;
            float volume = Mathf.Lerp(currentVolume, desiredVolume, lerpTimer / lerpingDuration);
            SetVolume(volume);
            yield return null;
        }
        SetVolume(desiredVolume);
    }
}
