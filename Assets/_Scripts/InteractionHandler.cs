using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    private Camera cam;

    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [SerializeField] private float interactionDistance;
    [SerializeField] private LayerMask interactionLayer;
    private Interactable currentInteractable;

    private void Start()
    {
       cam = GetComponent<FirstPersonController>().playerCamera;
    }

    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(cam.ViewportPointToRay(Vector3.zero), out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.gameObject.layer == 9 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
            }
        }
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactionKey) && currentInteractable != null)
        {
            currentInteractable.OnInteract();
        }
    }
}
