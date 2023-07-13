using Unity.Entities;
using UnityEngine;

public class PresentationAuthoring : MonoBehaviour
{
    public GameObject Prefab;
}

public class PresentaionBaker : Baker<PresentationAuthoring>
{
    public override void Bake(PresentationAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponentObject(entity, new PresentationComponent
        {
            Prefab=authoring.Prefab
        });
    }
}
