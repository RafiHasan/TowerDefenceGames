using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct EntityTagComponent : IComponentData
{

}

public class EntityTagAuthoring : MonoBehaviour
{
    
}

public class EntityTagBaker : Baker<EntityTagAuthoring>
{
    public override void Bake(EntityTagAuthoring authoring)
    {
        TransformUsageFlags transformUsageFlags = new TransformUsageFlags();
        Entity entity = this.GetEntity(transformUsageFlags);
        AddComponent(entity, new EntityTagComponent());
    }
}