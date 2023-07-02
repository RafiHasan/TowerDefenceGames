using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public partial class PlayerInputSystemBase : SystemBase
{
    protected override void OnUpdate()
    {
        foreach(RefRW<TargetDirectionComponent> targetDirection in SystemAPI.Query<RefRW<TargetDirectionComponent>>())
        {
            Vector3 direction = Vector3.zero;
            if(Input.GetKey(KeyCode.W))
            {
                direction += new Vector3(0,0,1);
            }
            if (Input.GetKey(KeyCode.S))
            {
                direction -= new Vector3(0, 0, 1);
            }
            if (Input.GetKey(KeyCode.A))
            {
                direction -= new Vector3(1, 0, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                direction += new Vector3(1, 0, 0);
            }


            targetDirection.ValueRW.value = direction;
        }
    }
}
