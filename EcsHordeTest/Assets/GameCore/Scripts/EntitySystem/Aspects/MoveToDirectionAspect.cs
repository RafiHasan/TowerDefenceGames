using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct MoveToDirectionAspect : IAspect
{
    private readonly Entity entity;

    private readonly RefRW<LocalTransform> _transform;
    private readonly RefRO<SpeedComponent> speed;
    private readonly RefRW<TargetDirectionComponent> targetDirection;

    public void Move(float deltaTime)
    {
        if (targetDirection.ValueRW.value.x == 0 && targetDirection.ValueRW.value.y == 0 && targetDirection.ValueRW.value.z == 0)
            return;
        float3 direction = math.normalize(targetDirection.ValueRW.value);
        _transform.ValueRW.Position += direction * speed.ValueRO.value * deltaTime;
    }

}
