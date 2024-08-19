using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] protected Outline outline;

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

    public virtual void OnFocus() { outline.enabled = true; }

    public virtual void OnLoseFocus() { outline.enabled = false; }

    public virtual void OnInteract() { }

}
