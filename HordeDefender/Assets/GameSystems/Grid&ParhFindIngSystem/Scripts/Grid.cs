using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GridAnchor
{
    UPPERLEFT,
    MIDDLELEFT,
    BOTTOMLEFT,
    UPPERCENTER,
    CENTER,
    BOTTOMCENTER,
    UPPERRIGHT,
    MIDDLERIGHT,
    BOTTOMRIGHT
}

public class Grid<T>
{
    public class OnGridValueChangedEventArgs : System.EventArgs
    {
        public int x;
        public int y;
    }

    public event System.EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;

    public int width;
    public int height;
    public float cellSize;
    public Vector3 originPosition;

    public GridAnchor gridAnchor;

    public T[,] gridArray;


    public Grid(int width, int height, float cellSize, Vector3 originPosition,Func<Grid<T>,int,int,T> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridArray = new T[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                gridArray[x, y] = createGridObject(this, x, y);
            }
        }
    }

    public Grid(int width, int height, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        gridArray = new T[width, height];
    }

    public Grid()
    {
        
    }
    public void SetGridAnchor(GridAnchor gridAnchor)
    {
        this.gridAnchor = gridAnchor;
    }
    public void SetGridSize(int width, int height)
    {
        this.width = width;
        this.height = height;
        gridArray = new T[width, height];
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize+new Vector3(1,1)*cellSize/2+ Offset() + originPosition;
    }

    private Vector3 Offset()
    {
        Vector3 offset = Vector3.zero;
        switch(gridAnchor)
        {
            case GridAnchor.UPPERLEFT:
                offset = -new Vector3(0, height * cellSize);
                break;
            case GridAnchor.MIDDLELEFT:
                offset = -new Vector3(0, height * cellSize / 2);
                break;
            case GridAnchor.BOTTOMLEFT:
                offset = -new Vector3(0, 0);
                break;
            case GridAnchor.UPPERCENTER:
                offset = -new Vector3(width * cellSize / 2, height * cellSize);
                break;
            case GridAnchor.CENTER:
                offset = -new Vector3(width * cellSize / 2, height * cellSize / 2);
                break;
            case GridAnchor.BOTTOMCENTER:
                offset = -new Vector3(width * cellSize / 2,0);
                break;
            case GridAnchor.UPPERRIGHT:
                offset = -new Vector3(width * cellSize, height * cellSize);
                break;
            case GridAnchor.MIDDLERIGHT:
                offset = -new Vector3(width * cellSize, height * cellSize / 2);
                break;
            case GridAnchor.BOTTOMRIGHT:
                offset = -new Vector3(width * cellSize,0);
                break;
        }

        return offset;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(((worldPosition-originPosition-Offset()).x) / cellSize);
        y = Mathf.FloorToInt(((worldPosition - originPosition - Offset()).y) / cellSize);
    }

    public void SetGridObject(int x, int y, T value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, y = y });
        }
    }

    public void SetGridObject(Vector3 worldPosition, T value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetGridObject(x, y, value);
    }

    public T GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(T);
        }
    }

    public T GetGridObject(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetGridObject(x, y);
    }

    public Vector3 GetOriginPosition()
    {
        return originPosition;
    }
    
    public T[,] GetGrid()
    {
        return gridArray;
    }
}
