using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : Interactable
{
    [SerializeField] private GameObject computerScreenUI;

    public override void OnInteract()
    {
        ShowInternetUI();
    }

    private void ShowInternetUI()
    {
        //TODO "Pause" game during computer gamings

        computerScreenUI.transform.SetAsLastSibling();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameManager.Instance.TakeAwayPlayerControl();

        computerScreenUI.SetActive(true);
    }

    public void HideInternetUI()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.Instance.GivePlayerControlBack();

        computerScreenUI.SetActive(false);
    }
}
