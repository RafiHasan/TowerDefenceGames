using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PresentationSystemGroup),OrderFirst =true)]
public partial struct PresentationLayerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {

    }

    public void OnDestroy(ref SystemState state)
    {

    }

    public void OnUpdate(ref SystemState state)
    {

        if (ZombieCounter.Instance != null && !ZombieCounter.Instance.ShowPresentationLayer)
        {

            foreach (var (presentationObject, entity) in SystemAPI.Query<PresentationObjectComponent>().WithEntityAccess())
            {
                SpriteRenderer spriteRenderer = state.EntityManager.GetComponentObject<SpriteRenderer>(entity);
                spriteRenderer.enabled = true;
            }
            return;
        }
        
        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (plgo, entity) in SystemAPI.Query<PresentationComponent>().WithEntityAccess())
        {
            commandBuffer.RemoveComponent<PresentationComponent>(entity);
            commandBuffer.AddComponent(entity, new PresentationObjectComponent { itemsprite= plgo.Prefab.GetComponent<SpriteRenderer>().sprite});
            commandBuffer.AddComponent(entity, new PresentationTransformComponent {  });
            commandBuffer.AddComponent(entity, new PresentationAnimatorComponent {  });
            commandBuffer.AddComponent(entity, new PresentationDataComponent { });
        }
        
        foreach (var (presentationObject, localTransform, entity) in SystemAPI.Query<PresentationObjectComponent, LocalTransform>().WithEntityAccess())
        {
            SpriteRenderer spriteRenderer = state.EntityManager.GetComponentObject<SpriteRenderer>(entity);
            spriteRenderer.enabled = false;

            if (state.EntityManager.HasComponent<SelectedTag>(entity))
                continue;


            GpuInstanceRenderer.Instance.UdateVisualGPU(presentationObject.itemsprite, Matrix4x4.Translate(localTransform.Position), spriteRenderer.color);
        }

        foreach (var (seletecTag,presentationObject, localTransform, entity) in SystemAPI.Query<SelectedTag, PresentationObjectComponent, LocalTransform>().WithEntityAccess())
        {
            SpriteRenderer spriteRenderer = state.EntityManager.GetComponentObject<SpriteRenderer>(entity);
            spriteRenderer.enabled = false;
            GpuInstanceRenderer.Instance.UdateVisualGPU(presentationObject.itemsprite, Matrix4x4.TRS(localTransform.Position,Quaternion.identity,Vector3.one*1.25f), new Color(spriteRenderer.color.r*0.9f, spriteRenderer.color.g * 0.9f, spriteRenderer.color.b * 0.9f, spriteRenderer.color.a));
        }



        foreach (var (presentationAnimator,presentationData,entity) in SystemAPI.Query<PresentationAnimatorComponent, PresentationDataComponent>().WithEntityAccess())
        {
            /*if(presentationData.entityState==EntityState.WALK)
            {
                presentationAnimator.animator.SetBool("Walk", true);
                presentationAnimator.animator.SetBool("Attack", false);
            }
            else if(presentationData.entityState == EntityState.ATTACK)
            {
                presentationAnimator.animator.SetBool("Walk", false);
                presentationAnimator.animator.SetBool("Attack", true);
            }
            else if(presentationData.entityState == EntityState.DEAD)
            {
                presentationAnimator.animator.SetBool("Walk", false);
                presentationAnimator.animator.SetBool("Attack", false);
                presentationAnimator.animator.SetBool("Death", true);
            }*/
            
        }

        


    }


    
}
