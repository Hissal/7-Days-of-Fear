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

    public event Action OnMentalHealthReachZero = delegate { };

    public void ReduceMentalHealth(float amount)
    {
        currentMentalHealth -= amount;

        if (currentMentalHealth <= 0)
        {
            OnMentalHealthReachZero.Invoke();
        }

        UpdateBar();
    }

    public void IncreaseMentalHealth(float amount)
    {
        currentMentalHealth += amount;

        if (currentMentalHealth > maxMentalhealth) currentMentalHealth = maxMentalhealth;

        UpdateBar();
    }

    private void UpdateBar()
    {
        bar.fillAmount = currentMentalHealth / maxMentalhealth;
    }

    public static MentalHealth Instance;

    private void Awake()
    {
        currentMentalHealth = maxMentalhealth;
        UpdateBar();

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
