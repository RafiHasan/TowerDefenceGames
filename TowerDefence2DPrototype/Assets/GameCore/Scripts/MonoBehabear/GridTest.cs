using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridTest : MonoBehaviour
{
    public int2 GridSize;
    public float2 CellSize;
    public float3 Origin;
    public int2 SelectedCell;

    private void Start()
    {
        /*Debug.Log("GridTest "+ GetCellPosition(-1,-1));
        Debug.Log("GridTest " + GetCellIndex(new float3(-3.48f, -4.5f, 0)));*/
    }

    public float3 GetCellPosition(int x, int y)
    {
        return Origin + new float3(CellSize.x / 2 + CellSize.x * x, CellSize.y / 2 + CellSize.y * y, 0);
    }

    public float3 GetCellPosition(int2 sellIndex)
    {
        return GetCellPosition(sellIndex.x, sellIndex.y);
    }

    public int2 GetCellIndex(float3 position)
    {
        return new int2((int)math.floor(position.x - Origin.x), (int)math.floor(position.y - Origin.y));
    }

    public int GetBufferIndex(float3 position)
    {
        int2 index = GetCellIndex(position);
        index += GridSize / 2;
        return (index.x + index.y * GridSize.x);
    }

    public void SelectCell(int2 index)
    {
        if (ContainsCell(index))
            SelectedCell = index;
    }

    public void UnSelectCell()
    {
        SelectedCell = new int2(int.MaxValue, int.MaxValue);
    }

    public bool IsSelected()
    {
        return (SelectedCell.x != int.MaxValue || SelectedCell.y != int.MaxValue);
    }

    public bool ContainsCell(int2 index)
    {
        if (index.x > -GridSize.x / 2 && index.x < GridSize.x / 2 && index.y > -GridSize.y / 2 && index.y < GridSize.y / 2)
            return true;

        return false;
    }
}
