using UnityEngine;

[System.Serializable]
public class QuestObjective : MonoBehaviour
{
    [field: SerializeField] public string description { get; private set; }
    public bool isComplete { get; private set; }
    public bool active { get; private set; }
    public event System.Action<QuestObjective> OnObjectiveComplete = delegate { };
    public event System.Action OnObjectiveHighlight = delegate { };
    public event System.Action OnObjectiveActivated = delegate { };

    public void Activate()
    {
        active = true;
        isComplete = false;
        OnObjectiveActivated?.Invoke();
    }
    public void DeActivate()
    {
        active = false;
    }

    public void OnComplete()
    {
        isComplete = true;
        active = false;
        OnObjectiveComplete?.Invoke(this);
    }

    public void Highlight()
    {
        OnObjectiveHighlight?.Invoke();
    }
}
