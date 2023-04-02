using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [SerializeField]private List<Quest> availableQuestList = new List<Quest>();

    private IDictionary<string, Quest> QuestDict = new Dictionary<string, Quest>();

    public GameObject heighlighter3d;
    public GameObject heighlighter2d;

    public void Awake()
    {
        if(Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddQuest(Quest quest)
    {
        QuestDict.Add(quest.QuestID.ToLower(), quest);
        if (!availableQuestList.Contains(quest))
        {
            availableQuestList.Add(quest);
            if(quest.GetProgress()==QuestProgress.InProgress)
            {
                StartQuest(quest);
            }
            else
            {
                quest.SetProgress(quest.GetProgress());
            }
        }
    }

    public List<Quest> GetAllActiveQuests()
    {
        List<Quest> activeQuests = new List<Quest>();
        foreach(Quest quest in availableQuestList)
        {
            if (quest.Progress == QuestProgress.InProgress)
                activeQuests.Add(quest);
        }

        return activeQuests;
    }

    public List<Quest> GetAllCompletedQuests()
    {
        List<Quest> activeQuests = new List<Quest>();
        foreach (Quest quest in availableQuestList)
        {
            if (quest.Progress == QuestProgress.Completed)
                activeQuests.Add(quest);
        }

        return activeQuests;
    }

    public void StartQuest(string ID)
    {
        if(QuestDict.TryGetValue(ID.ToLower(),out Quest quest))
        {
            StartQuest(quest);
        }
    }

    public void StartQuest(Quest quest)
    {
        if (quest.Progress==QuestProgress.NotStarted)
        {    
            quest.Initialize();
        }
    }

    public GameObject GetHeighlighter(Transform transform)
    {
        GameObject highlighter=null;
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            highlighter=Instantiate(heighlighter2d, transform.GetComponentInParent<Canvas>().transform);
            highlighter.GetComponent<RectTransform>().position = transform.position;
        }
        else
        {
            highlighter = Instantiate(heighlighter3d, transform.position, Quaternion.identity);
        }


        return highlighter;
    }
}
