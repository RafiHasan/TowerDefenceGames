using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct CleanUpTag : IComponentData
{
    public int Index;
}