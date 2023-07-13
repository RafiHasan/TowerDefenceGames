using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
            ecb.AddComponent(mainentity, new CleanUpTag
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
            
            ecb.AddComponent(mainentity, new CleanUpTag
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

        inputComponent.ValueRW.Clear();


        

        if(Input.GetKey(KeyCode.Backspace) && gridComponent.ValueRO.IsSelected())
        {
            EntityCommandBuffer ecb = commandBufferSystemBegin.CreateCommandBuffer();
            if (gridCells[gridComponent.ValueRO.GetBufferIndex(gridComponent.ValueRO.GetCellPosition(gridComponent.ValueRO.SelectedCell))].entity!=Entity.Null)
            {
                ecb.DestroyEntity(gridCells[gridComponent.ValueRO.GetBufferIndex(gridComponent.ValueRO.GetCellPosition(gridComponent.ValueRO.SelectedCell))].entity);
                gridCells[gridComponent.ValueRO.GetBufferIndex(gridComponent.ValueRO.GetCellPosition(gridComponent.ValueRO.SelectedCell))] = new DynamicGridCellItem { entity=Entity.Null};
            }
            
        }

    }


    [BurstCompile]
    public partial struct CleanUpJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecbp;

        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey, Entity entity, RefRW<CleanUpTag> _cleanup)
        {
            ecbp.DestroyEntity(sortKey,entity);
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
