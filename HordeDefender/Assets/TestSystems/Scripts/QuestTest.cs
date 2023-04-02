using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QuestTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DelayQuest();
        
    }

    public async void DelayQuest()
    {
        await Task.Yield();
        QuestManager.Instance.StartQuest("GameUnit0Click");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
