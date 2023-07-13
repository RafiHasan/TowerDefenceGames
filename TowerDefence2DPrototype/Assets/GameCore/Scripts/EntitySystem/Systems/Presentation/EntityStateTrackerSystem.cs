using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
[UpdateInGroup(typeof(NormalSystemGroup))]
[UpdateBefore(typeof(PresentationLayerSystem))]
[BurstCompile]
public partial struct EntityStateTrackerSystem : ISystem
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
        new PresentationEngageTargetTrackerJob
        {
            LookUpValidity = SystemAPI.GetComponentLookup<LocalTransform>(),
        }.ScheduleParallel(state.Dependency).Complete();

        new PresentationHealthTrackerJob
        {

        }.ScheduleParallel(state.Dependency).Complete();
    }

    [BurstCompile]
    public partial struct PresentationEngageTargetTrackerJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<LocalTransform> LookUpValidity;
        [BurstCompile]
        public void Execute(RefRO<SearchAbleComponent> _searchAble, RefRW<PresentationDataComponent> _presentationData)
        {
            bool IsValidEntity = LookUpValidity.HasComponent(_searchAble.ValueRO.Target);
            if (_searchAble.ValueRO.Target != Entity.Null && IsValidEntity)
            {
                _presentationData.ValueRW.entityState = EntityState.ATTACK;
            }
            else
            {
                _presentationData.ValueRW.entityState = EntityState.WALK;
            }
        }
    }

    [BurstCompile]
    public partial struct PresentationHealthTrackerJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(RefRO<DelayDestroyComponent> _delayDestroy, RefRW<PresentationDataComponent> _presentationData)
        {
            _presentationData.ValueRW.entityState = EntityState.DEAD;
        }
    }

}
