using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance;
    [SerializeField] private SceneManagerSO sceneManager_SO;
    private void Awake()
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


    public void LoadScene(string scenename)
    {
        LoadSceneAsync(scenename);
    }

    public void LoadSceneAdditive(string scenename)
    {
        sceneManager_SO.LoadSceneAdditive(scenename);
    }

    public void LoadSceneAsync(string scenename, bool additive = false, Action oncomplete = null)
    {
        sceneManager_SO.LoadSceneAsync(scenename, additive,oncomplete);
    }

    public void UnLoadSceneAsync(string scenename)
    {
        sceneManager_SO.UnLoadSceneAsync(scenename);
        
    }

    public void UnLoadAllSceneAsync()
    {
        sceneManager_SO.UnLoadAllSceneAsync();
    }
}
