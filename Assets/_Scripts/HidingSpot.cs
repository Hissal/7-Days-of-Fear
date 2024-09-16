using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HidingSpot : MonoBehaviour
{
    [SerializeField] private GameObject miniGame;
    [SerializeField, Tooltip("Location For Enemy To Walk To")] private Transform front;

    [field: SerializeField, Tooltip("How far the enemy can be from center when inspecting closet")]
    public float enemyRange { get; private set; }

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

    public Transform GetFront()
    {
        return front;
    }

    public Vector3 GetFrontPosition()
    {
        return front.position;
    }
    public Quaternion GetFrontRotation()
    {
        return front.rotation;
    }
}
