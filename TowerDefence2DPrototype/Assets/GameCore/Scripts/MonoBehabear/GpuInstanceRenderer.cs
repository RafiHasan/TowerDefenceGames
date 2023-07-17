using System.Collections.Generic;
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

    public Dictionary<Sprite, List<Matrix4x4>> matrixDict = new Dictionary<Sprite, List<Matrix4x4>>();

    public Dictionary<Sprite, Color> colorDict = new Dictionary<Sprite, Color>();

    public void UdateVisualGPU(Sprite sprite, Matrix4x4 instData,Color color)
    {

        if(matrixDict.ContainsKey(sprite))
        {
            matrixDict[sprite].Add(instData);
        }
        else
        {
            matrixDict.Add(sprite,new List<Matrix4x4> { instData });
        }


        if (colorDict.ContainsKey(sprite))
        {
            colorDict[sprite]=color;
        }
        else
        {
            colorDict.Add(sprite, color);
        }

    }


    public void LateUpdate()
    {

        foreach(Sprite sprite in matrixDict.Keys)
        {
            
            Material newMaterial = new Material(material);
            newMaterial.SetTexture("_MainTex", sprite.texture);

            if(colorDict.ContainsKey(sprite))
            {
                newMaterial.SetColor("_Color", colorDict[sprite]);
            }

            Graphics.DrawMeshInstanced(mesh, 0, newMaterial, matrixDict[sprite]);
        }

        matrixDict.Clear();
        colorDict.Clear();
    }
}
