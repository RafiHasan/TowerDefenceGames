using Unity.Entities;

public struct PLData : IComponentData
{
    public float cleanuptime;
    public bool IsDead;
    public bool IsWalking;
    public bool IsAttacking;
}
