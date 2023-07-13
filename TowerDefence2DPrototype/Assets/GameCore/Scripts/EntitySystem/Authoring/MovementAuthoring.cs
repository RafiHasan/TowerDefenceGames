using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MovementAuthoring : MonoBehaviour
{
    public float3 NextPosition;
    public float Speed;
    public int2 Goal;
}

public class MovementBaker : Baker<MovementAuthoring>
{
    public override void Bake(MovementAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new MovementComponent
        {
            Speed=authoring.Speed,
            Goal=authoring.Goal,
            NextPosition=authoring.NextPosition

        });
    }
}