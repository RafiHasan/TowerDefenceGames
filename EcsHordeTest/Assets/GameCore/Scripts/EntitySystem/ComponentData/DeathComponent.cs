using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DeathComponent : IComponentData
{
    public float delay;
    public Entity deathEffect;
}
