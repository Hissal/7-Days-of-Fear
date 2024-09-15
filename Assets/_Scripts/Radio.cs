using Assets._Scripts.Managers_Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : Interactable
{
    private static Radio instance;

    private void Awake()
    {
        instance = this;
    }

    bool isOn = false;

    [SerializeField] private AudioSource radioAudioSource;
    [SerializeField] private AudioClip[] radioClips;
    [SerializeField] private float volume = 0.4f;
    [SerializeField] private AudioClip turnOnSound;
    [SerializeField] private AudioClip turnOffSound;

    private void OnEnable()
    {
        TimeManager.OnDayChanged += SetRandomClip;

        //base.OnLoseFocus();
    }
    private void OnDisable()
    {
        TimeManager.OnDayChanged -= SetRandomClip;
    }

    private void Start()
    {
        radioAudioSource.volume = 0f;
    }

    public override void OnInteract()
    {
        if (radioAudioSource.volume <= 0f)
        {
            isOn = true;
            AudioManager.Instance.PlayAudioClip(turnOnSound, transform.position, 0.2f);
            radioAudioSource.volume = volume;
        }
        else
        {
            isOn = false;
            AudioManager.Instance.PlayAudioClip(turnOffSound, transform.position, 0.2f);
            radioAudioSource.volume = 0f;
        }

        base.OnInteract();
    }

    private void SetRandomClip(int day)
    {
        radioAudioSource.clip = radioClips[Random.Range(0, radioClips.Length)];
        radioAudioSource.Play();
        radioAudioSource.loop = true;
    }

    public static void FadeOutRadio_Static(float time)
    {
        instance.FadeOutRadio(time);
    }
    public static void FadeInRadio_Static(float time)
    {
        instance.FadeInRadio(time);
    }

    public void FadeOutRadio(float time)
    {
        if (isOn)
        {
            StartCoroutine(FadeOut(time));
        }
    }

    public void FadeInRadio(float time)
    {
        if (isOn)
        {
            StartCoroutine(FadeIn(time));

        }
    }

    IEnumerator FadeOut(float time)
    {
        float elapsedTime = 0f;
        float startVolume = volume;
        float targetVolume = 0f;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            radioAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / time);
            yield return null;

        }
    }

    IEnumerator FadeIn(float time)
    {
        float elapsedTime = 0f;
        float startVolume = 0f;
        float targetVolume = volume;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            radioAudioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / time);
            yield return null;
        }

        radioAudioSource.volume = targetVolume;
    }
}
