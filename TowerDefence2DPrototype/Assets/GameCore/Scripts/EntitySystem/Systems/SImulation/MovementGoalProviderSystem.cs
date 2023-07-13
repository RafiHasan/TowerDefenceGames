using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedTickSystemGroup))]
[UpdateAfter(typeof(SpawnerSystem))]
[UpdateAfter(typeof(SearchTargetSystem))]
[BurstCompile]
public partial struct MovementGoalProviderSystem : ISystem
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
        
        if (!SystemAPI.HasSingleton<GridComponent>())
            return;

        RefRW<GridComponent> gridComponent = SystemAPI.GetSingletonRW<GridComponent>();

        int enemycount = 0;
        int playercount = 0;
        foreach (SearchingTag search in SystemAPI.Query<SearchingTag>())
        {
            if(search.TagID==SearchingTagID.ENEMY)
                enemycount++;
            if (search.TagID == SearchingTagID.PLAYER)
                playercount++;
        }

        NativeArray<float3> tempenemypos = new NativeArray<float3>(enemycount, Allocator.Persistent);
        NativeArray<float3> tempplayerpos = new NativeArray<float3>(playercount, Allocator.Persistent);
        enemycount = 0;
        playercount = 0;
        foreach ((SearchingTag search,LocalTransform trans) in SystemAPI.Query<SearchingTag,LocalTransform>())
        {
            if (search.TagID == SearchingTagID.ENEMY)
            {
                tempenemypos[enemycount] = trans.Position;
                enemycount++;
            }
            if (search.TagID == SearchingTagID.PLAYER)
            {
                tempplayerpos[playercount] = trans.Position;
                playercount++;
            }
        }
        new GoalFinderJob
        {
            grid= gridComponent.ValueRO,
            tempenemypos = tempenemypos,
            tempplayerpos= tempplayerpos
        }.ScheduleParallel(state.Dependency).Complete();

        tempenemypos.Dispose();
        tempplayerpos.Dispose();
    }


    [BurstCompile]
    public partial struct GoalFinderJob : IJobEntity
    {
        [ReadOnly]
        public GridComponent grid;
        [ReadOnly]
        public NativeArray<float3> tempenemypos;
        [ReadOnly]
        public NativeArray<float3> tempplayerpos;
        
        [BurstCompile]
        public void Execute(RefRO<SearchingTag> _search,RefRW<MovementComponent> _movement,RefRO<LocalTransform> _transform)
        {
            if(_search.ValueRO.TagID==SearchingTagID.ENEMY)
            {
                //float3 mypos = _transform.ValueRO.Position;

                //float distance = float.MaxValue;
                float3 targetpos = grid.GetCellPosition(11,-14);
                /*for (int i = 0; i < tempplayerpos.Length; i++)
                {
                    if (math.distance(mypos, tempplayerpos[i]) < distance)
                    {
                        targetpos = tempplayerpos[i];
                        distance = math.distance(mypos, tempplayerpos[i]);
                    }
                }*/
                _movement.ValueRW.Goal = grid.GetCellIndex(targetpos);
            }
            else if(_search.ValueRO.TagID == SearchingTagID.PLAYER)
            {
                float3 mypos = _transform.ValueRO.Position;

                //float distance = float.MaxValue;
                float3 targetpos = _transform.ValueRO.Position;
                /*for (int i=0;i< tempenemypos.Length;i++)
                {
                    if(math.distance(mypos, tempenemypos[i])< distance)
                    {
                        targetpos = tempenemypos[i];
                        distance = math.distance(mypos, tempenemypos[i]);
                    }
                }*/
                _movement.ValueRW.Goal = grid.GetCellIndex(targetpos);

            }
        }
    }
}
