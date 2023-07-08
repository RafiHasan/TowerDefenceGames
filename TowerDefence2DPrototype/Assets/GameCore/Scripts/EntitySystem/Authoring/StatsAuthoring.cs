using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class StatsAuthoring : MonoBehaviour
{
    public Stat[] Stats;
}

public class StatsBaker : Baker<StatsAuthoring>
{
    public override void Bake(StatsAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        CustomFixedList16<Stat> customFixedList = new CustomFixedList16<Stat>();

        for(int i=0;i< authoring.Stats.Length && i<16;i++)
        {
            customFixedList.Add(authoring.Stats[i]);

            if (authoring.Stats[i].ID == StatID.HEALTH)
            {
                AddComponent(entity, new HealthComponent
                {
                    Value = authoring.Stats[i].value
                });
            }

        }

        AddComponent(entity, new StatsComponent
        {
            Stats= customFixedList
        });
    }
}
