using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public struct RandomComponent : IComponentData
{
    public Unity.Mathematics.Random random;
}
