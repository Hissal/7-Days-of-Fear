using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestSystem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questTMP;
    private Quest currentQuest;
    private List<Quest> nextQuests = new List<Quest>();

    public void StartQuest(Quest quest)
    {
        if (currentQuest != null && !quest.overrideCurrentQuest)
        {
            print("Adding quest to nextQuests");
            nextQuests.Add(quest);
            return;
        }
        else if (currentQuest != null && quest.overrideCurrentQuest)
        {
            currentQuest.OnObjectiveChanged -= UpdateQuestText;
            currentQuest.OnQuestComplete -= QuestCompleted;
            currentQuest.EndQuest();
            currentQuest = null;
        }

        currentQuest = quest;
        currentQuest.OnObjectiveChanged += UpdateQuestText;
        currentQuest.OnQuestComplete += QuestCompleted;
        currentQuest.StartQuest();
    }

    private void UpdateQuestText()
    {
        questTMP.text = currentQuest.Description;
    }

    private void QuestCompleted()
    {
        currentQuest.OnObjectiveChanged -= UpdateQuestText;
        currentQuest.OnQuestComplete -= QuestCompleted;
        currentQuest = null;
        if (nextQuests.Count > 0)
        {
            StartQuest(nextQuests[0]);
            nextQuests.RemoveAt(0);
        }
        else
        {
            questTMP.text = "";
        }
    }

    public void PauseCurrentQuest(string newQuestText)
    {
        if (newQuestText != "")
        {
            questTMP.text = newQuestText;
        }
        currentQuest.PauseQuest();
    }
    public void ResumeCurrentQuest()
    {
        currentQuest.ResumeQuest();
        questTMP.text += currentQuest.Description;
    }

    public static QuestSystem Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
