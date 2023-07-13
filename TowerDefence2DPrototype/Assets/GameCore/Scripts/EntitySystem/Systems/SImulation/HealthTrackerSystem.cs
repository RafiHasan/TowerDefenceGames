using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(NormalSystemGroup),OrderFirst =true)]
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
        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
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
        public void Execute([EntityIndexInQuery] int sortKey,Entity entity, RefRO<HealthComponent> _Health,RefRW<PLData> _plData)
        {

            if (_Health.ValueRO.Value <=0)
            {
                ecbp.RemoveComponent<HealthComponent>(sortKey,entity);
                ecbp.RemoveComponent<SearchAbleComponent>(sortKey, entity);
                ecbp.RemoveComponent<SearchingTag>(sortKey, entity);
                ecbp.AddComponent(sortKey,entity,new DelayDestroyComponent { Delay=2.0f });
                _plData.ValueRW.IsDead = true;
                _plData.ValueRW.cleanuptime = 1.5f;
            }

        }
    }
}
