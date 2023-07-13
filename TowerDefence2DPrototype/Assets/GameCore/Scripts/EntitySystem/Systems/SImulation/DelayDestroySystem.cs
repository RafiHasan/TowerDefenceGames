using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(NormalSystemGroup))]
[UpdateAfter(typeof(MovementSystem))]
[BurstCompile]
public partial struct DelayDestroySystem : ISystem
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
        float deltaTime = SystemAPI.Time.DeltaTime;
        new DelayDestroyJob
        {
            deltaTime = deltaTime,
            ecbp = commandBuffer.AsParallelWriter(),
        }.ScheduleParallel(state.Dependency).Complete();
    }

    [BurstCompile]
    public partial struct DelayDestroyJob : IJobEntity
    {
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecbp;
        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey,Entity entity, RefRW<DelayDestroyComponent> _delayDestroy)
        {
            _delayDestroy.ValueRW.DelayCounter += deltaTime;
            if (_delayDestroy.ValueRO.DelayCounter>= _delayDestroy.ValueRO.Delay)
            {
                ecbp.DestroyEntity(sortKey, entity);
            }
            
        }
    }
}
