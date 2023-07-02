using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct SpawnerComponent : IComponentData
{
    public Entity prefab;
}