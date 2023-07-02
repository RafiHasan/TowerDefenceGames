using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Entities.EntitiesJournaling;

public struct TargetPositionComponent : IComponentData
{
    public float3 value;
}

public class TargetPositionAuthoring : MonoBehaviour
{
    public float3 value;
}

public class TargetPositionBaker : Baker<TargetPositionAuthoring>
{
    public override void Bake(TargetPositionAuthoring authoring)
    {
        TransformUsageFlags transformUsageFlags = new TransformUsageFlags();
        Entity entity = this.GetEntity(transformUsageFlags);
        AddComponent(entity,new TargetPositionComponent
        {
            value = authoring.value
        });
    }
}