using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(FixedTickSystemGroup),OrderLast =true)]
[BurstCompile]
public partial struct FixedTickTimarSystem : ISystem
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
        float deltaTime = 0.125f;

        new SpawnTimarJob
        {
            deltaTime = deltaTime
        }.ScheduleParallel(state.Dependency).Complete();


        new DamageDealerTimarJob
        {
            deltaTime= deltaTime
        }.ScheduleParallel(state.Dependency).Complete();

        new BuffDebufferTimarJob
        {
            deltaTime = deltaTime
        }.ScheduleParallel(state.Dependency).Complete();

        new BuffDebuffTimarJob
        {
            deltaTime = deltaTime
        }.ScheduleParallel(state.Dependency).Complete();

        
    }

    [BurstCompile]
    public partial struct SpawnTimarJob : IJobEntity
    {
        public float deltaTime;
        [BurstCompile]
        public void Execute(RefRW<SpawnComponent> _timar)
        {
            _timar.ValueRW.delaycounter += deltaTime;
        }
    }

    [BurstCompile]
    public partial struct DamageDealerTimarJob : IJobEntity
    {
        public float deltaTime;
        [BurstCompile]
        public void Execute(RefRW<DamageDealerComponent> _timar)
        {
            _timar.ValueRW.CoolDownCounter += deltaTime;
        }
    }

    [BurstCompile]
    public partial struct BuffDebufferTimarJob : IJobEntity
    {
        public float deltaTime;
        [BurstCompile]
        public void Execute(RefRW<BuffDebufferComponent> _timar)
        {
            _timar.ValueRW.CoolDownCounter += deltaTime;
        }
    }

    [BurstCompile]
    public partial struct BuffDebuffTimarJob : IJobEntity
    {
        public float deltaTime;
        [BurstCompile]
        public void Execute(DynamicBuffer<BuffDebuffComponent> _buffer)
        {
            for (int i = 0; i < _buffer.Length; i++)
            {
                BuffDebuffComponent buffDebuffComponent = _buffer[i];
                buffDebuffComponent.EffectTime -= deltaTime;
                buffDebuffComponent.DeltaTime = deltaTime;
                _buffer[i] = buffDebuffComponent;
            }
        }
    }


}
