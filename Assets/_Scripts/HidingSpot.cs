using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HidingSpot : MonoBehaviour
{
    [SerializeField] private GameObject miniGame;
    [SerializeField, Tooltip("Location For Enemy To Walk To")] private Transform front;

    public event Action<Transform> onPlayerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == GameManager.Instance.playerTransform)
        {
            miniGame.SetActive(true);
            onPlayerEnter.Invoke(front);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == GameManager.Instance.playerTransform)
        {
            miniGame.SetActive(false);
        }
    }
}
