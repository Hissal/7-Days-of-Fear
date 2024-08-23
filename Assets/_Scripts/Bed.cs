using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : Interactable
{
    public override void OnInteract()
    {
        TimeManager.SetTime(TimeManager.day + 1, 7, 0);
    }
}
