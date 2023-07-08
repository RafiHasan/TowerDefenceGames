using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct DelayDestroyComponent : IComponentData
{
    public float Delay;
    public float DelayCounter;
}
