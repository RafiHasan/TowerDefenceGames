using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct DamageComponent : IBufferElementData
{
    public float Damage;
    public Entity AttackPrefab;
}
