using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class SearchAbleAuthoring : MonoBehaviour
{
    public SearchingTagID TargetTagID;
    public float SearchRadious;
}

public class SearchAbleBaker : Baker<SearchAbleAuthoring>
{
    public override void Bake(SearchAbleAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new SearchAbleComponent
        {
            TargetTagID= authoring.TargetTagID,
            SearchRadious=authoring.SearchRadious
        });
    }
}