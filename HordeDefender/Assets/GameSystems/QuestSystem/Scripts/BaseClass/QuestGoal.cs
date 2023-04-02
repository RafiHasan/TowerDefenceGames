using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public interface IQuestGoal
{

}

[System.Serializable]
public class QuestGoal: IQuestGoal
{
    public string TargetInstanceId;
    protected string QuestGoalDescription;
    [SerializeField]private long currentAmmount;
    public long CurrentAmmount { get =>currentAmmount; protected set { currentAmmount = value;Evaluate(); } }
    public long RequiredAmmount;

    public bool Completed { get; protected set; }
    public UnityEvent GoalCompleted = new UnityEvent();
    public UnityEvent GoalInitialize = new UnityEvent();
    private string QuestGoalId;
    public virtual string GetDescription()
    {
        return QuestGoalDescription;
    }

    public virtual void Initialize()
    {
        Completed = false;
        GoalInitialize?.Invoke();
    }

    protected virtual void UnInitialize()
    {

    }

    protected void Evaluate()
    {
        if(QuestGoalId!=null)
        PlayerPrefs.SetInt(QuestGoalId, (int)CurrentAmmount);
        if (CurrentAmmount >= RequiredAmmount)
        {
            Complete();
        }
    }

    public void VerifyGoal(string ID)
    {
        QuestGoalId = ID;
        CurrentAmmount =PlayerPrefs.GetInt(ID,0);
    }

    private void Complete()
    {
        Completed = true;
        GoalCompleted.Invoke();
        GoalCompleted.RemoveAllListeners();
        UnInitialize();
    }

    public void Skip()
    {
        //Monetization
        Complete();
    }
}
