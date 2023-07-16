using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static PathFindingSystem;
using static UnityEngine.EventSystems.EventTrigger;

[UpdateInGroup(typeof(FixedTickSystemGroup))]
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

        ComponentLookup<HealthComponent> LookupHealths = SystemAPI.GetComponentLookup<HealthComponent>();

        new BuffDebuffDealerJob
        {
            LookUpValidity= SystemAPI.GetComponentLookup<LocalTransform>(),
            LookUpBuffDebuf = SystemAPI.GetBufferLookup<BuffDebuffComponent>(),
            randomComponent = randomComponent,
            fixedTickTracker = fixedTickTracker,
            ecbp = commandBuffer.AsParallelWriter(),
        }.ScheduleParallel(state.Dependency).Complete();

        new BuffDebuffEffectJob
        {
            ecbp = commandBuffer.AsParallelWriter()
        }.ScheduleParallel(state.Dependency).Complete();
        
    }

    [BurstCompile]
    public partial struct BuffDebuffDealerJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<LocalTransform> LookUpValidity;
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

                if (_buffDebuffer.ValueRW.CoolDownCounter >= _buffDebuffer.ValueRO.CoolDown)
                {

                    if(LookUpBuffDebuf.TryGetBuffer(_searchable.ValueRO.Target,out DynamicBuffer<BuffDebuffComponent> buffDebuffComponent))
                    {
                        ecbp.AppendToBuffer<BuffDebuffComponent>(sortKey, _searchable.ValueRO.Target, new BuffDebuffComponent { BuffDebuffPrefab = _buffDebuffer.ValueRO.BuffDebuffPrefab, buffDebuffType = _buffDebuffer.ValueRO.buffDebuffType, EffectPower = _buffDebuffer.ValueRO.EffectPower, EffectTime = _buffDebuffer.ValueRO.EffectTime + randomComponent.ValueRO.NextFloat(fixedTickTracker.ValueRO.StepCount,-_buffDebuffer.ValueRO.EffectTimeOffset / 2, _buffDebuffer.ValueRO.EffectTimeOffset / 2) });
                    }
                    else
                    {
                        DynamicBuffer<BuffDebuffComponent> buffer = ecbp.AddBuffer<BuffDebuffComponent>(sortKey, _searchable.ValueRO.Target);
                        buffer.Add(new BuffDebuffComponent { BuffDebuffPrefab= _buffDebuffer.ValueRO.BuffDebuffPrefab, buffDebuffType = _buffDebuffer.ValueRO.buffDebuffType, EffectPower = _buffDebuffer.ValueRO.EffectPower, EffectTime = _buffDebuffer.ValueRO.EffectTime + randomComponent.ValueRO.NextFloat(fixedTickTracker.ValueRO.StepCount, -_buffDebuffer.ValueRO.EffectTimeOffset / 2, _buffDebuffer.ValueRO.EffectTimeOffset / 2) });
                    }

                    
                    _buffDebuffer.ValueRW.CoolDownCounter= randomComponent.ValueRO.NextFloat(fixedTickTracker.ValueRO.StepCount ,- _buffDebuffer.ValueRO.CoolDownOffset / 2, _buffDebuffer.ValueRO.CoolDownOffset / 2);

                }
            }
        }
    }


    [BurstCompile]
    public partial struct BuffDebuffEffectJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecbp;
        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey, Entity entity, DynamicBuffer<BuffDebuffComponent> _buffer,RefRW<HealthComponent> _health, RefRW<StatsComponent> _stats)
        {


            NativeHashMap<Entity, int> attacprefabs = new NativeHashMap<Entity, int>(1024, Allocator.Temp);
            float healthdiff = 0;
            float speedmult = 1;

            for(int i=0;i< _buffer.Length;i++)
            {
                BuffDebuffComponent buffDebuffComponent = _buffer[i];

                if (buffDebuffComponent.EffectTime <= 0)
                {
                    _buffer.RemoveAt(i);
                   
                    if(buffDebuffComponent.buffDebuffType== StatID.HEALTH)
                    {
                        healthdiff += buffDebuffComponent.EffectPower * (buffDebuffComponent .DeltaTime+ buffDebuffComponent.EffectTime);
                    }
                    else if(buffDebuffComponent.buffDebuffType == StatID.SPEED)
                    {

                    }

                }
                else
                {
                    if (buffDebuffComponent.buffDebuffType == StatID.HEALTH)
                    {
                        healthdiff += buffDebuffComponent.EffectPower * buffDebuffComponent.DeltaTime;
                    }
                    else if (buffDebuffComponent.buffDebuffType == StatID.SPEED)
                    {
                        speedmult *= buffDebuffComponent.EffectPower;
                    }

                    if (_buffer[i].BuffDebuffPrefab != Entity.Null)
                    {
                        if (attacprefabs.TryGetValue(_buffer[i].BuffDebuffPrefab, out int item))
                        {
                            attacprefabs[_buffer[i].BuffDebuffPrefab] = item + 1;
                        }
                        else
                        {
                            attacprefabs.Add(_buffer[i].BuffDebuffPrefab, 1);
                        }
                    }

                }

                

            }

            _stats.ValueRW.SetBuffValue(StatID.SPEED, speedmult);
            _health.ValueRW.Value += healthdiff;

            foreach (Entity key in attacprefabs.GetKeyArray(Allocator.Temp))
            {
                Entity spawnedentity = ecbp.Instantiate(sortKey, key);
                ecbp.AddComponent(sortKey, spawnedentity, new Parent { Value = entity });
                ecbp.SetComponent(sortKey, spawnedentity, new LocalTransform { Position = new float3(0, 0, 0), Scale = 1, Rotation = new quaternion() });
            }
        }
    }
}
