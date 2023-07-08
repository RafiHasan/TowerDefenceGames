using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class InputAuthoring : MonoBehaviour
{
    
}

public class InputBaker : Baker<InputAuthoring>
{
    public override void Bake(InputAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new InputComponent
        {
            
        });
    }
}