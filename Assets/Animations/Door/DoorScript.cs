using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : Interactable
{
    [SerializeField] private Animator anim;

    private void Start()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
            Debug.LogWarning("Animator on " + gameObject + "is null... Using GetComponent");
        }
    }

    public override void OnFocus()
    {
        base.OnFocus();
    }

    public override void OnInteract()
    {
        anim.SetTrigger("InteractionTrigger");
    }

    public override void OnLoseFocus()
    {
        base.OnLoseFocus();
    }
}
