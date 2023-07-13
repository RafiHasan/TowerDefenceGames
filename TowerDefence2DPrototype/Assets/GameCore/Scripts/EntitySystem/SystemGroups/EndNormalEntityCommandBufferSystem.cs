using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections;
using Unity.Entities;

[WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.Editor | WorldSystemFilterFlags.ThinClientSimulation)]
[UpdateInGroup(typeof(NormalSystemGroup), OrderLast = true)]
public partial class EndNormalEntityCommandBufferSystem : EntityCommandBufferSystem
{
    public unsafe struct Singleton : IComponentData, IECBSingleton
    {
        internal UnsafeList<EntityCommandBuffer>* pendingBuffers;
        internal AllocatorManager.AllocatorHandle allocator;
        public EntityCommandBuffer CreateCommandBuffer(WorldUnmanaged world)
        {
            return EntityCommandBufferSystem.CreateCommandBuffer(ref *pendingBuffers, allocator, world);
        }
        public void SetPendingBufferList(ref UnsafeList<EntityCommandBuffer> buffers)
        {
            pendingBuffers = (UnsafeList<EntityCommandBuffer>*)UnsafeUtility.AddressOf(ref buffers);
        }
        public void SetAllocator(Allocator allocatorIn)
        {
            allocator = allocatorIn;
        }
        public void SetAllocator(AllocatorManager.AllocatorHandle allocatorIn)
        {
            allocator = allocatorIn;
        }
    }
    protected override void OnCreate()
    {
        base.OnCreate();

        this.RegisterSingleton<Singleton>(ref PendingBuffers, World.Unmanaged);
    }
}
