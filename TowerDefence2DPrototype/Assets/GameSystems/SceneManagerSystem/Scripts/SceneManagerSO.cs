using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "ScriptAbleVariable/SceneManager")]
public class SceneManagerSO : ScriptableObject
{
    

    private IDictionary<string, Scene> AdditiveSceneList = new Dictionary<string, Scene>();

    public void LoadScene(string scenename)
    {
        if(SceneManager.Instance!=null)
        {
            SceneManager.Instance.LoadScene(scenename);
        }
        else
        {
            LoadSceneAsync(scenename);
        }        
    }

    public void LoadSceneAdditive(string scenename)
    {
        LoadSceneAsync(scenename, true);
    }

    public void LoadSceneAsync(string scenename, bool additive = false, Action oncomplete = null)
    {
        if (additive)
        {
            if (AdditiveSceneList.TryGetValue(scenename, out Scene scene))
            {
                if (!scene.IsValid())
                {
                    AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenename, LoadSceneMode.Additive);
                    op.completed += (AsyncOperation op) => { AdditiveSceneList[scenename] = UnityEngine.SceneManagement.SceneManager.GetSceneByName(scenename); oncomplete?.Invoke(); };
                }
            }
            else
            {
                AsyncOperation op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenename, LoadSceneMode.Additive);
                op.completed += (AsyncOperation op) => { AdditiveSceneList.Add(scenename, UnityEngine.SceneManagement.SceneManager.GetSceneByName(scenename)); oncomplete?.Invoke(); };
            }

        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(scenename, LoadSceneMode.Single);
        }
    }

    public void UnLoadSceneAsync(string scenename)
    {
        Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(scenename);
        if (scene != null)
        {
            AdditiveSceneList.Remove(scenename);
            if (scene.IsValid())
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
        }
        else
        {
            if (AdditiveSceneList.TryGetValue(scenename, out scene))
            {
                AdditiveSceneList.Remove(scenename);
            }
        }
    }

    public void UnLoadAllSceneAsync()
    {
        foreach (string scenename in AdditiveSceneList.Keys)
        {
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(scenename);
            if (scene != null && scene.IsValid())
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
            }
        }
        AdditiveSceneList.Clear();
    }
}
