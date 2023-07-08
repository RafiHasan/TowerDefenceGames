using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(NormalSystemGroup))]
[BurstCompile]
public partial struct HealthTrackerSystem : ISystem
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
        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        new HealthTrackerJob
        {
            ecbp = commandBuffer.AsParallelWriter(),
        }.ScheduleParallel(state.Dependency).Complete();
    }

    [BurstCompile]
    public partial struct HealthTrackerJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecbp;
        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey,Entity entity, RefRO<HealthComponent> _Health)
        {

            if (_Health.ValueRO.Value <=0)
            {
                ecbp.DestroyEntity(sortKey, entity);
            }

        }
    }
}
