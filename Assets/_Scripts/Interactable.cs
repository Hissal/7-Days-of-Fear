using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected Outline outline;
    [field: SerializeField] public bool canUseLeftMouse { get; protected set; } = false;


    private void Awake()
    {
        gameObject.layer = 9;

        if (outline == null)
        {
            outline = GetComponent<Outline>();
            Debug.LogWarning("Outline on " + gameObject + " is null... Using GetComponent");
        }

        outline.enabled = false;
    }

    public virtual void OnFocus() { outline.enabled = true; Reticle.Focus_Static(); }

    public virtual void OnLoseFocus() { outline.enabled = false; Reticle.UnFocus_Static(); }

    public virtual void OnInteract() { }

}
