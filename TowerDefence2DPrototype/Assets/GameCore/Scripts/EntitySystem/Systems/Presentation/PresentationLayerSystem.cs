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

            foreach (var (plgo, entity) in SystemAPI.Query<PLGameObjectComponent>().WithEntityAccess())
            {
                plgo.gameObject.SetActive(false);
                SpriteRenderer spriteRenderer = state.EntityManager.GetComponentObject<SpriteRenderer>(entity);
                spriteRenderer.enabled = true;
            }

            return;
        }


        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<BeginPresentationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach(var(plgo,entity) in SystemAPI.Query<PresentationComponent>().WithEntityAccess())
        {
            GameObject go = GameObject.Instantiate(plgo.Prefab,new Vector3(-5000,-5000,0),Quaternion.identity);
            commandBuffer.RemoveComponent<PresentationComponent>(entity);
            commandBuffer.AddComponent(entity, new PLGameObjectComponent { gameObject = go });
            commandBuffer.AddComponent(entity, new PLTransformComponent { transform = go.GetComponent<Transform>() });
            commandBuffer.AddComponent(entity, new PLAnimatorComponent { animator = go.GetComponent<Animator>() });
            commandBuffer.AddComponent(entity, new PLData { cleanuptime = float.MaxValue });
        }

        

        foreach (var (PLTransform, LTransform) in SystemAPI.Query<PLTransformComponent,LocalTransform>())
        {
            PLTransform.transform.position = LTransform.Position;
        }

        foreach (var (PLGO,PLAnimator,PLAnimData,entity) in SystemAPI.Query<PLGameObjectComponent,PLAnimatorComponent, RefRW<PLData>>().WithEntityAccess())
        {
            PLAnimator.animator.SetBool("Walk", PLAnimData.ValueRO.IsWalking);
            PLAnimator.animator.SetBool("Attack", PLAnimData.ValueRO.IsAttacking);
            PLAnimator.animator.SetBool("Death", PLAnimData.ValueRO.IsDead);
            PLAnimData.ValueRW.cleanuptime -= SystemAPI.Time.DeltaTime;

            if(PLAnimData.ValueRW.cleanuptime<0)
            {
                GameObject.Destroy(PLGO.gameObject);
                commandBuffer.RemoveComponent<PLAnimatorComponent>(entity);
                commandBuffer.RemoveComponent<PLTransformComponent>(entity);
                commandBuffer.RemoveComponent<PLData>(entity);
                commandBuffer.RemoveComponent<PLGameObjectComponent>(entity);
            }

        }

        foreach (var (plgo, entity) in SystemAPI.Query<PLGameObjectComponent>().WithEntityAccess())
        { 
            plgo.gameObject.SetActive(true);
            SpriteRenderer spriteRenderer = state.EntityManager.GetComponentObject<SpriteRenderer>(entity);
            spriteRenderer.enabled = false;
        }


    }


    
}
