using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGenerator : MonoBehaviour
{
    public QuestDataList questData;
    public List<Quest> questList;
    // Start is called before the first frame update
    void Start()
    {
        GenerateQuest();
    }

    public void GenerateQuest()
    {
        for(int i=0;i< questData.questDatas.Count;i++)
        {
            QuestManager.Instance.AddQuest(questData.questDatas[i].GetQuest());
        }

    }
}
