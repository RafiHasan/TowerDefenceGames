using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct MoveToPositionAspect : IAspect
{
    private readonly Entity entity;

    private readonly RefRW<LocalTransform> _transform;
    private readonly RefRO<SpeedComponent> speed;
    private readonly RefRW<TargetPositionComponent> targetPosition;

    public void Move(float deltaTime)
    {
        float3 direction = math.normalize(targetPosition.ValueRW.value - _transform.ValueRO.Position);
        _transform.ValueRW.Position += direction * speed.ValueRO.value * deltaTime;
    }

    public void TestReachedtargetPosition(RefRW<RandomComponent> randomComponent)
    {
        float reachedtargetDistance = 0.5f;
        if (math.distance(_transform.ValueRO.Position, targetPosition.ValueRW.value) < reachedtargetDistance)
        {
            targetPosition.ValueRW.value = GetRandomPosition(randomComponent);
        }
    }

    private float3 GetRandomPosition(RefRW<RandomComponent> randomComponent)
    {
        return new float3(randomComponent.ValueRW.random.NextFloat(-15f, 15f),0, randomComponent.ValueRW.random.NextFloat(-25f, 25f));
    }
}
