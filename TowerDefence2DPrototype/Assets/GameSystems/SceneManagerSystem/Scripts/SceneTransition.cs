using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    public Action OnTransitionComplete;
    public float interval;
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void StartTransition()
    {
        gameObject.SetActive(true);
        StartCoroutine(Transition());
    }

    public virtual IEnumerator Transition()
    {
        yield return new WaitForSeconds(interval);
        OnTransitionComplete?.Invoke();
        yield return new WaitForSeconds(interval);
        gameObject.SetActive(false);
    }

}
