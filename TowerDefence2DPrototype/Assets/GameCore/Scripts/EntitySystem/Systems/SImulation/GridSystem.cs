using System.Globalization;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[UpdateInGroup(typeof(FixedTickSystemGroup),OrderFirst =true)]

[BurstCompile]
public partial class GridSystem : SystemBase
{
    private EntityCommandBufferSystem commandBufferSystemBegin;

    protected override void OnStartRunning()
    {
        commandBufferSystemBegin = World.GetOrCreateSystemManaged<BeginFixedTickEntityCommandBufferSystem>();
    }
    uint randomseed;

    int resetcounter = 0;
    protected override void OnUpdate()
    {
        
        if (!SystemAPI.HasSingleton<GridComponent>())
            return;

        RefRW<GridComponent> gridComponent = SystemAPI.GetSingletonRW<GridComponent>();

        DynamicBuffer<DynamicGridCellItem> gridCells = World.EntityManager.GetBuffer<DynamicGridCellItem>(gridComponent.ValueRO.entity);


        for(int i=0;i<gridCells.Length;i++)
        {
            gridCells[i] = new DynamicGridCellItem { entity = Entity.Null };
        }


        new GridCellCheckerJob
        {
            gridCells = gridCells,
            grid = gridComponent.ValueRO
        }.Schedule(new JobHandle()).Complete();

        if (!SystemAPI.HasSingleton<InputComponent>())
            return;

        RefRW<InputComponent> inputComponent = SystemAPI.GetSingletonRW<InputComponent>();

        if (!SystemAPI.HasSingleton<RandomComponent>())
            return;

        RefRW<RandomComponent> randomComponent = SystemAPI.GetSingletonRW<RandomComponent>();


        if (!SystemAPI.HasSingleton<GameUnitListComponent>())
            return;

        RefRW<GameUnitListComponent> gameUnitListComponent = SystemAPI.GetSingletonRW<GameUnitListComponent>();

        if (inputComponent.ValueRO.Reset)
        {
            resetcounter++;
            EntityCommandBuffer ecb = commandBufferSystemBegin.CreateCommandBuffer();//SystemAPI.GetSingleton<EndFixedTickEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            new CleanUpJob
            {
                ecbp= ecb.AsParallelWriter()
            }.Schedule();


            if(resetcounter==2)
            {
                inputComponent.ValueRW.Reset = false;
                inputComponent.ValueRW.GameStart = false;
                resetcounter = 0;
                foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("PL"))
                {
                    GameObject.Destroy(gameObject);
                }
            }
            return;
        }

        if (inputComponent.ValueRO.Selected)
        {
            EntityCommandBuffer ecb = commandBufferSystemBegin.CreateCommandBuffer();
            foreach (var (selectable, localtransform, entity) in SystemAPI.Query<SelectAbleTag, LocalTransform>().WithEntityAccess())
            {
                if(math.distance(localtransform.Position, gridComponent.ValueRO.GetCellPosition(gridComponent.ValueRO.GetCellIndex(inputComponent.ValueRO.WorldPosition)))<0.1f)
                {
                    ecb.AddComponent<SelectedTag>(entity);
                    gridComponent.ValueRW.UnSelectCell();
                    inputComponent.ValueRW.Selected = false;
                    break;
                }
            }
        }
            



        if (inputComponent.ValueRO.Selected)
        {
            gridComponent.ValueRW.SelectCell(gridComponent.ValueRO.GetCellIndex(inputComponent.ValueRO.WorldPosition));
        }
        else if(inputComponent.ValueRO.DeSelected)
        {
            gridComponent.ValueRW.UnSelectCell();
        }

        if (inputComponent.ValueRO.Spawn && gridComponent.ValueRO.IsSelected() && inputComponent.ValueRO.ItemIndex!=1)
        {
            EntityCommandBuffer ecb= commandBufferSystemBegin.CreateCommandBuffer();//SystemAPI.GetSingleton<EndFixedTickEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            Entity mainentity = ecb.CreateEntity();
            ecb.AddComponent(mainentity, new SpawnComponent
            {
                gobject = gameUnitListComponent.ValueRO.UnitList[inputComponent.ValueRO.ItemIndex-1],
                localTransform = gameUnitListComponent.ValueRO.Parent,
                Position = gridComponent.ValueRO.GetCellPosition(gridComponent.ValueRO.SelectedCell.x, gridComponent.ValueRO.SelectedCell.y),
                delay = 0,
                delaycounter = 0,
                count = 1
            });
            ecb.AddComponent(mainentity, new SpawnedTag
            {
                Index= inputComponent.ValueRO.ItemIndex - 1
            });
            gridComponent.ValueRW.UnSelectCell();
        }
        else if(inputComponent.ValueRO.Spawn && inputComponent.ValueRO.ItemIndex == 1 && !inputComponent.ValueRO.GameStart)
        {
            EntityCommandBuffer ecb = commandBufferSystemBegin.CreateCommandBuffer(); //SystemAPI.GetSingleton<EndFixedTickEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            Entity mainentity = ecb.CreateEntity();

            float customtime = ZombieCounter.Instance.timar / 0.125f;

            float customdelay;
            float customspawnperturn;

            if(ZombieCounter.Instance.ammount/ customtime<=1)
            {
                customdelay = ZombieCounter.Instance.timar / ZombieCounter.Instance.ammount;
                customspawnperturn = 1;
                
                
            }
            else
            {
                customdelay = 0;
                customspawnperturn = ZombieCounter.Instance.ammount / customtime;
            }


            ecb.AddComponent(mainentity, new SpawnComponent
            {
                gobject = gameUnitListComponent.ValueRO.UnitList[inputComponent.ValueRO.ItemIndex - 1],
                localTransform = gameUnitListComponent.ValueRO.Parent,
                Position = new float3(-13.5f,11.5f,0),
                delay = customdelay,
                spawnperturn = (int)customspawnperturn,
                count = ZombieCounter.Instance.ammount
            });
            
            ecb.AddComponent(mainentity, new SpawnedTag
            {
                Index = inputComponent.ValueRO.ItemIndex - 1
            });

            inputComponent.ValueRW.GameStart = true;
        }

        if(!inputComponent.ValueRO.GameStart)
        {
            try
            {
                if (randomseed != ZombieCounter.Instance.seed)
                {
                    randomComponent.ValueRW.InitState(ZombieCounter.Instance.seed);
                    randomseed = ZombieCounter.Instance.seed;
                }
            }
            catch
            {

            }
            
        }


        new GridSelectorJob
        {
            grid = gridComponent.ValueRO
        }.ScheduleParallel();

        

        if(inputComponent.ValueRO.Delete && gridComponent.ValueRO.IsSelected())
        {           
            EntityCommandBuffer ecb = commandBufferSystemBegin.CreateCommandBuffer();
            int bufferIndex = gridComponent.ValueRO.GetBufferIndex(gridComponent.ValueRO.GetCellPosition(gridComponent.ValueRO.SelectedCell));
            if (gridCells[bufferIndex].entity != Entity.Null && World.EntityManager.HasComponent<Simulate>(gridCells[bufferIndex].entity))
            {
                if(!World.EntityManager.HasComponent<CleanUpTag>(gridCells[bufferIndex].entity))
                    ecb.AddComponent(gridCells[bufferIndex].entity, new CleanUpTag { });
            }
        }

        inputComponent.ValueRW.Clear();
    }


    [BurstCompile]
    public partial struct CleanUpJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecbp;

        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey, Entity entity, RefRW<SpawnedTag> _cleanup)
        {
            ecbp.RemoveComponent<SpawnedTag>(sortKey, entity);
            ecbp.AddComponent(sortKey, entity, new CleanUpTag { });
        }
    }

    [BurstCompile]
    public partial struct GridCellCheckerJob : IJobEntity
    {
        public DynamicBuffer<DynamicGridCellItem> gridCells;
        public GridComponent grid;

        [BurstCompile]
        public void Execute(Entity entity,RefRO<BlockerTag> _blockerTag, RefRO<LocalTransform> _transform)
        {
            int index = grid.GetBufferIndex(_transform.ValueRO.Position);
            gridCells[index] = new DynamicGridCellItem { entity=entity };
        }
    }



    [BurstCompile]
    public partial struct GridSelectorJob : IJobEntity
    {
        public GridComponent grid;

        [BurstCompile]
        public void Execute(RefRO<GridSelectorTag> _tag, RefRW<LocalTransform> _transform)
        {
            _transform.ValueRW.Position = grid.GetCellPosition(grid.SelectedCell);
        }
    }


}
