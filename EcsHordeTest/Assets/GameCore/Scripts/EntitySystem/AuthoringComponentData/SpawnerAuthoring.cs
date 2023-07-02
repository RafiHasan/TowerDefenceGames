using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct SpawnerComponent : IComponentData
{
    public int spawnAmmount;
    public int spawnedAmmount;
    public float3 spawnPosition;
    public Entity prefab;
}

public class SpawnerAuthoring : MonoBehaviour
{
    public int spawnAmmount;
    public GameObject prefab;
}

public class SpawnerBaker : Baker<SpawnerAuthoring>
{
    public override void Bake(SpawnerAuthoring authoring)
    {
        TransformUsageFlags transformUsageFlags = new TransformUsageFlags();
        Entity entity = this.GetEntity(transformUsageFlags);
        AddComponent(entity, new SpawnerComponent
        {
            prefab = GetEntity(authoring.prefab),
            spawnedAmmount=0,
            spawnPosition=authoring.transform.position,
            spawnAmmount =authoring.spawnAmmount
        });
    }
}