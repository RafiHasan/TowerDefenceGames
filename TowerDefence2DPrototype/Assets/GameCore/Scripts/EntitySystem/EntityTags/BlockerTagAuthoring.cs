using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct BlockerTag:IComponentData,IEnableableComponent
{

}
public class BlockerTagAuthoring : MonoBehaviour
{
    
}


public class BlockerTagBaker : Baker<BlockerTagAuthoring>
{
    public override void Bake(BlockerTagAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new BlockerTag
        {

        });
    }
}