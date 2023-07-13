using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedTickSystemGroup))]
[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    double time;
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
        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<BeginFixedTickEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        float deltaTime = (float)(SystemAPI.Time.ElapsedTime - time);
        time = SystemAPI.Time.ElapsedTime;
        new SpawnerJob
        {
            deltaTime= deltaTime,
            ecbp = commandBuffer.AsParallelWriter()
        }.ScheduleParallel();
    }

    [BurstCompile]
    public partial struct SpawnerJob : IJobEntity
    {
        public float deltaTime;
        public EntityCommandBuffer.ParallelWriter ecbp;

        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey,Entity entity, RefRW<SpawnComponent> _spawnerComponent,RefRO<SpawnedTag> _cleanUp)
        {
            if (_spawnerComponent.ValueRO.count > 0)
            {
                _spawnerComponent.ValueRW.delaycounter += deltaTime;
                if (_spawnerComponent.ValueRO.delaycounter> _spawnerComponent.ValueRO.delay)
                {

                    int spawncount = _spawnerComponent.ValueRO.count > _spawnerComponent.ValueRO.spawnperturn ? _spawnerComponent.ValueRO.spawnperturn : _spawnerComponent.ValueRO.count;
                    
                    if (spawncount == 0)
                        spawncount = 1;

                    for (int i=0;i< spawncount;i++)
                    {
                        Entity spawnedentity = ecbp.Instantiate(sortKey, _spawnerComponent.ValueRO.gobject);

                        if (_spawnerComponent.ValueRO.localTransform != Entity.Null)
                        {
                            ecbp.AddComponent(sortKey, spawnedentity, new Parent { Value = _spawnerComponent.ValueRO.localTransform });
                            ecbp.AddComponent(sortKey, spawnedentity, new SpawnedTag {  Index=_cleanUp.ValueRO.Index });
                            ecbp.SetComponent<LocalTransform>(sortKey, spawnedentity, new LocalTransform { Position = _spawnerComponent.ValueRO.Position, Scale = 1, Rotation = new quaternion() });
                            DynamicBuffer<LinkedEntityGroup> group = ecbp.AddBuffer<LinkedEntityGroup>(sortKey, _spawnerComponent.ValueRO.localTransform);
                            group.Add(_spawnerComponent.ValueRO.localTransform);  // Always add self as first member of group.
                            group.Add(spawnedentity);
                        }
                        else
                        {

                        }
                    }
                    
                    _spawnerComponent.ValueRW.count-= spawncount;
                    _spawnerComponent.ValueRW.delaycounter = 0;
                }
                
            }
            else
            {
                ecbp.DestroyEntity(sortKey, entity);
            }
        }
    }

}
