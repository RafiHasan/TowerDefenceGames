using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(FixedTickSystemGroup))]
public partial class NormalSystemGroup : ComponentSystemGroup
{
    protected override void OnUpdate()
    {
        base.OnUpdate();
    }
}
