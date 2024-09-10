using System.Collections.Generic;
using UnityEngine;

public class QuestChain : MonoBehaviour
{
    [SerializeField] private List<Quest> quests;
    private Quest currentQuest;

    [SerializeField] private int activeDay;

    private void Awake()
    {
        TimeManager.OnDayChanged += CheckDay;
    }
    
    void CheckDay(int day)
    {
        if (day == activeDay)
        {
            StartQuestChain();
        }
    }

    public void StartQuestChain()
    {
        print("Starting Quest Chain");
        currentQuest = quests[0];
        currentQuest.OnQuestComplete += QuestCompleted;
        QuestSystem.Instance.StartQuest(currentQuest);
    }

    private void QuestCompleted()
    {
        int index = quests.IndexOf(currentQuest);
        if (index + 1 < quests.Count)
        {
            currentQuest.OnQuestComplete -= QuestCompleted;
            currentQuest = quests[index + 1];
            currentQuest.OnQuestComplete += QuestCompleted;
            QuestSystem.Instance.StartQuest(currentQuest);
        }
        else
        {
            QuestChainCompleted();
        }
    }

    private void QuestChainCompleted()
    {
        Debug.Log("Quest Chain Completed");
    }
}
