using Unity.Entities;
using UnityEngine;

public struct SpeedComponent : IComponentData
{
    public float value;
}

public class SpeedAuthoring : MonoBehaviour
{
    public float value;
}


public class SpeedBaker : Baker<SpeedAuthoring>
{
    public override void Bake(SpeedAuthoring authoring)
    {
        TransformUsageFlags transformUsageFlags = new TransformUsageFlags();
        Entity entity = this.GetEntity(transformUsageFlags);
        AddComponent(entity, new SpeedComponent
        {
            value = authoring.value
        });
    }
}