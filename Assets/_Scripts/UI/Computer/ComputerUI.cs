using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerUI : MonoBehaviour
{
    public event System.Action OnTaskSuccesful = delegate { };

    [SerializeField] private EmailButton[] emailButtons;

    private bool ongoingTask = false;
    private bool taskDone = true;
    private bool emailFound = false;

    private void OnEnable()
    {
        if (!emailFound && ongoingTask)
            OnTaskComplete(null);
    }
    private void OnDisable()
    {
        if (taskDone)
        {
            taskDone = false;
            OnTaskSuccesful?.Invoke();
        }
    }

    public void ActivateEmailMorning()
    {
        CheckEmails(EmailButton.EmailType.Morning);
    }
    public void ActivateEmailsEvening()
    {
        CheckEmails(EmailButton.EmailType.Evening);
    }

    public void SetTask()
    {
        if (ongoingTask) return;
        ongoingTask = true;

        bool emailFound = false;

        foreach (var emailButton in emailButtons)
        {
            if (emailButton.appearanceDay == TimeManager.day && emailButton.isPartOfQuest)
            {
                emailFound = true;
                if (emailButton.read) StartCoroutine(OnTaskCompleteWithDelay(emailButton));
                else
                {
                    taskDone = false;
                    emailButton.OnEmailOpened += OnTaskComplete;
                }
            }
        }

        this.emailFound = emailFound;
    }

    private IEnumerator OnTaskCompleteWithDelay(EmailButton emailButton)
    {
        yield return new WaitForSeconds(1f);
        OnTaskComplete(emailButton);
    }

    private void CheckEmails(EmailButton.EmailType emailType)
    {
        print("Checking Emails: " + emailType);
        foreach (EmailButton emailButton in emailButtons)
        {
            if (emailButton.appearanceDay == TimeManager.day && emailButton.emailType == emailType)
            {
                emailButton.gameObject.SetActive(true);
            }
            if (emailButton.appearanceDay > TimeManager.day ||
                    (emailButton.appearanceDay == TimeManager.day &&
                    emailType == EmailButton.EmailType.Morning &&
                    emailButton.emailType == EmailButton.EmailType.Evening))
            {
                emailButton.gameObject.SetActive(false);
            }
        }
    }

    public void OpenEmail(EmailButton emailButton)
    {
        foreach (var button in emailButtons)
        {
            button.CloseEmail();
        }

        emailButton.OpenEmail();
    }

    public void OnTaskComplete(EmailButton emailButton)
    {
        taskDone = true;
        ongoingTask = false;
        if (emailButton != null) emailButton.OnEmailOpened -= OnTaskComplete;
    }

}
