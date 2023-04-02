using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SafeAreaFilter : MonoBehaviour
{
    
    private void Awake()
    {
        var rect = Screen.safeArea;
        var anchorMin = rect.position;
        var anchorMax = rect.position + rect.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;

        Debug.Log($"SafeAreaFilter: {anchorMin} {anchorMax}");
    }

    
    

}
