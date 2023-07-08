using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct DamageDealerComponent : IComponentData
{
    public Entity AttackPrefab;
    public float Damage;
    public float DamageOffset;
    public float CoolDown;
    public float CoolDownCounter;
    public float CoolDownOffset;
}
