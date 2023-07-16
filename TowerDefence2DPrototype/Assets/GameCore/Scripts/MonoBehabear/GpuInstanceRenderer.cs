using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GpuInstanceRenderer : MonoBehaviour
{
    public static GpuInstanceRenderer Instance;
    public Material material;
    public Mesh mesh;

    private void Awake()
    {
        Instance = this;
    }
    public Dictionary<Sprite, List<Matrix4x4>> keyValuePairs = new Dictionary<Sprite, List<Matrix4x4>>();

    public void UdateVisualGPU(Sprite sprite, Matrix4x4 instData)
    {

        if(keyValuePairs.ContainsKey(sprite))
        {
            keyValuePairs[sprite].Add(instData);
        }
        else
        {
            keyValuePairs.Add(sprite,new List<Matrix4x4> { instData });
        }
    }


    public void LateUpdate()
    {

        foreach(Sprite sprite in keyValuePairs.Keys)
        {
            
            Material newMaterial = new Material(material);
            newMaterial.SetTexture("_MainTex", sprite.texture);
            Graphics.DrawMeshInstanced(mesh, 0, newMaterial, keyValuePairs[sprite]);
        }

        keyValuePairs.Clear();
    }
}
