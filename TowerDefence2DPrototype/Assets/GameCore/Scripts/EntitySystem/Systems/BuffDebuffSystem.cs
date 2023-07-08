using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(NormalSystemGroup))]
[BurstCompile]
public partial struct BuffDebuffSystem : ISystem
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

        RefRO<RandomComponent> randomComponent = default;
        bool hasrandom = false;
        foreach (RefRO<RandomComponent> rcomp in SystemAPI.Query<RefRO<RandomComponent>>())
        {
            randomComponent = rcomp;
            hasrandom = true;
        }
        if (!hasrandom)
            return;

        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        float deltaTime = SystemAPI.Time.DeltaTime;
        ComponentLookup<HealthComponent> LookupHealths = SystemAPI.GetComponentLookup<HealthComponent>();

        new BuffDebuffDealerJob
        {
            deltaTime = deltaTime,
            randomComponent = randomComponent,
            ecbp = commandBuffer.AsParallelWriter(),
        }.ScheduleParallel(state.Dependency).Complete();

        new BuffDebuffEffectJob
        {
            deltaTime = deltaTime,
            ecbp = commandBuffer.AsParallelWriter(),
        }.ScheduleParallel(state.Dependency).Complete();
        
    }

    [BurstCompile]
    public partial struct BuffDebuffDealerJob : IJobEntity
    {
        public float deltaTime;
        [NativeDisableUnsafePtrRestriction]
        public RefRO<RandomComponent> randomComponent;
        public EntityCommandBuffer.ParallelWriter ecbp;
        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey, RefRW<BuffDebufferComponent> _buffDebuffer, RefRO<SearchAbleComponent> _searchable)
        {

            if (_searchable.ValueRO.Target != Entity.Null)
            {
                _buffDebuffer.ValueRW.CoolDownCounter += deltaTime;

                if (_buffDebuffer.ValueRW.CoolDownCounter >= _buffDebuffer.ValueRO.CoolDown)
                {
                    ecbp.RemoveComponent<BuffDebuffComponent>(sortKey, _searchable.ValueRO.Target);
                    ecbp.AddComponent(sortKey, _searchable.ValueRO.Target, new BuffDebuffComponent { buffDebuffType= _buffDebuffer.ValueRO.buffDebuffType, EffectTime= _buffDebuffer.ValueRO.EffectTime });
                    _buffDebuffer.ValueRW.CoolDownCounter= randomComponent.ValueRO.random.NextFloat(-_buffDebuffer.ValueRO.CoolDownOffset / 2, _buffDebuffer.ValueRO.CoolDownOffset / 2);

                    if (_buffDebuffer.ValueRO.BuffDebuffPrefab != Entity.Null)
                    {
                        Entity spawnedentity = ecbp.Instantiate(sortKey, _buffDebuffer.ValueRO.BuffDebuffPrefab);
                        ecbp.AddComponent(sortKey, spawnedentity, new Parent { Value = _searchable.ValueRO.Target });
                        ecbp.SetComponent<LocalTransform>(sortKey, spawnedentity, new LocalTransform { Position = new float3(0, 0, 0), Scale = 1, Rotation = new quaternion() });
                        DynamicBuffer<LinkedEntityGroup> group = ecbp.AddBuffer<LinkedEntityGroup>(sortKey, _searchable.ValueRO.Target);
                        group.Add(_searchable.ValueRO.Target);  // Always add self as first member of group.
                        group.Add(spawnedentity);
                    }


                }
            }
        }
    }


    [BurstCompile]
    public partial struct BuffDebuffEffectJob : IJobEntity
    {
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecbp;
        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey,Entity entity, RefRW<BuffDebuffComponent> _buffDebuff, RefRW<StatsComponent> _stats)
        {
            _buffDebuff.ValueRW.EffectTime -= deltaTime;

            if (_buffDebuff.ValueRW.EffectTime<0)
            {
                ecbp.RemoveComponent<BuffDebuffComponent>(sortKey, entity);
                _stats.ValueRW.SetBuffValue(_buffDebuff.ValueRO.buffDebuffType,0);
            }
            else
            {
                _stats.ValueRW.SetBuffValue(_buffDebuff.ValueRO.buffDebuffType, -0.5f);
            }

        }
    }
}
