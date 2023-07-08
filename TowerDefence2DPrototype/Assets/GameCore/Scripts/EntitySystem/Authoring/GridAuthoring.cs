using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static PathFindingSystem;

public class GridAuthoring : MonoBehaviour
{
    public int2 GridSize;
    public float2 CellSize;
    public float3 Origin;
}

public class GridBaker : Baker<GridAuthoring>
{
    public override void Bake(GridAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new GridComponent
        {
            entity=entity,
            GridSize = authoring.GridSize,
            CellSize = authoring.CellSize,
            Origin = authoring.Origin,
            SelectedCell=new int2(int.MaxValue,int.MaxValue),
        });
        DynamicBuffer<DynamicGridCellItem> gridcells = AddBuffer<DynamicGridCellItem>(entity);

        for(int i=0;i< authoring.GridSize.x* authoring.GridSize.y;i++)
        {
            gridcells.Add(new DynamicGridCellItem {  entity= Entity.Null });
        }

    }
}