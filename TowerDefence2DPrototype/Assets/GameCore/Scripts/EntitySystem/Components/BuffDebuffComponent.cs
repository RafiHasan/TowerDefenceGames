using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BuffDebuffComponent : IBufferElementData
{
    public StatID buffDebuffType;
    public float EffectTime;
    public float EffectPower;
}
