using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public partial struct MovingISystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {



        if (!SystemAPI.HasSingleton<RandomComponent>())
            return;


        RefRW<RandomComponent> randomComponent = SystemAPI.GetSingletonRW<RandomComponent>();
        float deltaTime = SystemAPI.Time.DeltaTime;


        new MoveDirectionJob
        {
            deltaTime = deltaTime
        }.ScheduleParallel();

        JobHandle jobHandle = new MovePositionJob
        {
            deltaTime = deltaTime
        }.Schedule(state.Dependency);

        jobHandle.Complete();
        

        new TestReachedTargetPositionJob
        {
            randomComponent = randomComponent
        }.Run();
    }
}

[BurstCompile]
public partial struct MovePositionJob : IJobEntity
{
    public float deltaTime;

    [BurstCompile]
    public void Execute(MoveToPositionAspect moveToPositionAspect)
    {
        moveToPositionAspect.Move(deltaTime);
    }
}

[BurstCompile]
public partial struct MoveDirectionJob : IJobEntity
{
    public float deltaTime;

    [BurstCompile]
    public void Execute(MoveToDirectionAspect moveToDirectionAspect)
    {
        moveToDirectionAspect.Move(deltaTime);
    }
}

[BurstCompile]
public partial struct TestReachedTargetPositionJob : IJobEntity
{
    [NativeDisableUnsafePtrRestriction] public RefRW<RandomComponent> randomComponent;

    [BurstCompile]
    public void Execute(MoveToPositionAspect moveToPositionAspect)
    {
        moveToPositionAspect.TestReachedtargetPosition(randomComponent);
    }
}