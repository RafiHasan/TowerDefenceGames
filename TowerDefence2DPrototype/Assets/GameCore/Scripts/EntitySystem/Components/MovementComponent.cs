using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct MovementComponent : IComponentData,IEnableableComponent
{
    public float3 NextPosition;
    public float Speed;
    public int2 Goal;
    public bool PathCalculated;
}
