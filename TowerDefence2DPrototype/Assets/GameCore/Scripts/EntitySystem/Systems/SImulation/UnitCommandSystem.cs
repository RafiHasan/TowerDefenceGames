using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct UnitCommandSystem : ISystem
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

        RefRW<GridComponent> gridComponent = SystemAPI.GetSingletonRW<GridComponent>();


        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        new UnitSetDestinationCommandJob
        {
            gridPos= gridComponent.ValueRO.SelectedCell,
            ecbp = commandBuffer.AsParallelWriter()
        }.ScheduleParallel(state.Dependency).Complete();

    }

    [BurstCompile]
    public partial struct UnitSetDestinationCommandJob : IJobEntity
    {
        public int2 gridPos;
        public EntityCommandBuffer.ParallelWriter ecbp;
        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey,Entity entity, RefRW<MovementComponent> _movement, RefRO<SelectedTag> _selected)
        {

            if (gridPos.Equals(new int2(int.MaxValue, int.MaxValue)))
                return;
            _movement.ValueRW.PathCalculated = false;
            _movement.ValueRW.Goal=gridPos;
            ecbp.RemoveComponent<SelectedTag>(sortKey,entity);
        }
    }
}
