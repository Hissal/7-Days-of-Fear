using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reticle : MonoBehaviour
{
    private static Reticle instance;

    [SerializeField] private GameObject focusedReticle;
    [SerializeField] private GameObject unFocusedReticle;
    [SerializeField] private GameObject grabbingReticle;

    private Image focusedImage;
    private Image unFocusedImage;
    private Image grabbingImage;

    private void Start()
    {
        focusedImage = focusedReticle.GetComponent<Image>();
        unFocusedImage = unFocusedReticle.GetComponent<Image>();
        grabbingImage = grabbingReticle.GetComponent<Image>();
    }

    private void Focus()
    {
        focusedImage.enabled = true;
        unFocusedImage.enabled = false;
        grabbingImage.enabled = false;
    }

    private void UnFocus()
    {
        unFocusedImage.enabled = true;
        focusedImage.enabled = false;
        grabbingImage.enabled = false;
    }

    private void Grab()
    {
        grabbingImage.enabled = true;
        focusedImage.enabled = false;
        unFocusedImage.enabled = false;
    }

    private void HideReticle()
    {
        focusedReticle.SetActive(false);
        unFocusedReticle.SetActive(false);
        grabbingReticle.SetActive(false);
    }

    private void ShowReticle()
    {
        focusedReticle.SetActive(true);
        unFocusedReticle.SetActive(true);
        grabbingReticle.SetActive(true);
    }

    public static void HideReticle_Static()
    {
        instance.HideReticle();
    }
    public static void ShowReticle_Static()
    {
        instance.ShowReticle();
    }

    public static void Focus_Static()
    {
        instance.Focus();
    }

    public static void UnFocus_Static()
    {
        instance.UnFocus();
    }

    public static void Grab_Static()
    {
        instance.Grab();
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
        }
    }
}
