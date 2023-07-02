using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct RandomComponent : IComponentData
{
    public Unity.Mathematics.Random random;
}

public class RandomAuthoring : MonoBehaviour
{
    
}

public class RandomBaker : Baker<RandomAuthoring>
{
    public override void Bake(RandomAuthoring authoring)
    {
        TransformUsageFlags transformUsageFlags = new TransformUsageFlags();
        Entity entity = this.GetEntity(transformUsageFlags);
        AddComponent(entity, new RandomComponent
        {
            random=new Unity.Mathematics.Random(1)
        });
    }
}
