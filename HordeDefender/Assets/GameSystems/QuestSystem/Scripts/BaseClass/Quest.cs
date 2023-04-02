using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum QuestProgress
{
    NotStarted,
    InProgress,
    Completed,
    Done
}

[System.Serializable]
public struct QuestRewardInfo
{
    public long Ammount;
    public QuestReward questReward;
}


[System.Serializable]
public class Quest 
{
    public string QuestID;
    public string QuestName;
    public Sprite QuestIcon;
    public string QuestDescription;
    [SerializeField]private QuestProgress progress;
    public QuestProgress Progress { get => progress;private set { progress = value; SetProgress(value); } }
    public List<QuestGoal> QuestGoals=new List<QuestGoal>();
    public List<QuestRewardInfo> QuestRewards=new List<QuestRewardInfo>();

    public QuestCompletedEvent QuestCompleted= new QuestCompletedEvent();

    Action initaction = null;
    public bool IsInitComplete=true;
    private Conversation conversation;
    public Quest(string id,Action init=null)
    {
        QuestID = id;
        initaction = init;
        IsInitComplete = true;
    }

    public void Initialize()
    {
        initaction?.Invoke();
        WaitForInitComplete();
    }

    public void AddInitAction(Action init)
    {
        initaction = init;
    }



    async void WaitForInitComplete()
    {
        while (DialogueManager.Instance == null)
            await Task.Yield();

        if (conversation != null)
        {
            IsInitComplete = false;
            DialogueManager.Instance.AddConversation(conversation);
            DialogueManager.Instance.OnConversationEnd += OKToChange;
            DialogueManager.Instance.StartConversation();
        }

        while (!IsInitComplete)
            await Task.Yield();
        Progress = QuestProgress.InProgress;
        DialogueManager.Instance.OnConversationEnd -= OKToChange;

        foreach (QuestGoal goal in QuestGoals)
        {
            goal.Initialize();
            goal.GoalCompleted.AddListener(delegate { CheckGoals(); });
        }
        VerifyGoals();
    }

    public void OKToChange()
    {
        IsInitComplete = true;
    }

    private void CheckGoals()
    {
        bool Completed = QuestGoals.TrueForAll(g=>g.Completed);
        if (Completed)
        {
            Progress = QuestProgress.Completed;
            QuestCompleted.Invoke(this);
            QuestCompleted.RemoveAllListeners();
        }
    }

    public void AddGoal(QuestGoal goal)
    {
        if(!QuestGoals.Contains(goal))
        {
            QuestGoals.Add(goal);
        }
    }

    public void AddReward(QuestRewardInfo reward)
    {
        if(!QuestRewards.Contains(reward))
        {
            QuestRewards.Add(reward);
        }    
    }
    public QuestProgress GetProgress()
    {
        return (QuestProgress)Enum.Parse(typeof(QuestProgress), PlayerPrefs.GetString("QuestProgress" + QuestID, "NotStarted"));
    }
    public void SetProgress(QuestProgress progress)
    {
        PlayerPrefs.SetString("QuestProgress" + QuestID, progress.ToString());
    }
    private void VerifyGoals()
    {
        for (int i = 0; i < QuestGoals.Count; i++)
        {
            QuestGoal questGoal = QuestGoals[i];
            questGoal.VerifyGoal("QuestGoalProgress" + QuestID + i);
        }
    }

    public void AddConversation(Conversation conversation)
    {
        this.conversation = conversation;
    }

    public void ClaimReward()
    {
        if(Progress==QuestProgress.Completed)
        {
            Progress = QuestProgress.Done;
            foreach(QuestRewardInfo reward in QuestRewards)
            {
                reward.questReward?.GainReward();
            }
        }
    }

}
