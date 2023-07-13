using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[System.Serializable]
public struct EntitySaveData:IComponentData
{
    public int Index;
    public float3 Position;
    public float Health;
}

public class SerializableList
{
    public List<EntitySaveData> list;
}

[UpdateInGroup(typeof(FixedTickSystemGroup), OrderFirst = true)]
[UpdateBefore(typeof(GridSystem))]
[BurstCompile]
public partial class SaveLoadSystem : SystemBase
{
    private EntityCommandBufferSystem commandBufferSystem;

    protected override void OnStartRunning()
    {
        commandBufferSystem = World.GetOrCreateSystemManaged<EndFixedTickEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        
        if (!SystemAPI.HasSingleton<InputComponent>())
            return;

        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();

        if (!SystemAPI.HasSingleton<GameUnitListComponent>())
            return;

        RefRW<GameUnitListComponent> gameUnitListComponent = SystemAPI.GetSingletonRW<GameUnitListComponent>();


        string inputstring=JsonUtility.ToJson(inputComponent.ValueRO);

        //Debug.Log(inputstring);
        EntityCommandBuffer ecb = commandBufferSystem.CreateCommandBuffer();
        /*new SaveJob
        {
            ecbp= ecb.AsParallelWriter()
        }.ScheduleParallel();*/


        if(inputComponent.ValueRO.Save)
        {
            inputComponent.ValueRW.Save = false;
            SerializableList saveDatas = new SerializableList { list=new List<EntitySaveData>() };

            Entities.WithoutBurst().ForEach((Entity entity, LocalTransform _transform, CleanUpTag _cleanup) => {

                saveDatas.list.Add(new EntitySaveData { Index = _cleanup.Index, Position = _transform.Position });

            }).Run();

            JsonEntitySaver = JsonUtility.ToJson(saveDatas);
        }


        if (inputComponent.ValueRO.Load)
        {
            inputComponent.ValueRW.Reset = true;
            inputComponent.ValueRW.Load = false;
            SerializableList saveDatas = JsonUtility.FromJson<SerializableList>(JsonEntitySaver);
            for (int i=0;i< saveDatas.list.Count;i++)
            {
                Entity mainentity = ecb.CreateEntity();
                ecb.AddComponent(mainentity, new SpawnComponent
                {
                    gobject = gameUnitListComponent.ValueRO.UnitList[saveDatas.list[i].Index],
                    localTransform = gameUnitListComponent.ValueRO.Parent,
                    Position = saveDatas.list[i].Position,
                    delay = 0,
                    delaycounter = 0,
                    count = 1
                });
                ecb.AddComponent(mainentity, new CleanUpTag
                {
                    Index = saveDatas.list[i].Index
                });
            }

            Debug.Log("Data Loaded");
        }



    }
    public string JsonEntitySaver="";
}
