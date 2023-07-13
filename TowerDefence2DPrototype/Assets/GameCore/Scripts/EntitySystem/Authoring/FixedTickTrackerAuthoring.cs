using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class FixedTickTrackerAuthoring : MonoBehaviour
{
    
}


public class FixedTickTrackerBaker : Baker<FixedTickTrackerAuthoring>
{
    public override void Bake(FixedTickTrackerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(entity, new FIxedTickTrackerComponent
        {
            StepCount=0
        });
    }
}