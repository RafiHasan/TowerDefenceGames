using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DelayDestroyAuthoring : MonoBehaviour
{
    public float Delay;
}
public class DelayDestroyBaker : Baker<DelayDestroyAuthoring>
{
    public override void Bake(DelayDestroyAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new DelayDestroyComponent
        {
           Delay=authoring.Delay
        });
    }
}