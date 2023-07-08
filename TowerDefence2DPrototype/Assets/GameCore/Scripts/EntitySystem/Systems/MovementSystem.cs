using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static GridSystem;

[UpdateInGroup(typeof(NormalSystemGroup))]
[BurstCompile]
public partial struct MovementSystem : ISystem
{
    double time;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        time = SystemAPI.Time.ElapsedTime;
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = (float)(SystemAPI.Time.ElapsedTime- time);
        time = SystemAPI.Time.ElapsedTime;
        new MovementJob
        {
            deltaTime = deltaTime
        }.ScheduleParallel();
    }

    [BurstCompile]
    public partial struct MovementJob : IJobEntity
    {
        public float deltaTime;
        [BurstCompile]
        public void Execute(RefRO<MovementComponent> _movement, RefRW<LocalTransform> _transform)
        {
            float3 target = _movement.ValueRO.NextPosition;
            float3 current = _transform.ValueRO.Position;

            float3 currentnormal = math.normalize(target - current);
            current += math.normalize(_movement.ValueRO.NextPosition - _transform.ValueRO.Position) * _movement.ValueRO.Speed * deltaTime;
            float3 nextnormal= math.normalize(target - current);

            if( math.dot(currentnormal, nextnormal)>0.9f)
            {
                _transform.ValueRW.Position = current;
            }            
        }
    }
}
