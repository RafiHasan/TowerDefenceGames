using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public enum SearchingTagID
{
    NONE,
    BUILDING,
    PLAYER,
    ENEMY
}
public struct SearchingTag : IComponentData
{
    public SearchingTagID TagID;
}

public class SearchingTagAuthoring : MonoBehaviour
{
    public SearchingTagID TagID;
}

public class SearchingTagBaker : Baker<SearchingTagAuthoring>
{
    public override void Bake(SearchingTagAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new SearchingTag
        {
            TagID= authoring.TagID
        });
    }
}