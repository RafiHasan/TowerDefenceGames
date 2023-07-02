using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntityTagAuthoring : MonoBehaviour
{
    
}


public class EntityTagBaker : Baker<EntityTagAuthoring>
{
    public override void Bake(EntityTagAuthoring authoring)
    {
        AddComponent(new EntityTag());
    }
}