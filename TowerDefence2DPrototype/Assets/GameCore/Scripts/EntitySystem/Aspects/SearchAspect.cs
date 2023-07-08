using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct SearchAspect : IAspect
{
    public readonly Entity entity;
    public readonly RefRO<SearchingTag> SearchTag;
    public readonly RefRO<LocalTransform> LocalTransform;

    public float Distance(RefRO<LocalTransform> localTransform)
    {
        return math.distance(LocalTransform.ValueRO.Position,localTransform.ValueRO.Position);
    }
}
