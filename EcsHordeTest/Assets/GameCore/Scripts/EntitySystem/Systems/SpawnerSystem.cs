using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {

        /*if (!SystemAPI.HasSingleton<SpawnerComponent>())
            return;

        SpawnerComponent spawnerComponent = SystemAPI.GetSingleton<SpawnerComponent>();*/

        foreach(RefRW<SpawnerComponent> spawnerComponent in SystemAPI.Query<RefRW<SpawnerComponent>>())
        {
            if (spawnerComponent.ValueRO.spawnedAmmount < spawnerComponent.ValueRO.spawnAmmount)
            {
                GameManager.Instance.instancecount = spawnerComponent.ValueRO.spawnedAmmount;
                Entity entity=EntityManager.Instantiate(spawnerComponent.ValueRO.prefab);
                LocalTransform localTransform = EntityManager.GetComponentData<LocalTransform>(entity);
                EntityManager.SetComponentData(entity, new LocalTransform() { Position = spawnerComponent.ValueRO.spawnPosition,Scale= localTransform.Scale,Rotation= localTransform.Rotation });
                spawnerComponent.ValueRW.spawnedAmmount++;
            }
        }       
    }
}
