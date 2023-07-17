using Unity.Entities;
using UnityEngine;

public struct SelectAbleTag : IComponentData
{

}

public class SelectAbleTagAuthoring : MonoBehaviour
{
    public int value;
}

public class SelectAbleTagBaker : Baker<SelectAbleTagAuthoring>
{
    public override void Bake(SelectAbleTagAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new SelectAbleTag
        {
            
        });
    }
}