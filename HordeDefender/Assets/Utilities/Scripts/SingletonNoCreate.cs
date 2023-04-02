using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonNoCreate<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                }

                return _instance;
            }
        }
    }


    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)FindObjectOfType(typeof(T));
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != null && _instance.gameObject != gameObject)
        {
            Destroy(gameObject);
        }
    }

}
