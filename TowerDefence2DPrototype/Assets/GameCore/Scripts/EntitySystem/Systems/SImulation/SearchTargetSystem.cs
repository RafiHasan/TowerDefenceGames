using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct SearchData
{
    public Entity entity;
    public SearchingTagID SearchTag;
    public float3 LocalTransform;
}


public struct SearchDataKey:IEquatable<SearchDataKey>
{
    public int3 position;
    public SearchingTagID SearchTag;

    public bool Equals(SearchDataKey other)
    {
        return ((position.Equals(other.position)) && (SearchTag == other.SearchTag));
    }

    public override int GetHashCode() {
        int hashresult = 0;
        hashresult = hashresult * 128 + 64 + position.x;
        hashresult = hashresult * 128 + 64 + position.y;
        hashresult = hashresult * 128 + 64 + position.z;
        hashresult = hashresult * 128 + (int)SearchTag;

        return hashresult;

    }
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
        
        NativeParallelMultiHashMap<SearchDataKey, SearchData> multiHashMap = new NativeParallelMultiHashMap<SearchDataKey, SearchData>(2048, Allocator.Persistent);
        foreach (SearchAspect searchAspect in SystemAPI.Query<SearchAspect>())
        {
            SearchData searchData= new SearchData { entity = searchAspect.entity, SearchTag = searchAspect.SearchTag.ValueRO.TagID, LocalTransform = searchAspect.LocalTransform.ValueRO.Position };
            SearchDataKey searchDataKey = new SearchDataKey { position = new int3((int)(searchData.LocalTransform.x / 4), (int)(searchData.LocalTransform.y / 4), 0), SearchTag = searchData.SearchTag };
            multiHashMap.Add(searchDataKey, searchData);
        }

        ComponentLookup<LocalTransform> transforms = SystemAPI.GetComponentLookup<LocalTransform>();

        new SearchTargetJob
        {
            transforms = transforms,
            multiHashMap = multiHashMap,
        }.ScheduleParallel(state.Dependency).Complete();

        new EngageTargetJob
        {
            LookUpValidity = SystemAPI.GetComponentLookup<LocalTransform>(),
        }.ScheduleParallel(state.Dependency).Complete();

        multiHashMap.Dispose();

    }

    [BurstCompile]
    public partial struct SearchTargetJob : IJobEntity
    {
        [ReadOnly]
        public NativeParallelMultiHashMap<SearchDataKey, SearchData> multiHashMap;
        [ReadOnly]
        public ComponentLookup<LocalTransform> transforms;

        [BurstCompile]
        public void Execute(RefRW<SearchAbleComponent> _searchAble,RefRO<LocalTransform> _localTransform)
        {

            bool IsValidEntity = transforms.HasComponent(_searchAble.ValueRO.Target);


            if (_searchAble.ValueRO.Target != Entity.Null && IsValidEntity)
            {
                bool haslocaltoworld = transforms.TryGetComponent(_searchAble.ValueRO.Target,out LocalTransform _componentData);

                if (haslocaltoworld)
                {
                    float3 val = _componentData.Position;
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


            for(int i=-1;i<=1;i++)
            {
                for(int j=-1;j<=1;j++)
                {
                    int3 stpos = new int3((int)(_localTransform.ValueRO.Position.x / 4)+i, (int)(_localTransform.ValueRO.Position.y / 4)+j, 0);

                    SearchDataKey key = new SearchDataKey { position = stpos, SearchTag = _searchAble.ValueRO.TargetTagID };

                    if (multiHashMap.TryGetFirstValue(key, out SearchData searchData, out NativeParallelMultiHashMapIterator<SearchDataKey> it))
                    {
                        if (math.distance(searchData.LocalTransform, _localTransform.ValueRO.Position) < _searchAble.ValueRO.SearchRadious)
                        {
                            _searchAble.ValueRW.Target = searchData.entity;
                            goto label;
                        }
                    }
                }
            }

        label:;

        }
    }

    [BurstCompile]
    public partial struct EngageTargetJob : IJobEntity
    {
        [ReadOnly]
        public ComponentLookup<LocalTransform> LookUpValidity;
        [BurstCompile]
        public void Execute(Entity entity, RefRO<StatsComponent> _stats, RefRO<SearchAbleComponent> _searchAble, RefRW<MovementComponent> _movement, RefRW<PLData> _plData)
        {
            bool IsValidEntity = LookUpValidity.HasComponent(_searchAble.ValueRO.Target);
            if (_searchAble.ValueRO.Target != Entity.Null && IsValidEntity)
            {
                _movement.ValueRW.Speed = 0;
                _plData.ValueRW.IsAttacking = true;

                _plData.ValueRW.IsWalking = false;

            }
            else
            {
                _movement.ValueRW.Speed = _stats.ValueRO.GetStatValue(StatID.SPEED);
                _plData.ValueRW.IsAttacking = false;

                _plData.ValueRW.IsWalking = true;
            }
        }
    }
}
