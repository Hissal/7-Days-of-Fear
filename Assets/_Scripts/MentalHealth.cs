using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class MentalHealth : MonoBehaviour
{
    public static MentalHealth Instance;
    private void Awake()
    {
        currentMentalHealth = maxMentalhealth;
        UpdateVisual();

        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Debug.Log("Creating MentalHealth Instance");
            Instance = this;
        }
    }

    [SerializeField] private Image bar;
    [field: SerializeField] public float maxMentalhealth { get; private set; }
    public float currentMentalHealth { get; private set;}

    [SerializeField] private Color highMentalHealthColor;
    [SerializeField] private Color lowMentalHealthColor;
    [SerializeField] private Color takeDamageColor;
    [SerializeField] private Color healColor;

    public event Action OnMentalHealthReachZero = delegate { };
    public event Action<float> OnMentalHealthIncrease = delegate { };

    private bool isMentalHealthZero;

    private bool paused;

    private float mentalHealthDrainPerSecond;

    [SerializeField] private float drainageOnDay1;
    [SerializeField] private float drainageOnDay2;
    [SerializeField] private float drainageOnDay3;
    [SerializeField] private float drainageOnDay4;
    [SerializeField] private float drainageOnDay5;
    [SerializeField] private float drainageOnDay6;
    [SerializeField] private float drainageOnDay7;

    [SerializeField] private float mentalHealthOnDay1 = 100f;
    [SerializeField] private float mentalHealthOnDay2 = 100f;
    [SerializeField] private float mentalHealthOnDay3 = 80f;
    [SerializeField] private float mentalHealthOnDay4 = 70f;
    [SerializeField] private float mentalHealthOnDay5 = 60f;
    [SerializeField] private float mentalHealthOnDay6 = 50f;
    [SerializeField] private float mentalHealthOnDay7 = 40f;

    public bool mentalHealthDrainagePauseManual;

    public bool fading;

    [SerializeField] private Volume volume;

    IEnumerator FailureCheckerLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            if (GameManager.Instance.enemyActive || mentalHealthDrainagePauseManual)
            {
                Debug.Log("Pausing Drainage In FailureCheckerLoop");
                PauseDrainage();
            }
            else
            {
                Debug.Log("Resuming Drainage In FailureCheckerLoop");
                ResumeDrainage();
            }

            Debug.Log("MentalHealth Drainage Paused = " + paused + "Drainage is set to: " + mentalHealthDrainPerSecond);
        }
    }

    private void OnEnable()
    {
        TimeManager.OnDayChanged += SetDrainageAmount;
        StartCoroutine(FailureCheckerLoop());
    }
    private void OnDisable()
    {
        TimeManager.OnDayChanged -= SetDrainageAmount;
        StopAllCoroutines();
    }

    private void Update()
    {
       if (currentMentalHealth != 0f && !paused) ReduceMentalHealth(mentalHealthDrainPerSecond * Time.deltaTime, true);
    }

    public void PauseDrainage()
    {
        paused = true;
    }
    public void ResumeDrainage()
    {
        paused = false;
    }
    private void SetDrainageAmount(int day)
    {
        switch (day)
        {
            case 1:
                mentalHealthDrainPerSecond = drainageOnDay1;
                currentMentalHealth = mentalHealthOnDay1;
                break;
            case 2:
                mentalHealthDrainPerSecond = drainageOnDay2;
                currentMentalHealth = mentalHealthOnDay2;
                break;
            case 3:
                mentalHealthDrainPerSecond = drainageOnDay3;
                currentMentalHealth = mentalHealthOnDay3;
                break;
            case 4:
                mentalHealthDrainPerSecond = drainageOnDay4;
                currentMentalHealth = mentalHealthOnDay4;
                break;
            case 5:
                mentalHealthDrainPerSecond = drainageOnDay5;
                currentMentalHealth = mentalHealthOnDay5;
                break;
            case 6:
                mentalHealthDrainPerSecond = drainageOnDay6;
                currentMentalHealth = mentalHealthOnDay6;
                break;
            case 7:
                mentalHealthDrainPerSecond = drainageOnDay7;
                currentMentalHealth = mentalHealthOnDay7;
                break;
            default:
                mentalHealthDrainPerSecond = drainageOnDay1;
                currentMentalHealth = mentalHealthOnDay1;
                break;
        }
    }

    public void ReduceMentalHealth(float amount, bool drainage)
    {
        if (!drainage) StartCoroutine(DamageFadeRoutine());

        currentMentalHealth -= amount;

        if (currentMentalHealth <= 0)
        {
            currentMentalHealth = 0;

            if (!isMentalHealthZero)
            {
                isMentalHealthZero = true;
                OnMentalHealthReachZero?.Invoke();
            }
        }
        else if (currentMentalHealth > 0)
        {
            isMentalHealthZero = false;
        }

        UpdateVisual();
    }

    /// <summary>
    /// Increases the current mental health by the specified amount.
    /// </summary>
    /// <param name="amount">The amount to increase the mental health by.</param>
    /// <returns>The updated value of the current mental health.</returns>
    public float IncreaseMentalHealth(float amount, bool task = false)
    {
        if (task) StartCoroutine(HealFadeRoutine());

        currentMentalHealth += amount;
        if (currentMentalHealth > maxMentalhealth) currentMentalHealth = maxMentalhealth;
        OnMentalHealthIncrease?.Invoke(currentMentalHealth);
        UpdateVisual();
        return currentMentalHealth;
    }

    public void SetMentalHealth(float amount)
    {
        currentMentalHealth = amount;

        if (currentMentalHealth > maxMentalhealth) currentMentalHealth = maxMentalhealth;

        if (currentMentalHealth <= 0 && !isMentalHealthZero)
        {
            currentMentalHealth = 0;
            isMentalHealthZero = true;
            OnMentalHealthReachZero?.Invoke();
        }
        else if (currentMentalHealth > 0)
        {
            isMentalHealthZero = false;
        }

        if (amount > currentMentalHealth) OnMentalHealthIncrease?.Invoke(amount);

        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (fading) return;

        float mentalHealthPrecentage = currentMentalHealth / maxMentalhealth;
        if (currentMentalHealth <= 0f)
        {
            mentalHealthPrecentage = 0f;
        }

        bar.color = Color.Lerp(lowMentalHealthColor, highMentalHealthColor, mentalHealthPrecentage);
        volume.weight = Mathf.Lerp(1f, 0f, mentalHealthPrecentage);
    }

    private IEnumerator DamageFadeRoutine()
    {
        if (fading) yield break;
        fading = true;

        Color originalColor = bar.color;
        float elapsedTime = 0f;
        float fadeDuration = 0.05f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            bar.color = Color.Lerp(originalColor, takeDamageColor, t);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            bar.color = Color.Lerp(takeDamageColor, originalColor, t);
            yield return null;
        }

        fading = false;
    }
    private IEnumerator HealFadeRoutine()
    {
        if (fading) yield break;
        fading = true;

        Color originalColor = bar.color;
        float elapsedTime = 0f;
        float fadeDuration = 0.05f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            bar.color = Color.Lerp(originalColor, healColor, t);
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;
            bar.color = Color.Lerp(healColor, originalColor, t);
            yield return null;
        }

        fading = false;
    }
}
