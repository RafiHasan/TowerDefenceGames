using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct SearchAbleComponent : IComponentData
{
    public SearchingTagID TargetTagID;
    public Entity Target;
    public float SearchRadious;
}
