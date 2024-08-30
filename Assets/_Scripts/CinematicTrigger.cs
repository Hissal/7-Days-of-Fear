using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicTrigger : MonoBehaviour
{
    [SerializeField] private Cinematic cinematic;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FirstPersonController>() != null)
        {
            CinematicManager.Instance.PlayCinematic(cinematic);
            this.gameObject.SetActive(false);
        }
    }
}
