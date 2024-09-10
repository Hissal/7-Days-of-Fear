using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    [SerializeField] string description;
    [SerializeField] private List<QuestObjective> objectives;
    private List<QuestObjective> activeObjectives = new List<QuestObjective>();

    public bool isComplete { get; private set; }

    [SerializeField, Tooltip("If true, only one objective is active at a time")]
    private bool assisted;
    private QuestObjective currentObjective;

    public event System.Action OnObjectiveChanged = delegate { };
    public event System.Action OnQuestComplete = delegate { };

    public bool paused { get; private set; }

    [field: SerializeField] public bool overrideCurrentQuest { get; private set; }

    public string Description
    {
        get
        {
            if (assisted && currentObjective != null) return currentObjective.description;
            return description;
        }
    }

    public void StartQuest()
    {
        activeObjectives = new List<QuestObjective>();

        if (assisted)
        {
            QuestObjective objective = objectives[0];

            objective.OnObjectiveComplete += ObjectiveCompleted;
            HighlightObjective(objective);
            objective.Activate();
            activeObjectives.Add(objective);

            currentObjective = objective;
        }
        else
        {
            foreach (QuestObjective objective in objectives)
            {
                objective.OnObjectiveComplete += ObjectiveCompleted;
                objective.Activate();
                activeObjectives.Add(objective);
            }
            currentObjective = null;
        }

        OnObjectiveChanged?.Invoke();
    }

    private void HighlightObjective(QuestObjective objective)
    {
        Debug.Log("Highlighting Objective");
        objective.Highlight();
    }

    private void ObjectiveCompleted(QuestObjective objective)
    {
        Debug.Log("Objective Completed");

        activeObjectives.Remove(objective);
        objective.OnObjectiveComplete -= ObjectiveCompleted;

        if (assisted)
        {
            ActivateNextObjective(objective);
        }
        else
        {
            bool allCompleted = true;
            foreach (QuestObjective obj in objectives)
            {
                if (!obj.isComplete)
                {
                    allCompleted = false;
                }
            }

            if (allCompleted)
            {
                CompleteQuest();
            }
        }
    }

    private void ActivateNextObjective(QuestObjective objective)
    {
        int index = objectives.IndexOf(objective);
        if (index + 1 < objectives.Count)
        {
            QuestObjective nextObjective = objectives[index + 1];
            nextObjective.OnObjectiveComplete += ObjectiveCompleted;
            HighlightObjective(nextObjective);
            nextObjective.Activate();
            activeObjectives.Add(objective);

            currentObjective = nextObjective;
            OnObjectiveChanged?.Invoke();
        }
        else
        {
            CompleteQuest();
        }
    }

    public void PauseQuest()
    {
        activeObjectives.ForEach(obj => obj.DeActivate());
        paused = true;
    }
    public void ResumeQuest()
    {
        activeObjectives.ForEach(obj => obj.Activate());
        paused = false;
    }

    private void CompleteQuest()
    {
        EndQuest();

        Debug.Log("Quest Completed");
        isComplete = true;
        OnQuestComplete?.Invoke();
        currentObjective = null;
    }
    
    public void EndQuest()
    {
        if (assisted)
        {
            currentObjective.OnObjectiveComplete -= ObjectiveCompleted;
            currentObjective = null;
        }
        else
        {
            foreach (QuestObjective objective in objectives)
            {
                objective.OnObjectiveComplete -= ObjectiveCompleted;
            }
        }
    }

    public Quest(List<QuestObjective> objectives)
    {
        this.objectives = objectives;
    }
}
