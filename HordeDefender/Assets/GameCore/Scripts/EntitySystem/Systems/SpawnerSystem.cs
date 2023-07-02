using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial class SpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityQuery entityQuery=EntityManager.CreateEntityQuery(typeof(EntityTag));
        int spawnAmmount = 1000;

        SpawnerComponent spawnerComponent=SystemAPI.GetSingleton<SpawnerComponent>();

        if(entityQuery.CalculateEntityCount()<spawnAmmount)
        {
            EntityManager.Instantiate(spawnerComponent.prefab);
        }
    }
}
