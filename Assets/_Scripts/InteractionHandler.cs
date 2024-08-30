using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionHandler : MonoBehaviour
{
    private Camera cam;

    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private Vector3 interactionRayPoint = default;
    [SerializeField] private float interactionDistance;
    [SerializeField] private LayerMask interactionLayer;
    private Interactable currentInteractable = null;

    public bool canInteract = true;

    private void Start()
    {
       cam = GetComponent<FirstPersonController>().playerCamera;
    }

    private void Update()
    {
        if (!canInteract) return;

        HandleInteractionCheck();
        HandleInteractionInput();
    }

    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(cam.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            if (hit.collider.gameObject.layer == 9 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
                else
                {
                    Debug.LogWarning("Trying to set a non interactable as currentInteractable... " + hit.collider.gameObject);
                }
            }
            else if (hit.collider.gameObject.layer != 9 && currentInteractable)
            {
                currentInteractable.OnLoseFocus();
                currentInteractable = null;
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
