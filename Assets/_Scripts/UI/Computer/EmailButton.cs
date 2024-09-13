using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailButton : MonoBehaviour
{
    [field: SerializeField] public bool diseappearAfterRead { get; private set; }
    [field: SerializeField] public bool isPartOfQuest { get; private set; }
    [field: SerializeField] public GameObject attatchedEmail { get; private set; }
    [field: SerializeField] public int appearanceDay { get; private set; }
    public enum EmailType { Morning, Evening }
    [field: SerializeField] public EmailType emailType { get; private set; }

    public event System.Action<EmailButton> OnEmailOpened = delegate { };
    [field: SerializeField] public bool hiJohn = false;
    [field: SerializeField] public bool smiley = false;

    public bool read { get; private set; } = false;

    public void OpenEmail()
    {
        if (attatchedEmail != null) attatchedEmail.SetActive(true);

        OnEmailOpened?.Invoke(this);
        read = true;
        if (diseappearAfterRead)
        {
            if (hiJohn)
            {
                PlayerPrefs.SetInt("HiJohn", 1);
            }
            else if (smiley)
            {
                PlayerPrefs.SetInt("Smiley", 1);
            }
            
            gameObject.SetActive(false);
        }
    }
    public void CloseEmail()
    {
        if (attatchedEmail != null) attatchedEmail.SetActive(false);
    }
}
