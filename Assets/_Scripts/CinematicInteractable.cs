using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicInteractable : Interactable
{
    [SerializeField] private Cinematic cinematic;
    
    public override void OnInteract()
    {
        CinematicManager.Instance.PlayCinematic(cinematic);
        base.OnInteract();
    }

    public override void OnFocus()
    {
        base.OnFocus();
    }

    public override void OnLoseFocus()
    {
        base.OnLoseFocus();
    }
}
