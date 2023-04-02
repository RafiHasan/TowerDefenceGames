using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using static PathFindingDots;

public class PathFindingDots : MonoBehaviour
{
    public static int MOVE_STRAIGHT_COST = 10;
    public static int MOVE_DIAGONAL_COST = 14;

    
    public static List<Vector2Int> mapNodes = new List<Vector2Int>();
    public static List<FindpathJob> pathResult = new List<FindpathJob>();
    public static List<int> validpathindex = new List<int>();
    public static List<Vector2Int> FindPath(int2 startposition, Grid<Vector2Int> mainmap,List<Vector2Int> destinations)
    {
        mapNodes.Clear();
        pathResult.Clear();
        validpathindex.Clear();
        int2 gridsize = new int2(mainmap.width, mainmap.height);

        
        NativeArray<JobHandle> pathHandles = new NativeArray<JobHandle>(destinations.Count, Allocator.Temp);


        for(int i=0;i < destinations.Count;i++)
        {
            Vector2Int mapNode = destinations[i];
            int2 endposition = new int2(mapNode.x, mapNode.y);
            NativeArray<PathNode> pathNodes = new NativeArray<PathNode>(gridsize.x * gridsize.y, Allocator.Persistent);

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
                    pathNode.hCost = pathNode.CalculateDistanceCost(endposition)*2;
                    pathNode.CalculateFCost();
                    pathNode.walkable = true;
                    pathNode.parentIndex = -1;
                    
                    pathNodes[pathNode.index] = pathNode;
                }
            }

            FindpathJob findpathJob = new FindpathJob()
            {
                startposition = startposition,
                endposition = endposition,
                pathNodes = pathNodes,
                gridsize = gridsize
            };
            pathResult.Add(findpathJob);
            pathHandles[i] = findpathJob.Schedule();
            
        }

        JobHandle.CompleteAll(pathHandles);

        
        for (int i = 0; i < pathHandles.Length; i++)
        {
            int endNodeIndex = PathNode.CalculateIndex(pathResult[i].endposition.x, pathResult[i].endposition.y, gridsize.x);

            PathNode endNode = pathResult[i].pathNodes[endNodeIndex];
            if (endNode.parentIndex == -1)
            {
                //Debug.Log("Path not found");
            }
            else
            {
                validpathindex.Add(i);
            }
        }


        if(validpathindex.Count>0)
        {
            int index = validpathindex[UnityEngine.Random.Range(0, validpathindex.Count)];
            int endNodeIndex = PathNode.CalculateIndex(pathResult[index].endposition.x, pathResult[index].endposition.y, gridsize.x);
            PathNode endNode = pathResult[index].pathNodes[endNodeIndex];

            int x = endNode.x;
            int y = endNode.y;
            mapNodes.Add(mainmap.GetGridObject(x, y));
            while (endNode.parentIndex != -1)
            {
                endNode = pathResult[index].pathNodes[endNode.parentIndex];
                x = endNode.x;
                y = endNode.y;
                mapNodes.Add(mainmap.GetGridObject(x, y));
            }
            mapNodes.Reverse();
            
        }

        for (int i = 0; i < pathHandles.Length; i++)
        {
            pathResult[i].pathNodes.Dispose();
        }

        pathHandles.Dispose();
        return mapNodes;
    }

    [BurstCompile]
    public struct FindpathJob : IJob
    {
        public int2 startposition;
        public int2 endposition;
        public NativeArray<PathNode> pathNodes;
        public int2 gridsize;

        public void Execute()
        {

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
                    int tentativeGCost = currentNode.gCost + currentNode.CalculateDistanceCost(neighbourPosition)* neighbourNode.costModifier;
                    if (tentativeGCost < neighbourNode.gCost)
                    {
                        neighbourNode.parentIndex = currentNodeIndex;
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.hCost = neighbourNode.CalculateDistanceCost(endposition)*2;
                        neighbourNode.CalculateFCost();
                        pathNodes[neighbourNodeIndex] = neighbourNode;

                        if (!openList.Contains(neighbourNodeIndex))
                        {
                            openList.Add(neighbourNodeIndex);
                        }
                    }
                }

            }


            neighbourOffsets.Dispose();
            openList.Dispose();
            closedList.Dispose();
            
        }

        

        private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodes, PathNode endNode)
        {
            if (endNode.parentIndex == -1)
            {
                return new NativeList<int2>(Allocator.Temp);
            }
            else
            {
                NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
                path.Add(new int2(endNode.x, endNode.y));
                PathNode currentNode = endNode;
                while (currentNode.parentIndex != -1)
                {
                    PathNode parentNode = pathNodes[currentNode.parentIndex];
                    path.Add(new int2(parentNode.x, parentNode.y));
                    currentNode = parentNode;
                }
                return path;
            }
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
