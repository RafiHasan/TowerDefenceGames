using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using static GridSystem;
using static PathFindingSystem;

[UpdateInGroup(typeof(NormalSystemGroup))]
[BurstCompile]
public partial struct MovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        
        if (!SystemAPI.HasSingleton<GridComponent>())
            return;
        GridComponent gridComponent = SystemAPI.GetSingleton<GridComponent>();
        float deltaTime = SystemAPI.Time.DeltaTime;
        new MovementJob
        {
            deltaTime = deltaTime,
            gridComponent = gridComponent,
        }.ScheduleParallel(state.Dependency).Complete();
    }

    [BurstCompile]
    public partial struct MovementJob : IJobEntity
    {
        public float deltaTime;
        public GridComponent gridComponent;
        [BurstCompile]
        public void Execute(RefRW<MovementComponent> _movement, RefRW<LocalTransform> _transform,DynamicBuffer<PathNodeBufferElement> buffer)
        {
            
            float3 target = _movement.ValueRO.NextPosition;
            float3 current = _transform.ValueRO.Position;

            float3 currentnormal = math.normalize(target - current);
            current += currentnormal * _movement.ValueRO.Speed * deltaTime;
            float3 nextnormal= math.normalize(target - current);
            
            if ( math.dot(currentnormal, nextnormal)>0.9f)
            {

                _transform.ValueRW.Position = current;
            }
            else
            {
                if (buffer.Length == 0)
                    return;

                var index = gridComponent.GetCellIndex(_transform.ValueRO.Position) + gridComponent.GridSize / 2;
                bool foundmatch = false;
                if (buffer[buffer.Length - 1].x == index.x && buffer[buffer.Length - 1].y == index.y)
                {
                    foundmatch = true;
                    buffer.RemoveAt(buffer.Length - 1);
                }

                if (!foundmatch)
                {
                    _movement.ValueRW.NextPosition = gridComponent.GetCellPosition(new int2(buffer[buffer.Length - 1].x, buffer[buffer.Length - 1].y) - gridComponent.GridSize / 2);
                }

            }
        }
    }
}
