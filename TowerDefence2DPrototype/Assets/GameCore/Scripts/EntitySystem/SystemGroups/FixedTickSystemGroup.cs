using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup),OrderFirst =true)]
public partial class FixedTickSystemGroup : ComponentSystemGroup
{
    float timar=0.125f;
    double timecounter;

    protected override void OnCreate()
    {
        base.OnCreate();
        timecounter = SystemAPI.Time.ElapsedTime;
    }
    protected override void OnUpdate()
    {
        if (SystemAPI.Time.ElapsedTime < timecounter)
            return;

        if (!SystemAPI.HasSingleton<FIxedTickTrackerComponent>())
            return;

        RefRW<FIxedTickTrackerComponent> fIxedTickTrackerComponent= SystemAPI.GetSingletonRW<FIxedTickTrackerComponent>();
        fIxedTickTrackerComponent.ValueRW.StepCount++;
        timecounter += timar;

        base.OnUpdate();
    }
}
