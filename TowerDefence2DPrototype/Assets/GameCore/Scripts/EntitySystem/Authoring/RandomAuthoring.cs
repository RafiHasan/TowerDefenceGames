using Unity.Entities;
using UnityEngine;

public class RandomAuthoring : MonoBehaviour
{
    public uint Seed;
}

public class RandomBaker : Baker<RandomAuthoring>
{
    public override void Bake(RandomAuthoring authoring)
    {
        TransformUsageFlags transformUsageFlags = new TransformUsageFlags();
        Entity entity = this.GetEntity(transformUsageFlags);
        AddComponent(entity, new RandomComponent
        {
            random = new Unity.Mathematics.Random(authoring.Seed)
        });
    }
}