using UnityEngine;

[System.Serializable]
public class QuestObjective : MonoBehaviour
{
    [field: SerializeField] public string description { get; private set; }
    public bool isComplete { get; private set; }
    public bool active { get; private set; }
    public event System.Action<QuestObjective> OnObjectiveComplete = delegate { };
    public event System.Action OnObjectiveHighlight = delegate { };
    public event System.Action<bool> OnObjectiveActivated = delegate { };
    public event System.Action<bool> OnObjectivePaused = delegate { };

    public event System.Action<QuestObjective> OnConditionBroken = delegate { };

    public bool paused { get; private set; }

    public void Activate()
    {
        active = true;
        isComplete = false;
        OnObjectiveActivated?.Invoke(true);
    }
    public void DeActivate()
    {
        active = false;
        OnObjectiveActivated?.Invoke(false);
    }

    public void OnComplete()
    {
        isComplete = true;
        OnObjectiveComplete?.Invoke(this);
    }

    public void Pause()
    {
        paused = true;
        OnObjectivePaused?.Invoke(true);
    }
    public void UnPause()
    {
        paused = false;
        OnObjectivePaused?.Invoke(false);
    }

    public void ConditionBroken()
    {
        OnConditionBroken?.Invoke(this);
    }

    public void Highlight()
    {
        OnObjectiveHighlight?.Invoke();
    }
}
