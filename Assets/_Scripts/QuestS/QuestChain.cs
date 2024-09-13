using System.Collections.Generic;
using UnityEngine;

public class QuestChain : MonoBehaviour
{
    [SerializeField] private List<Quest> quests;
    private Quest currentQuest;

    [SerializeField] private int[] activeDays;

    [SerializeField] private bool activeInMorning;
    [SerializeField] private bool activeInEvening;
    private bool active;

    private void OnEnable()
    {
        TimeManager.OnDayChanged += CheckDay;
        if (activeInMorning) TimeManager.OnMorning += TryStartQuestChain;
        if (activeInEvening) TimeManager.OnEvening += TryStartQuestChain;
    }

    private void OnDisable()
    {
        TimeManager.OnDayChanged -= CheckDay;
        if (activeInMorning) TimeManager.OnMorning -= TryStartQuestChain;
        if (activeInEvening) TimeManager.OnEvening -= TryStartQuestChain;
    }

    private void TryStartQuestChain()
    {
        if (active)
        {
            StartQuestChain();
        }
    }

    void CheckDay(int day)
    {
        print("Checking Day");

        if (IsDayActive(day))
        {
            active = true;
        }
        else
        {
            active = false;
        }
    }

    private bool IsDayActive(int day)
    {
        foreach (int activeDay in activeDays)
        {
            if (day == activeDay)
            {
                return true;
            }
        }
        return false;
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
