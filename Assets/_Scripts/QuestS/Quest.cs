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
    private bool oneObjectiveAtOnce;
    [SerializeField, Tooltip("If true, Objectives will be highlighted")]
    private bool highlightObjectives;
    private QuestObjective currentObjective;

    public event System.Action OnObjectiveChanged = delegate { };
    public event System.Action OnQuestComplete = delegate { };

    public bool paused { get; private set; }

    [field: SerializeField] public bool overrideCurrentQuest { get; private set; }

    public string Description
    {
        get
        {
            if (oneObjectiveAtOnce && currentObjective != null) return currentObjective.description;
            return description;
        }
    }

    public void StartQuest()
    {
        activeObjectives = new List<QuestObjective>();

        if (oneObjectiveAtOnce)
        {
            QuestObjective objective = objectives[0];

            ActivateObjective(objective);
        }
        else
        {
            foreach (QuestObjective objective in objectives)
            {
                ActivateObjective(objective);
            }

            currentObjective = null;
            OnObjectiveChanged?.Invoke();
        }
    }

    private void HighlightObjective(QuestObjective objective)
    {
        Debug.Log("Highlighting Objective: " + objective);
        objective.Highlight();
    }

    private void ObjectiveCompleted(QuestObjective objective)
    {
        Debug.Log("Objective Completed: " + objective);

        DeActivateObjective(objective);

        if (oneObjectiveAtOnce)
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
            ActivateObjective(nextObjective);
        }
        else
        {
            CompleteQuest();
        }
    }

    private void ReActivatePreviousObjective(QuestObjective objective)
    {
        if (currentObjective == objective) return;

        Debug.Log("Objective Condition Broken: " + objective);

        objectives.Remove(objective);
        objectives.Insert(objectives.IndexOf(currentObjective), objective);
        DeActivateObjective(currentObjective);
        ActivateObjective(objective);
    }

    private void ActivateObjective(QuestObjective objective)
    {
        if (activeObjectives.Contains(objective)) return;

        objective.OnConditionBroken += ReActivatePreviousObjective;
        objective.OnObjectiveComplete += ObjectiveCompleted;
        if (highlightObjectives) HighlightObjective(objective);
        objective.Activate();
        activeObjectives.Add(objective);

        if (!oneObjectiveAtOnce) return;

        currentObjective = objective;
        OnObjectiveChanged?.Invoke();
    }
    private void DeActivateObjective(QuestObjective objective)
    {
        objective.OnObjectiveComplete -= ObjectiveCompleted;
        objective.OnConditionBroken -= ReActivatePreviousObjective;
        objective.DeActivate();
        activeObjectives.Remove(objective);
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

        Debug.Log("Quest Completed: " + this);
        isComplete = true;
        OnQuestComplete?.Invoke();
        currentObjective = null;
    }

    public void EndQuest()
    {
        if (oneObjectiveAtOnce)
        {
            DeActivateObjective(currentObjective);
            currentObjective = null;
        }
        else
        {
            foreach (QuestObjective objective in objectives)
            {
                DeActivateObjective(objective);
            }
        }
    }

    public Quest(List<QuestObjective> objectives)
    {
        this.objectives = objectives;
    }
}
