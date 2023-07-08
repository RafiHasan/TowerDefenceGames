using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(NormalSystemGroup))]
[BurstCompile]
public partial struct DamageDealerSystem : ISystem
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
        RefRO<RandomComponent> randomComponent=default;
        bool hasrandom = false;
        foreach(RefRO<RandomComponent> rcomp in SystemAPI.Query<RefRO<RandomComponent>>())
        {
            randomComponent = rcomp;
            hasrandom = true;
        }
        if (!hasrandom)
            return;

        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        float deltaTime = SystemAPI.Time.DeltaTime;
        ComponentLookup<HealthComponent> LookupHealths = SystemAPI.GetComponentLookup<HealthComponent>();
        new DamageDealerJob
        {
            deltaTime= deltaTime,
            randomComponent= randomComponent,
            ecbp = commandBuffer.AsParallelWriter(),
            LookupHealths = LookupHealths
        }.ScheduleParallel(state.Dependency).Complete();
    }

    [BurstCompile]
    public partial struct DamageDealerJob : IJobEntity
    {
        public float deltaTime;
        [NativeDisableUnsafePtrRestriction]
        public RefRO<RandomComponent> randomComponent;
        public EntityCommandBuffer.ParallelWriter ecbp;
        [ReadOnly]
        public ComponentLookup<HealthComponent> LookupHealths;
        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey, RefRW<DamageDealerComponent> _damageDealer, RefRO<SearchAbleComponent> _searchable)
        {

            if(_searchable.ValueRO.Target!=Entity.Null)
            {
                _damageDealer.ValueRW.CoolDownCounter += deltaTime;

                if(_damageDealer.ValueRW.CoolDownCounter >= _damageDealer.ValueRO.CoolDown)
                {
                    bool hashealth = LookupHealths.TryGetComponent(_searchable.ValueRO.Target, out HealthComponent health);

                    if (hashealth)
                    {
                        health.Value -= (_damageDealer.ValueRO.Damage + randomComponent.ValueRO.random.NextFloat(-_damageDealer.ValueRO.DamageOffset / 2, _damageDealer.ValueRO.DamageOffset / 2));
                        ecbp.SetComponent<HealthComponent>(sortKey,_searchable.ValueRO.Target,new HealthComponent { Value= health.Value });

                        if (_damageDealer.ValueRO.AttackPrefab != Entity.Null)
                        {
                            Entity spawnedentity = ecbp.Instantiate(sortKey, _damageDealer.ValueRO.AttackPrefab);
                            ecbp.AddComponent(sortKey, spawnedentity, new Parent { Value = _searchable.ValueRO.Target });
                            ecbp.SetComponent<LocalTransform>(sortKey, spawnedentity, new LocalTransform { Position = new float3(0,0,0), Scale = 1, Rotation = new quaternion() });
                            DynamicBuffer<LinkedEntityGroup> group = ecbp.AddBuffer<LinkedEntityGroup>(sortKey, _searchable.ValueRO.Target);
                            group.Add(_searchable.ValueRO.Target);  // Always add self as first member of group.
                            group.Add(spawnedentity);
                        }
                            
                    }
                    _damageDealer.ValueRW.CoolDownCounter = randomComponent.ValueRO.random.NextFloat(-_damageDealer.ValueRO.CoolDownOffset / 2, _damageDealer.ValueRO.CoolDownOffset / 2);
                    
                }
            }
            

            
        }
    }
}
