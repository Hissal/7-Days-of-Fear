using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    [SerializeField]
    private Transform teleportTarget; // The target position to teleport to

    [SerializeField]
    private LayerMask collisionLayer; // The layer mask for collision detection


    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object's layer is in the specified layer mask
        if (((1 << other.gameObject.layer) & collisionLayer) != 0)
        {
            // Teleport the player to the target position
            Transform playerT = GameManager.Instance.playerTransform;
            CharacterController playerController = playerT.GetComponent<CharacterController>();
            playerController.enabled = false;
            playerT.position = teleportTarget.position;
            playerController.enabled = true;
        }
    }
}
