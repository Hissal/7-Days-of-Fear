using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MentalHealth : MonoBehaviour
{
    [SerializeField] private Image bar;
    [field: SerializeField] public float maxMentalhealth { get; private set; }
    public float currentMentalHealth { get; private set;}

    [SerializeField] private Color highMentalHealthColor;
    [SerializeField] private Color lowMentalHealthColor;

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


    private void OnEnable()
    {
        TimeManager.OnDayChanged += SetDrainageAmount;
    }
    private void Update()
    {
       if (currentMentalHealth != 0f && !paused) ReduceMentalHealth(mentalHealthDrainPerSecond * Time.deltaTime);
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
                break;
            case 2:
                mentalHealthDrainPerSecond = drainageOnDay2;
                break;
            case 3:
                mentalHealthDrainPerSecond = drainageOnDay3;
                break;
            case 4:
                mentalHealthDrainPerSecond = drainageOnDay4;
                break;
            case 5:
                mentalHealthDrainPerSecond = drainageOnDay5;
                break;
            case 6:
                mentalHealthDrainPerSecond = drainageOnDay6;
                break;
            case 7:
                mentalHealthDrainPerSecond = drainageOnDay7;
                break;
            default:
                mentalHealthDrainPerSecond = drainageOnDay1;
                break;
        }
    }

    public void ReduceMentalHealth(float amount)
    {
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
    public float IncreaseMentalHealth(float amount)
    {
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
        float mentalHealthPrecentage = currentMentalHealth / maxMentalhealth;
        bar.color = Color.Lerp(lowMentalHealthColor, highMentalHealthColor, mentalHealthPrecentage);
    }

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
            Instance = this;
        }
    }
}
