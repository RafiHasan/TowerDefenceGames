using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct GridSelectorTag : IComponentData
{

}
public class GridSelectorTagAuthoring : MonoBehaviour
{
    
}
public class GridSelectorTagBaker : Baker<GridSelectorTagAuthoring>
{
    public override void Bake(GridSelectorTagAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new GridSelectorTag
        {
            
        });
    }
}