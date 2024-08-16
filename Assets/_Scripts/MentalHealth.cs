using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MentalHealth : MonoBehaviour
{
    [SerializeField] private int maxMentalhealth;
    public int currentMentalHealth { get; private set;}

    public event Action OnMentalHealthDrained = delegate { };

    public void DrainMentalHealth(int amount)
    {
        currentMentalHealth -= amount;

        if (currentMentalHealth <= 0)
        {
            OnMentalHealthDrained.Invoke();
        }
    }

    public void IncreaseMentalHealth(int amount)
    {
        currentMentalHealth += amount;

        if (currentMentalHealth > maxMentalhealth) currentMentalHealth = maxMentalhealth;
    }
}
