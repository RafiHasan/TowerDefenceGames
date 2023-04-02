using System;
using System.Collections.Generic;
using UnityEngine;
using TypeReferences;

public enum RewardType
{
    Coin,
    Quest,
    Item,
}

[System.Serializable]
public class GoalData
{
    [ClassImplements(typeof(IQuestGoal))]
    public ClassTypeReference GoalType = typeof(QuestGoal);
    public string TargetId;
    public long requiredammount;
}

[System.Serializable]
public class RewardData
{
    
    [ClassImplements(typeof(IQuestReward))]
    public ClassTypeReference RewardType = typeof(QuestReward);
    public string rewardId;
    public long rewardammount;
}

[CreateAssetMenu(fileName = "QuestData", menuName = "QuestData")]
public class QuestDataList : ScriptableObject
{
    public List<QuestData> questDatas;
}

[System.Serializable]
public class QuestData
{
    public string ID;
    public string QuestName;
    public Sprite QuestIcon;
    public string QuestDescription;   
    public List<GoalData> QuestGoals = new List<GoalData>();
    public List<RewardData> QuestRewards = new List<RewardData>();
    public Conversation conversation;
    public Quest GetQuest()
    {
        
        Quest quest = new Quest(ID.ToLower());

        foreach(GoalData goalData in QuestGoals)
        {
            QuestGoal questGoal = (QuestGoal)Activator.CreateInstance(goalData.GoalType);
            questGoal.TargetInstanceId= goalData.TargetId;
            questGoal.RequiredAmmount = goalData.requiredammount;
            quest.AddGoal(questGoal);

        }

        foreach (RewardData rewardData in QuestRewards)
        {
            QuestReward questReward = (QuestReward)Activator.CreateInstance(rewardData.RewardType);
            QuestRewardInfo questRewardInfo = new QuestRewardInfo();
            
            questRewardInfo.questReward = questReward;
            questRewardInfo.Ammount = rewardData.rewardammount;
            quest.AddReward(questRewardInfo);

        }

        if(conversation.dialogues.Count>0)
        {
            quest.AddConversation(conversation);
        }

        return quest;
    }
}
