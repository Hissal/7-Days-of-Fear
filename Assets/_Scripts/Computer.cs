using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Computer : Interactable
{
    [SerializeField] private ComputerUI computerUI;

    [SerializeField] private QuestObjective questObjective;

    private bool questActive = false;

    private void OnEnable()
    {
        TimeManager.OnMorning += SendMorningEmails;
        TimeManager.OnEvening += SendEveningEmails;

        questObjective.OnObjectiveActivated += ObjectiveActivated;
        questObjective.OnObjectiveHighlight += Highlight;
    }
    private void OnDisable()
    {
        TimeManager.OnMorning -= SendMorningEmails;
        TimeManager.OnEvening -= SendEveningEmails;

        questObjective.OnObjectiveActivated -= ObjectiveActivated;
        questObjective.OnObjectiveHighlight -= Highlight;
    }

    private void ObjectiveActivated(bool active)
    {
        questActive = active;
        EmailButton emailButton = computerUI.SetTask();
        if (emailButton != null) StartCoroutine(CompleteWithDelay(emailButton));
        computerUI.OnTaskSuccesful += OnTaskComplete;
    }
    private IEnumerator CompleteWithDelay(EmailButton emailButton)
    {
        yield return new WaitForSeconds(1f);
        computerUI.OnTaskComplete(emailButton);
    }

    private void Highlight()
    {
        OnFocus();
    }

    private void OnTaskComplete()
    {
        computerUI.OnTaskSuccesful -= OnTaskComplete;
        questObjective.OnComplete();
    }

    private void SendMorningEmails()
    {
        computerUI.ActivateEmailMorning();
    }
    private void SendEveningEmails()
    {
        computerUI.ActivateEmailsEvening();
    }

    public override void OnInteract()
    {
        ShowInternetUI();
    }

    private void ShowInternetUI()
    {
        computerUI.transform.SetAsLastSibling();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameManager.Instance.TakeAwayPlayerControl();
        MentalHealth.Instance.PauseDrainage();

        computerUI.gameObject.SetActive(true);
    }

    public void HideInternetUI()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.Instance.GivePlayerControlBack();
        MentalHealth.Instance.ResumeDrainage();

        computerUI.gameObject.SetActive(false);
    }
}
