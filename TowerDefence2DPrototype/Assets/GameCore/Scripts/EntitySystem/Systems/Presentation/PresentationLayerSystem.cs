using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

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
                presentationObject.gameObject.SetActive(false);
                SpriteRenderer spriteRenderer = state.EntityManager.GetComponentObject<SpriteRenderer>(entity);
                spriteRenderer.enabled = true;
            }
            return;
        }

        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (plgo, entity) in SystemAPI.Query<PresentationComponent>().WithEntityAccess())
        {
            GameObject go = GameObject.Instantiate(plgo.Prefab, new Vector3(-5000, -5000, 0), Quaternion.identity);
            SpriteRenderer spriteRenderer = state.EntityManager.GetComponentObject<SpriteRenderer>(entity);
            go.GetComponent<SpriteRenderer>().color = spriteRenderer.color;
            commandBuffer.RemoveComponent<PresentationComponent>(entity);
            commandBuffer.AddComponent(entity, new PresentationObjectComponent { gameObject = go });
            commandBuffer.AddComponent(entity, new PresentationTransformComponent { transform = go.GetComponent<Transform>() });
            commandBuffer.AddComponent(entity, new PresentationAnimatorComponent { animator = go.GetComponent<Animator>() });
            commandBuffer.AddComponent(entity, new PresentationDataComponent { });
        }


        

        foreach (var (presentationObject, entity) in SystemAPI.Query<PresentationObjectComponent>().WithEntityAccess())
        {
            presentationObject.gameObject.SetActive(true);
            SpriteRenderer spriteRenderer = state.EntityManager.GetComponentObject<SpriteRenderer>(entity);
            spriteRenderer.enabled = false;
            
        }

        foreach (var (presentationTransform, localTransform) in SystemAPI.Query<PresentationTransformComponent,LocalTransform>())
        {
            presentationTransform.transform.position = localTransform.Position;
        }

        foreach (var (presentationAnimator,presentationData,entity) in SystemAPI.Query<PresentationAnimatorComponent, PresentationDataComponent>().WithEntityAccess())
        {
            if(presentationData.entityState==EntityState.WALK)
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
            }
            
        }

        


    }


    
}
