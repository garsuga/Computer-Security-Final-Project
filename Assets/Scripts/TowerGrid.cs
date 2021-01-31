using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerGrid
{
    public class Position
    {
        public int x;
        public int y;

        public Position(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }

    public Vector3 Origin { get; }
    public float Scale { get; }
    public int Height { get; }
    public int Width { get; }
    public bool[] Disabled { get; }
    public bool[] Occupied { get; }

    public TowerGrid(int width, int height, float scale, Vector3 origin, bool[] disabled)
    {
        this.Height = height;
        this.Width = width;
        this.Origin = origin;
        this.Scale = scale;
        this.Disabled = disabled;

        this.Occupied = new bool[width * height];
    }

    public Position toGridPosition(Vector3 worldPos)
    {
        Vector3 localizedPosition = worldPos - Origin;

        localizedPosition = localizedPosition / Scale;

        if (localizedPosition.x < 0 || localizedPosition.x > Width)
        {
            return null;
        }

        if (localizedPosition.y < 0 || localizedPosition.y > Height)
        {
            return null;
        }

        int x = (int)localizedPosition.x;
        int y = (int)localizedPosition.y;

        return new Position(x, y);
    }

    public Vector3 toWorldPosition(Position gridPosition)
    {
        float localX = gridPosition.x * Scale;
        float localY = gridPosition.y * Scale;
        Vector3 localPosition = new Vector3(localX, localY);

        return Origin + localPosition;
    }

    public bool isGridPosDisabled(Position position)
    {
        return Disabled != null && Disabled.Length == Height * Width && Disabled[position.y * Width + position.x];
    }

    public void setGridPosDisabled(Position position, bool disabled)
    {
        Disabled[position.y * Width + position.x] = disabled;
    }

    public bool isGridPosOccupied(Position position)
    {
        return Occupied[position.y * Width + position.x];
    }

    public void setGridPositionOccupied(Position position, bool occupied)
    {
        Occupied[position.y * Width + position.x] = occupied;
    }

    public Vector3 snapToGrid(Vector3 worldPos, float z)
    {
        // snap tower to grid
        Vector3 snappedTowerPos = toWorldPosition(toGridPosition(worldPos));
        return new Vector3(snappedTowerPos.x, snappedTowerPos.y, z) + new Vector3(Scale / 2, Scale / 2);
    }
}
