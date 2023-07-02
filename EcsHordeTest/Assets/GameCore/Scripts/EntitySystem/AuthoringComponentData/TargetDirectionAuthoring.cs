using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Entities.EntitiesJournaling;

public struct TargetDirectionComponent : IComponentData
{
    public float3 value;
}

public class TargetDirectionAuthoring : MonoBehaviour
{
    public float3 value;
}

public class TargetDirectionBaker : Baker<TargetDirectionAuthoring>
{
    public override void Bake(TargetDirectionAuthoring authoring)
    {
        TransformUsageFlags transformUsageFlags = new TransformUsageFlags();
        Entity entity = this.GetEntity(transformUsageFlags);
        AddComponent(entity,new TargetDirectionComponent
        {
            value = authoring.value
        });
    }
}