using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MentalHealth : MonoBehaviour
{
    [SerializeField] private Image bar;
    [SerializeField] private float maxMentalhealth;
    public float currentMentalHealth { get; private set;}

    [SerializeField] private float mentalHealthDrainPerSecond;

    [SerializeField] private Color highMentalHealthColor;
    [SerializeField] private Color lowMentalHealthColor;

    public event Action OnMentalHealthReachZero = delegate { };

    private void Update()
    {
       if (currentMentalHealth != 0f) ReduceMentalHealth(mentalHealthDrainPerSecond * Time.deltaTime);
    }

    public void ReduceMentalHealth(float amount)
    {
        currentMentalHealth -= amount;

        if (currentMentalHealth <= 0)
        {
            OnMentalHealthReachZero?.Invoke();
        }

        UpdateVisual();
    }

    public void IncreaseMentalHealth(float amount)
    {
        currentMentalHealth += amount;

        if (currentMentalHealth > maxMentalhealth) currentMentalHealth = maxMentalhealth;

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
