using System;
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

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The EnmailButton if email has been read otherwise returns null</returns>
    public EmailButton SetTask()
    {
        if (ongoingTask) return null;
        ongoingTask = true;

        bool emailFound = false;
        EmailButton readEmail = null;

        foreach (var emailButton in emailButtons)
        {
            if (emailButton.appearanceDay == TimeManager.day && emailButton.isPartOfQuest)
            {
                emailFound = true;
                if (emailButton.read) readEmail = emailButton;
                else
                {
                    readEmail = null;
                    taskDone = false;
                    emailButton.OnEmailOpened += OnTaskComplete;
                }
            }
        }

        this.emailFound = emailFound;

        return readEmail;
    }

    private void CheckEmails(EmailButton.EmailType emailType)
    {
        print("Checking Emails: " + emailType);
        foreach (EmailButton emailButton in emailButtons)
        {
            if (emailButton.diseappearAfterRead && emailButton.read) continue;

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

            if (PlayerPrefs.GetInt("HiJohn") == 1 && emailButton.hiJohn && PlayerPrefs.GetInt("Retry") == 1)
            {
                if (TimeManager.day == emailButton.appearanceDay && emailButton.emailType == emailType)
                {
                    emailButton.gameObject.SetActive(true);
                }
                else
                {
                    emailButton.gameObject.SetActive(false);
                }
            }
            else if (PlayerPrefs.GetInt("Smiley") == 1 && emailButton.smiley && PlayerPrefs.GetInt("Retry") == 1)
            {
                if (TimeManager.day == emailButton.appearanceDay && emailButton.emailType == emailType)
                {
                    emailButton.gameObject.SetActive(true);
                }
                else
                {
                    emailButton.gameObject.SetActive(false);
                }
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
