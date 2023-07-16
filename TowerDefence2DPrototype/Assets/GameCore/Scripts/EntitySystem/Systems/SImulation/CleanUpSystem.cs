using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateInGroup(typeof(NormalSystemGroup), OrderLast = true)]
[BurstCompile]
public partial struct CleanUpSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<BeginNormalEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (cleanUp, entity) in SystemAPI.Query<CleanUpTag>().WithEntityAccess())
        {
            commandBuffer.RemoveComponent<CleanUpTag>(entity);

            if(state.EntityManager.HasComponent<PresentationObjectComponent>(entity))
            {
                commandBuffer.RemoveComponent<PresentationObjectComponent>(entity);
            }

            commandBuffer.DestroyEntity(entity);
        }
    }

}
