using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class BuffDebufferAuthoring : MonoBehaviour
{
    public GameObject BuffDebuffPrefab;
    public StatID buffDebuffType;
}

public class BuffDebufferBaker : Baker<BuffDebufferAuthoring>
{
    public override void Bake(BuffDebufferAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new BuffDebufferComponent
        {
            BuffDebuffPrefab=GetEntity(authoring.BuffDebuffPrefab, TransformUsageFlags.Dynamic),
            buffDebuffType=authoring.buffDebuffType,
        });
    }
}