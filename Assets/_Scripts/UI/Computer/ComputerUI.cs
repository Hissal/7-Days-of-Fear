using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerUI : MonoBehaviour
{
    public event System.Action OnTaskSuccesful = delegate { };

    [SerializeField] private EmailButton[] emailButtons;

    private bool ongoingTask = false;

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

        foreach (var emailButton in emailButtons)
        {
            if (emailButton.appearanceDay == TimeManager.day && emailButton.isPartOfQuest)
            {
                if (emailButton.read) StartCoroutine(OnTaskCompleteWithDelay(emailButton));
                else
                {
                    emailButton.OnEmailOpened += OnTaskComplete;
                    ongoingTask = true;
                }
            }
        }
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
        ongoingTask = false;
        emailButton.OnEmailOpened -= OnTaskComplete;
        OnTaskSuccesful?.Invoke();
    }

}
