using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSceneLoad : MonoBehaviour
{
    public SceneManagerSO SceneManagerSO;
    public string SceneName;
    public bool IsOverlay;
    void Start()
    {
        if(!IsOverlay)
        {
            SceneManagerSO.LoadScene(SceneName);
        }
        else
        {
            SceneManagerSO.LoadSceneAdditive(SceneName);
        }
    }
}
