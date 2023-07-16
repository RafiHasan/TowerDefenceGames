using System;
using System.Diagnostics;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedTickSystemGroup))]
[UpdateAfter(typeof(GridSystem))]
[UpdateAfter(typeof(MovementGoalProviderSystem))]
[UpdateAfter(typeof(SearchTargetSystem))]
[UpdateAfter(typeof(SpawnerSystem))]
[BurstCompile]
public partial struct PathFindingSystem : ISystem
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
        if (!SystemAPI.HasSingleton<GridComponent>())
            return;
        GridComponent gridComponent = SystemAPI.GetSingleton<GridComponent>();


        if (!SystemAPI.HasSingleton<RandomComponent>())
            return;
        RefRW<RandomComponent> randomComponent = SystemAPI.GetSingletonRW<RandomComponent>();


        DynamicBuffer<DynamicGridCellItem> gridCells = state.EntityManager.GetBuffer<DynamicGridCellItem>(gridComponent.entity);

        RefRW<FIxedTickTrackerComponent> fIxedTickTrackerComponent = SystemAPI.GetSingletonRW<FIxedTickTrackerComponent>();

        EntityCommandBuffer commandBuffer = SystemAPI.GetSingleton<EndFixedTickEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        new PathFindingJob
        {
            fIxedTickTrackerComponent = fIxedTickTrackerComponent.ValueRO,
            randomComponent = randomComponent,
            gridCells = gridCells,
            grid = gridComponent,
            ecbp = commandBuffer.AsParallelWriter()
        }.ScheduleParallel();

    }

    [BurstCompile]
    public partial struct PathFindingJob : IJobEntity
    {
        public FIxedTickTrackerComponent fIxedTickTrackerComponent;
        [NativeDisableUnsafePtrRestriction]
        public RefRW<RandomComponent> randomComponent;
        [ReadOnly]
        public DynamicBuffer<DynamicGridCellItem> gridCells;
        public GridComponent grid;
        public EntityCommandBuffer.ParallelWriter ecbp;
        [BurstCompile]
        public void Execute([EntityIndexInQuery] int sortKey, Entity entity, RefRW<MovementComponent> _movement, RefRO<LocalTransform> _transform)
        {

            if (_movement.ValueRO.PathCalculated == true || _movement.ValueRO.Speed==0)
                return;

            _movement.ValueRW.NextPosition = _transform.ValueRO.Position;

            int2 startposition = grid.GetCellIndex(_transform.ValueRO.Position);
            
            startposition += grid.GridSize/2;
            int2 endposition = _movement.ValueRO.Goal;
            endposition += grid.GridSize / 2;
            if (startposition.Equals(endposition))
                return;

            int2 gridsize = grid.GridSize;
            NativeArray<PathNode> pathNodes = new NativeArray<PathNode>(gridsize.x * gridsize.y, Allocator.Temp);

            for (int x = 0; x < gridsize.x; x++)
            {
                for (int y = 0; y < gridsize.y; y++)
                {
                    PathNode pathNode = new PathNode();
                    pathNode.x = x;
                    pathNode.y = y;
                    pathNode.index = PathNode.CalculateIndex(x, y, gridsize.x);
                    pathNode.costModifier = 1;
                    pathNode.gCost = int.MaxValue;
                    pathNode.hCost = pathNode.CalculateDistanceCost(endposition) * 2;
                    pathNode.CalculateFCost();
                    pathNode.walkable = (gridCells[pathNode.index].entity == Entity.Null);
                    pathNode.parentIndex = -1;

                    pathNodes[pathNode.index] = pathNode;
                }
            }



            NativeArray<int2> neighbourOffsets = new NativeArray<int2>(4, Allocator.Temp);



            neighbourOffsets[0] = new int2(0, 1);
            neighbourOffsets[1] = new int2(0, -1);
            neighbourOffsets[2] = new int2(-1, 0);
            neighbourOffsets[3] = new int2(1, 0);
            /*neighbourOffsets[4] = new int2(1, 1);
            neighbourOffsets[5] = new int2(-1, -1);
            neighbourOffsets[6] = new int2(1, -1);
            neighbourOffsets[7] = new int2(-1, 1);*/

            int endNodeIndex = CalculateIndex(endposition.x, endposition.y, gridsize.x);

            PathNode startNode = pathNodes[CalculateIndex(startposition.x, startposition.y, gridsize.x)];
            startNode.gCost = 0;
            startNode.CalculateFCost();
            pathNodes[startNode.index] = startNode;

            NativeList<int> openList = new NativeList<int>(Allocator.Temp);
            NativeList<int> closedList = new NativeList<int>(Allocator.Temp);
            openList.Add(startNode.index);

            while (openList.Length > 0)
            {
                int currentNodeIndex = GetLowestFCostNodeIndex(openList, pathNodes);
                PathNode currentNode = pathNodes[currentNodeIndex];
                if (endNodeIndex == currentNodeIndex)
                {
                    break;
                }

                for (int i = 0; i < openList.Length; i++)
                {
                    if (openList[i] == currentNodeIndex)
                    {
                        openList.RemoveAtSwapBack(i);
                        break;
                    }
                }

                closedList.Add(currentNodeIndex);

                for (int i = 0; i < neighbourOffsets.Length; i++)
                {
                    int2 neighbourOffset = neighbourOffsets[i];
                    int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);
                    if (!IsPositionInsideGrid(neighbourPosition, gridsize))
                    {
                        continue;
                    }

                    int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridsize.x);
                    if (closedList.Contains(neighbourNodeIndex))
                    {
                        continue;
                    }

                    PathNode neighbourNode = pathNodes[neighbourNodeIndex];
                    if (!neighbourNode.walkable)
                    {
                        continue;
                    }

                    int2 currentNodePosition = new int2(currentNode.x, currentNode.y);
                    int2 direction = new int2(neighbourPosition.x - currentNode.x, neighbourPosition.y - currentNode.y);
                    int tentativeGCost = currentNode.gCost + currentNode.CalculateDistanceCost(neighbourPosition) * neighbourNode.costModifier;
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.parentIndex = currentNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = neighbourNode.CalculateDistanceCost(endposition) * 2;
                        neighbourNode.CalculateFCost();
                        pathNodes[neighbourNodeIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNodeIndex))
                        {
                            openList.Add(neighbourNodeIndex);
                        }
                    }
                }

            }

            PathNode endNode = pathNodes[PathNode.CalculateIndex(endposition.x, endposition.y, gridsize.x)];

            PathNode tempendNode = endNode;
            DynamicBuffer<PathNodeBufferElement> buffer = ecbp.AddBuffer<PathNodeBufferElement>(sortKey, entity);

            while (tempendNode.parentIndex!=-1)
            {
                buffer.Add(new PathNodeBufferElement { x = tempendNode.x, y = tempendNode.y });
                tempendNode = pathNodes[tempendNode.parentIndex];               
            }
            
            if (endNode.parentIndex!=-1)
            {               
                _movement.ValueRW.NextPosition = grid.GetCellPosition(tempendNode.x-grid.GridSize.x/2, tempendNode.y - grid.GridSize.y / 2);
                _movement.ValueRW.PathCalculated = true;
            }
            neighbourOffsets.Dispose();
            openList.Dispose();
            closedList.Dispose();
            pathNodes.Dispose();

        }


        private bool IsPositionInsideGrid(int2 neighbourPosition, int2 gridsize)
        {
            return neighbourPosition.x >= 0 && neighbourPosition.x < gridsize.x && neighbourPosition.y >= 0 && neighbourPosition.y < gridsize.y;
        }



        private int CalculateIndex(int x, int y, int gridwidth)
        {
            return x + y * gridwidth;
        }

        private int GetLowestFCostNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodes)
        {
            PathNode lowestFCostNode = pathNodes[openList[0]];
            for (int i = 1; i < openList.Length; i++)
            {
                PathNode testPathNode = pathNodes[openList[i]];
                if (testPathNode.fCost < lowestFCostNode.fCost)
                {
                    lowestFCostNode = testPathNode;
                }
            }
            return lowestFCostNode.index;

        }


    }

    public struct PathNodeBufferElement:IBufferElementData
    {
        public int x;
        public int y;
    }


    [BurstCompile]
    public struct PathNode
    {
        public int x;
        public int y;
        public int index;
        public int gCost;
        public int hCost;
        public int fCost;

        public int costModifier;

        public bool walkable;

        public int parentIndex;

        public static bool IsPositionInsideGrid(int2 neighbourPosition, int2 gridsize)
        {
            return neighbourPosition.x >= 0 && neighbourPosition.x < gridsize.x && neighbourPosition.y >= 0 && neighbourPosition.y < gridsize.y;
        }
        public static int CalculateIndex(int x, int y, int gridwidth)
        {
            return x + y * gridwidth;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        public int CalculateDistanceCost(int2 from)
        {
            int2 to = new int2(x, y);
            int MOVE_STRAIGHT_COST = 10;
            int MOVE_DIAGONAL_COST = 14;
            int xDistance = math.abs(from.x - to.x);
            int yDistance = math.abs(from.y - to.y);
            int remaining = math.abs(xDistance - yDistance);
            return MOVE_STRAIGHT_COST * math.min(xDistance, yDistance) + MOVE_DIAGONAL_COST * remaining;
        }
    }

}
