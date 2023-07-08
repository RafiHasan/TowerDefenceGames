using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct InputComponent : IComponentData
{
    public bool GameStart;
    public bool Selected;
    public float3 WorldPosition;
    public bool DeSelected;
    public bool Spawn;
    public bool Reset;
    public bool Save;
    public bool Load;
    public int ItemIndex;

    public void Clear()
    {
        
        Selected = false;
        DeSelected = false;
        Spawn = false;
        ItemIndex = 0;
    }
}
