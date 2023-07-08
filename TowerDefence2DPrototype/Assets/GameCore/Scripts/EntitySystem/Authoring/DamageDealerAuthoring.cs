using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DamageDealerAuthoring : MonoBehaviour
{
    public GameObject AttackPrefab;
    public float Damage;
    public float CoolDown;
}


public class DamageDealerBaker : Baker<DamageDealerAuthoring>
{
    public override void Bake(DamageDealerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new DamageDealerComponent
        {
            AttackPrefab = GetEntity(authoring.AttackPrefab, TransformUsageFlags.Dynamic),
            Damage =authoring.Damage,
            CoolDown=authoring.CoolDown,
            CoolDownCounter=0
        });
    }
}