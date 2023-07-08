using Unity.Entities;
using Unity.Mathematics;

public struct SpawnComponent : IComponentData
{
    public Entity gobject;
    public Entity localTransform;
    public float3 Position;
    public float delay;
    public float delaycounter;
    public int spawnperturn;
    public int count;
}
