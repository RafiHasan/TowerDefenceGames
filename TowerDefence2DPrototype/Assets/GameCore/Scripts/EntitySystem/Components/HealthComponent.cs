using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct HealthComponent : IComponentData, IEnableableComponent
{
    public float Value;
}
