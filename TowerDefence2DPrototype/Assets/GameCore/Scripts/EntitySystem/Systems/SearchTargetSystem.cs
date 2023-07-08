using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public struct SearchData
{
    public Entity entity;
    public SearchingTagID SearchTag;
    public float3 LocalTransform;
}
[UpdateInGroup(typeof(FixedTickSystemGroup))]
[UpdateAfter(typeof(GridSystem))]

[BurstCompile]
public partial struct SearchTargetSystem : ISystem
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
        int count = 0;
        foreach (SearchAspect searchAspect in SystemAPI.Query<SearchAspect>())
        {
            count++;
        }

        NativeArray<SearchData> tempsearchAspects = new NativeArray<SearchData>(count, Allocator.Persistent);
        count = 0;
        foreach (SearchAspect searchAspect in SystemAPI.Query<SearchAspect>())
        {
            tempsearchAspects[count] = new SearchData { entity= searchAspect.entity, SearchTag= searchAspect.SearchTag.ValueRO.TagID,LocalTransform=searchAspect.LocalTransform.ValueRO.Position };
            count++;
        }

        JobHandle handle=new SearchTargetJob
        {
            searchAspects = tempsearchAspects
        }.ScheduleParallel(state.Dependency);

        new EngageTargetJob
        {
            
        }.ScheduleParallel(handle).Complete();

        tempsearchAspects.Dispose();

    }

    [BurstCompile]
    public partial struct SearchTargetJob : IJobEntity
    {
        [ReadOnly]
        public NativeArray<SearchData> searchAspects;
        [BurstCompile]
        public void Execute(RefRW<SearchAbleComponent> _searchAble,RefRO<LocalTransform> _localTransform)
        {
            if (_searchAble.ValueRO.Target != Entity.Null)
            {
                SearchData searchAspect = new SearchData();
                bool haslocaltoworld = false;

                for (int i = 0; i < searchAspects.Length; i++)
                {
                    if (_searchAble.ValueRO.Target == searchAspects[i].entity)
                    {
                        haslocaltoworld = true;
                        searchAspect = searchAspects[i];
                    }
                }


                if (haslocaltoworld)
                {
                    float3 val = searchAspect.LocalTransform;
                    if (math.distance(val, _localTransform.ValueRO.Position) < _searchAble.ValueRO.SearchRadious)
                    {

                    }
                    else
                    {
                        _searchAble.ValueRW.Target = Entity.Null;
                    }
                }
                else
                {
                    _searchAble.ValueRW.Target = Entity.Null;

                }

                return;
            }

            for (int i = 0; i < searchAspects.Length; i++)
            {
                if (searchAspects[i].SearchTag != _searchAble.ValueRO.TargetTagID)
                    continue;
                float3 val = searchAspects[i].LocalTransform;
                if (math.distance(val, _localTransform.ValueRO.Position) < _searchAble.ValueRO.SearchRadious)
                {
                    _searchAble.ValueRW.Target = searchAspects[i].entity;
                    break;
                }

            }


        }
    }

    [BurstCompile]
    public partial struct EngageTargetJob : IJobEntity
    {
        public void Execute(Entity entity, RefRO<StatsComponent> _stats, RefRO<SearchAbleComponent> _searchAble, RefRW<MovementComponent> _movement)
        {

            if(_searchAble.ValueRO.Target!=Entity.Null)
            {
                _movement.ValueRW.Speed = 0;
            }
            else
            {
                _movement.ValueRW.Speed = _stats.ValueRO.GetStatValue(StatID.SPEED);
            }
        }
    }
}
