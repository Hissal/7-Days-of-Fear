using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HidingSpot : MonoBehaviour
{
    [SerializeField] private GameObject miniGame;
    [SerializeField, Tooltip("Location For Enemy To Walk To")] private Transform front;

    public event Action<Transform, HidingSpot> onPlayerEnter = delegate { };
    public event Action<HidingSpot> onPlayerExit = delegate { };

    [field: SerializeField] public List<DoorOpener> hidingSpotDoors { get; private set; }

    public bool playerInside { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == GameManager.Instance.playerTransform)
        {
            playerInside = true;
            miniGame.SetActive(true);
            onPlayerEnter?.Invoke(front, this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform == GameManager.Instance.playerTransform)
        {
            playerInside = false;
            miniGame.SetActive(false);
            onPlayerExit?.Invoke(this);
        }
    }
}
