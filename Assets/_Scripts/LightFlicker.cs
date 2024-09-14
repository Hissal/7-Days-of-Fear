using Assets._Scripts.Managers_Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Tooltip("External light to flicker; you can leave this null if you attach script to a light")]
    public new Light light;
    [field: SerializeField] public LightFlicker[] childLights { get; private set; }
    [Tooltip("Minimum random light intensity")]
    public float flickerMinIntensity = 0f;
    [Tooltip("Maximum random light intensity")]
    public float flickerMaxIntensity = 1f;
    [Tooltip("How much to smooth out the randomness; lower values = sparks, higher = lantern")]
    [Range(1, 50)]
    public int smoothing = 5;

    private float lightIntenisty;

    // Continuous average calculation via FIFO queue
    // Saves us iterating every time we update, we just change by the delta
    Queue<float> smoothQueue;
    float lastSum = 0;

    [SerializeField] AudioClip flickerSound;
    public bool flicker;
    private bool flickering;
    [SerializeField] float flickeringVolume = 0.05f;

    AudioSource audioSource;
    /// <summary>
    /// Reset the randomness and start again. You usually don't need to call
    /// this, deactivating/reactivating is usually fine but if you want a strict
    /// restart you can do.
    /// </summary>
    public void Reset()
    {
        smoothQueue.Clear();
        lastSum = 0;
    }

    void Awake()
    {
        smoothQueue = new Queue<float>(smoothing);
        // External or internal light?
        if (light == null)
        {
            light = GetComponent<Light>();
        }

        lightIntenisty = light.intensity;
        TurnOffLight();
    }

    void Update()
    {
        if (light == null)
            return;


        if (flicker && !flickering)
        {
            flickering = true;
            StopAllCoroutines();
            StartCoroutine(FadeInFlickeringSound());
        }
        else if (!flicker && flickering)
        {
            flickering = false;
            StopAllCoroutines();
            if (audioSource) StartCoroutine(FadeOutFlickeringSound());
        }

        if (!flicker)
        {
            return;
        }

        // pop off an item if too big
        while (smoothQueue.Count >= smoothing)
        {
            lastSum -= smoothQueue.Dequeue();
        }

        // Generate random new item, calculate new average
        float newVal = Random.Range(flickerMinIntensity, flickerMaxIntensity);
        smoothQueue.Enqueue(newVal);
        lastSum += newVal;

        // Calculate new smoothed average
        light.intensity = lastSum / (float)smoothQueue.Count;
    }

    IEnumerator FadeInFlickeringSound()
    {
        float fadeTime = 0.2f;
        float elapsedTime = 0f;
        audioSource = AudioManager.Instance.PlayAudioClip(flickerSound, transform.position, 0f, true);

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, flickeringVolume, elapsedTime / fadeTime);
            yield return null;
        }
    }
    IEnumerator FadeOutFlickeringSound()
    {
        float fadeTime = 0.2f;
        float elapsedTime = 0f;

        float volume = audioSource.volume;

        while (elapsedTime < fadeTime && audioSource)
        {
            elapsedTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(volume, 0f, elapsedTime / fadeTime);
            yield return null;
        }
        AudioManager.Instance.StopAudioClip(audioSource);
    }

    public void CheckChildLights()
    {
        if (childLights.Length > 0)
        {
            foreach (LightFlicker childLight in childLights)
            {
                if (light.intensity > 0f)
                {
                    childLight.TurnOnLight();
                }
                else
                {
                    childLight.TurnOffLight();
                }
            }
        }
    }

    public void TurnOffLight()
    {
        light.intensity = 0f;
    }
    public void TurnOnLight()
    {
        light.intensity = lightIntenisty;
    }
}
