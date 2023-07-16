using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedTickSystemGroup))]
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
        if (!SystemAPI.HasSingleton<RandomComponent>())
            return;

        RefRW<RandomComponent> randomComponent = SystemAPI.GetSingletonRW<RandomComponent>();

        if (!SystemAPI.HasSingleton<FIxedTickTrackerComponent>())
            return;

        RefRW<FIxedTickTrackerComponent> fixedTickTracker = SystemAPI.GetSingletonRW<FIxedTickTrackerComponent>();

        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        ComponentLookup<HealthComponent> LookupHealths = SystemAPI.GetComponentLookup<HealthComponent>();
        new DamageDealerJob
        {
            LookUpValidity = SystemAPI.GetComponentLookup<LocalTransform>(),
            randomComponent= randomComponent,
            fixedTickTracker= fixedTickTracker,
            ecbp = commandBuffer.AsParallelWriter(),
            LookupHealths = LookupHealths,
            LookUpDamage=SystemAPI.GetBufferLookup<DamageComponent>()
        }.ScheduleParallel(state.Dependency).Complete();

        new DamagingJob
        {
            ecbp = commandBuffer.AsParallelWriter()
        }.ScheduleParallel(state.Dependency).Complete();
    }

    [BurstCompile]
    public partial struct DamageDealerJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<LocalTransform> LookUpValidity;
        [NativeDisableUnsafePtrRestriction]
        public RefRW<RandomComponent> randomComponent;
        [NativeDisableUnsafePtrRestriction]
        public RefRW<FIxedTickTrackerComponent> fixedTickTracker;
        public EntityCommandBuffer.ParallelWriter ecbp;
        [ReadOnly]
        public ComponentLookup<HealthComponent> LookupHealths;
        [ReadOnly]
        public BufferLookup<DamageComponent> LookUpDamage;
        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey, RefRW<DamageDealerComponent> _damageDealer, RefRO<SearchAbleComponent> _searchable)
        {

            bool IsValidEntity = LookUpValidity.HasComponent(_searchable.ValueRO.Target);


            if (_searchable.ValueRO.Target != Entity.Null && IsValidEntity)
            {

                if(_damageDealer.ValueRW.CoolDownCounter >= _damageDealer.ValueRO.CoolDown)
                {
                    bool hashealth = LookupHealths.TryGetComponent(_searchable.ValueRO.Target, out HealthComponent health);

                    if (hashealth)
                    {

                        if(LookUpDamage.TryGetBuffer(_searchable.ValueRO.Target, out DynamicBuffer<DamageComponent> _buffer))
                        {
                            ecbp.AppendToBuffer(sortKey, _searchable.ValueRO.Target, new DamageComponent { AttackPrefab = _damageDealer.ValueRO.AttackPrefab, Damage = (_damageDealer.ValueRO.Damage + randomComponent.ValueRO.NextFloat(fixedTickTracker.ValueRO.StepCount, -_damageDealer.ValueRO.DamageOffset / 2, _damageDealer.ValueRO.DamageOffset / 2)) });
                        }
                        else
                        {
                            DynamicBuffer<DamageComponent> buffer = ecbp.AddBuffer<DamageComponent>(sortKey, _searchable.ValueRO.Target);
                            buffer.Add(new DamageComponent {  AttackPrefab= _damageDealer.ValueRO.AttackPrefab, Damage = (_damageDealer.ValueRO.Damage + randomComponent.ValueRO.NextFloat(fixedTickTracker.ValueRO.StepCount, -_damageDealer.ValueRO.DamageOffset / 2, _damageDealer.ValueRO.DamageOffset / 2)) });
                        }

                    }
                    _damageDealer.ValueRW.CoolDownCounter = randomComponent.ValueRO.NextFloat(fixedTickTracker.ValueRO.StepCount ,- _damageDealer.ValueRO.CoolDownOffset / 2, _damageDealer.ValueRO.CoolDownOffset / 2);
                    
                }
            }
            
        }
    }


    [BurstCompile]
    public partial struct DamagingJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecbp;
        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey,Entity entity, DynamicBuffer<DamageComponent> _buffer, RefRW<HealthComponent> _health)
        {

            NativeHashMap<Entity, int> attacprefabs = new NativeHashMap<Entity, int>(1024,Allocator.Temp);


            for (int i = 0; i < _buffer.Length; i++)
            {
                _health.ValueRW.Value -= _buffer[i].Damage;
                if (_buffer[i].AttackPrefab!=Entity.Null)
                {
                    if(attacprefabs.TryGetValue(_buffer[i].AttackPrefab,out int item))
                    {
                        attacprefabs[_buffer[i].AttackPrefab] = item + 1;
                    }
                    else
                    {
                        attacprefabs.Add(_buffer[i].AttackPrefab,1);
                    }
                }
            }

            _buffer.Clear();

            foreach (Entity key in attacprefabs.GetKeyArray(Allocator.Temp))
            {
                Entity spawnedentity = ecbp.Instantiate(sortKey, key);
                ecbp.AddComponent(sortKey, spawnedentity, new Parent { Value = entity });
                ecbp.SetComponent(sortKey, spawnedentity, new LocalTransform { Position = new float3(0, 0, 0), Scale = 1, Rotation = new quaternion() });
            }

            
        }
    }




}
