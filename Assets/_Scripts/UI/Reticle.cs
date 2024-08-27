using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    private static Reticle instance;

    [SerializeField] private Image reticle;
    [SerializeField] private Sprite focusedReticle;
    [SerializeField] private Sprite unFocusedReticle;

    private void Focus()
    {
        reticle.sprite = focusedReticle;
    }

    private void UnFocus()
    {
        reticle.sprite = unFocusedReticle;
    }

    public static void Focus_Static()
    {
        instance.Focus();
    }

    public static void UnFocus_Static()
    {
        instance.UnFocus();
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            reticle.sprite = unFocusedReticle;
        }
    }
}
