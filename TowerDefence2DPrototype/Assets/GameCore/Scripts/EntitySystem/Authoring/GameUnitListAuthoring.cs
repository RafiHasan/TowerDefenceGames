using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class GameUnitListAuthoring : MonoBehaviour
{
    public List<GameObject> UnitList;
}

public class GameUnitListBaker : Baker<GameUnitListAuthoring>
{
    public override void Bake(GameUnitListAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        CustomFixedList16<Entity> TempUnitLists = new CustomFixedList16<Entity>();

        for (int i = 0; i < authoring.UnitList.Count; i++)
        {
            Entity tempentity = GetEntity(authoring.UnitList[i], TransformUsageFlags.Dynamic);
            TempUnitLists.Add(tempentity);
        }

        AddComponent(entity, new GameUnitListComponent
        {
            Parent=GetEntity(authoring.transform, TransformUsageFlags.Dynamic),
            UnitList= TempUnitLists
        });
    }
}