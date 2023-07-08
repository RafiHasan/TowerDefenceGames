using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BuffDebufferComponent : IComponentData
{
    public Entity BuffDebuffPrefab;
    public StatID buffDebuffType;
    public float CoolDown;
    public float CoolDownCounter;
    public float CoolDownOffset;
    public float EffectTime;
}


