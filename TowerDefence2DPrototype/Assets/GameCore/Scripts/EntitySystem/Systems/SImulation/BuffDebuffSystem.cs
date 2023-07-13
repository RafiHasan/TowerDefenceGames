using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static PathFindingSystem;

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

        if (!SystemAPI.HasSingleton<RandomComponent>())
            return;

        RefRW<RandomComponent> randomComponent = SystemAPI.GetSingletonRW<RandomComponent>();

        if (!SystemAPI.HasSingleton<FIxedTickTrackerComponent>())
            return;

        RefRW<FIxedTickTrackerComponent> fixedTickTracker = SystemAPI.GetSingletonRW<FIxedTickTrackerComponent>();

        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        float deltaTime = SystemAPI.Time.DeltaTime;
        ComponentLookup<HealthComponent> LookupHealths = SystemAPI.GetComponentLookup<HealthComponent>();

        new BuffDebuffDealerJob
        {
            LookUpValidity= SystemAPI.GetComponentLookup<LocalTransform>(),
            deltaTime = deltaTime,
            LookUpBuffDebuf = SystemAPI.GetBufferLookup<BuffDebuffComponent>(),
            randomComponent = randomComponent,
            fixedTickTracker = fixedTickTracker,
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
        [ReadOnly]
        public ComponentLookup<LocalTransform> LookUpValidity;
        public float deltaTime;
        [ReadOnly]
        public BufferLookup<BuffDebuffComponent> LookUpBuffDebuf;
        [NativeDisableUnsafePtrRestriction]
        public RefRW<RandomComponent> randomComponent;
        [NativeDisableUnsafePtrRestriction]
        public RefRW<FIxedTickTrackerComponent> fixedTickTracker;
        public EntityCommandBuffer.ParallelWriter ecbp;
        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey, RefRW<BuffDebufferComponent> _buffDebuffer, RefRO<SearchAbleComponent> _searchable)
        {

            bool IsValidEntity = LookUpValidity.HasComponent(_searchable.ValueRO.Target);


            if (_searchable.ValueRO.Target != Entity.Null && IsValidEntity)
            {
                _buffDebuffer.ValueRW.CoolDownCounter += deltaTime;

                if (_buffDebuffer.ValueRW.CoolDownCounter >= _buffDebuffer.ValueRO.CoolDown)
                {
                    //ecbp.RemoveComponent<BuffDebuffComponent>(sortKey, _searchable.ValueRO.Target);

                    if(LookUpBuffDebuf.TryGetBuffer(_searchable.ValueRO.Target,out DynamicBuffer<BuffDebuffComponent> buffDebuffComponent))
                    {
                        ecbp.AppendToBuffer<BuffDebuffComponent>(sortKey, _searchable.ValueRO.Target, new BuffDebuffComponent { buffDebuffType = _buffDebuffer.ValueRO.buffDebuffType, EffectPower = _buffDebuffer.ValueRO.EffectPower, EffectTime = _buffDebuffer.ValueRO.EffectTime + randomComponent.ValueRO.NextFloat(fixedTickTracker.ValueRO.StepCount,-_buffDebuffer.ValueRO.EffectTimeOffset / 2, _buffDebuffer.ValueRO.EffectTimeOffset / 2) });
                    }
                    else
                    {
                        DynamicBuffer<BuffDebuffComponent> buffer = ecbp.AddBuffer<BuffDebuffComponent>(sortKey, _searchable.ValueRO.Target);
                        buffer.Add(new BuffDebuffComponent { buffDebuffType = _buffDebuffer.ValueRO.buffDebuffType, EffectPower = _buffDebuffer.ValueRO.EffectPower, EffectTime = _buffDebuffer.ValueRO.EffectTime + randomComponent.ValueRO.NextFloat(fixedTickTracker.ValueRO.StepCount, -_buffDebuffer.ValueRO.EffectTimeOffset / 2, _buffDebuffer.ValueRO.EffectTimeOffset / 2) });
                    }

                    
                    _buffDebuffer.ValueRW.CoolDownCounter= randomComponent.ValueRO.NextFloat(fixedTickTracker.ValueRO.StepCount ,- _buffDebuffer.ValueRO.CoolDownOffset / 2, _buffDebuffer.ValueRO.CoolDownOffset / 2);

                    if (_buffDebuffer.ValueRO.BuffDebuffPrefab != Entity.Null)
                    {
                        Entity spawnedentity = ecbp.Instantiate(sortKey, _buffDebuffer.ValueRO.BuffDebuffPrefab);
                        ecbp.AddComponent(sortKey, spawnedentity, new Parent { Value = _searchable.ValueRO.Target });
                        ecbp.SetComponent(sortKey, spawnedentity, new LocalTransform { Position = new float3(0, 0, 0), Scale = 1, Rotation = new quaternion() });
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
        public void Execute([EntityIndexInQuery] int sortKey,Entity entity,DynamicBuffer<BuffDebuffComponent> _buffer,RefRW<HealthComponent> _health, RefRW<StatsComponent> _stats)
        {

            float healthdiff = 0;
            float speedmult = 1;

            for(int i=0;i< _buffer.Length;i++)
            {
                BuffDebuffComponent buffDebuffComponent = _buffer[i];
                buffDebuffComponent.EffectTime -= deltaTime;
                _buffer[i] = buffDebuffComponent;

                if (buffDebuffComponent.EffectTime <= 0)
                {
                    _buffer.RemoveAt(i);
                   
                    if(buffDebuffComponent.buffDebuffType== StatID.HEALTH)
                    {
                        healthdiff += buffDebuffComponent.EffectPower * (deltaTime+ buffDebuffComponent.EffectTime);
                    }
                    else if(buffDebuffComponent.buffDebuffType == StatID.SPEED)
                    {

                    }

                }
                else
                {
                    if (buffDebuffComponent.buffDebuffType == StatID.HEALTH)
                    {
                        healthdiff += buffDebuffComponent.EffectPower * deltaTime;
                    }
                    else if (buffDebuffComponent.buffDebuffType == StatID.SPEED)
                    {
                        speedmult *= buffDebuffComponent.EffectPower;
                    }
                }
            }

            _stats.ValueRW.SetBuffValue(StatID.SPEED, speedmult);
            _health.ValueRW.Value += healthdiff;
        }
    }
}
