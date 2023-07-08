using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BuffDebuffComponent : IComponentData
{
    public StatID buffDebuffType;
    public float EffectTime;
}
