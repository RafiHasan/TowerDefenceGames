using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup),OrderFirst =true)]
public partial class FixedTickSystemGroup : ComponentSystemGroup
{
    float timar=0.125f;
    float timecounter;
    protected override void OnUpdate()
    {
        timecounter += SystemAPI.Time.DeltaTime;
        if (timecounter < timar)
            return;
        timecounter = 0;
        base.OnUpdate();
    }
}
