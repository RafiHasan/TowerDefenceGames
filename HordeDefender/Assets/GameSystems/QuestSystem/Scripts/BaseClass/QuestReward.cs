using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface IQuestReward
{

}

[System.Serializable]
public class QuestReward: IQuestReward
{
    public string Name;
    public Sprite Icon;
    public string Description;
    public event Action RewardAction;
    public void GainReward()
    {
        WaitToGain();
    }

    async void WaitToGain()
    {
        await Task.Delay(250);
        RewardAction.Invoke();
    }
}
