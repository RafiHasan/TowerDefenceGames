using Unity.Entities;


public enum EntityState
{
    WALK,
    ATTACK,
    DAMAGED,
    DEAD
}


public struct PresentationDataComponent : IComponentData
{
    public EntityState entityState;
}
