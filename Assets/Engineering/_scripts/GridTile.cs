using System;
using System.Collections.Generic;
using UnityEngine;

public enum GridTileState
{
    Unknown,
    locked,
    unlocked,
    occupied,
    obstacle
}
public class GridTile : IEquatable<GridTile>, IEqualityComparer<GridTile>
{
    public int Gcost;
    public int Hcost;
    public int fcost { get { return Gcost + Hcost; } }
    public GridTile parent;
    public (int, int) index;
    public GridTileState TileStatus;
    public bool isWalkable = true;
    Vector3 worldPosition;
    public Vector3 WorldPosition { get { return worldPosition; } }
    public GridTileState Status { get { return TileStatus; } }

    public GridTile(Vector3 worldPosition)
    {
        this.worldPosition = worldPosition;
    }
    public bool IsLocked() => TileStatus.Equals(GridTileState.locked);
    public bool IsOccupied() => TileStatus.Equals(GridTileState.occupied);
    public bool IsObstacle() => TileStatus.Equals(GridTileState.obstacle);
    public void SetObstacleStatus() => TileStatus = GridTileState.obstacle;
    public void SetLockedStatus() => TileStatus = GridTileState.locked;
    public void SetOccupiedStatus() => TileStatus = GridTileState.occupied;
    public void SetIndex((int, int) nodeIndex) => index = nodeIndex;
    public (int, int) GetIndex() => index;

    public bool Equals(GridTile x, GridTile y)
    {
        return x.index.Item2 == y.index.Item2 && x.index.Item1 == y.index.Item1;
    }
    public int GetHashCode(GridTile obj)
    {
        return HashCode.Combine(obj.index.Item1, obj.index.Item2);
    }
    public bool Equals(GridTile other)
    {
        return index.Item2 == other.index.Item2 && index.Item1 == other.index.Item1;
    }
}
