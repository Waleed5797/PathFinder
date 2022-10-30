using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

public class Grid : IDisposable
{
    public float DiaognalCost; 
    public float straightCost;

    Dictionary<(int, int), GridTile> _nodes;
    int gridRows, gridColums;
    Vector2 tileSize;
    public Grid(int rows, int columns, Vector2 tileSize)
    {
        if (rows <= 0 || columns <= 0)
        {
            Debug.Log("Issue with grid diemensions");
            return;
        }

        gridRows = rows;
        gridColums = columns;
        this.tileSize = tileSize;
        if (this.tileSize.magnitude < 0.01f)
        {
            Debug.Log("Tile Size Issue");
        }
        _nodes = new Dictionary<(int, int), GridTile>();
        for (int rowIndex = 0; rowIndex < columns; rowIndex++)
        {
            for (int columIndex = 0; columIndex < rows; columIndex++)
            {
                var n = new GridTile(GetNodeWorldPosition(rowIndex, columIndex));
                n.index = (rowIndex, columIndex);
                _nodes.Add((rowIndex, columIndex), n);
            }
        }
    }

    Vector3 GetNodeWorldPosition(int rowNumber, int coloumNumber)
    {
        return new Vector3(rowNumber * tileSize.x, 0, coloumNumber * tileSize.y);
    }

    (int, int) GetNodeIndexFromWorldPosition(Vector3 iPosition)
    {
        var r = (int)(iPosition.x / tileSize.x);
        var c = (int)(iPosition.z / tileSize.y);
        if (r < gridRows && c < gridColums)
        {
            return (r, c);
        }
        return (-1, -1);
    }

    public Vector3 GetWorldPositionOfNode(int rowNumber, int coloumNumber)
    {
        var gnode = new GridTile(Vector3.zero);
        if (_nodes.TryGetValue((rowNumber, coloumNumber), out gnode))
        {
            return gnode.WorldPosition;
        }
        return Vector3.zero;
    }
    public (int, int) GetNodeIndexFromWroldPosition(Vector3 worldPosition)
    {
        return GetNodeIndexFromWorldPosition(worldPosition);
    }
    public List<GridTile> GetGridNodes() => _nodes.Values.ToList();

    public void Dispose() => _nodes = null;

    public Vector2 GetTileSize() => tileSize;

    public List<GridTile> GetNeighbourNodes(GridTile currentTile)
    {
        List<GridTile> neighbourTiles = new List<GridTile>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                var rowcheck = currentTile.GetIndex().Item1 + i;
                var columncheck = currentTile.GetIndex().Item2 + j;

                if (rowcheck >= 0 && rowcheck < gridRows && columncheck >= 0 && columncheck < gridColums)
                {
                    neighbourTiles.Add(_nodes[(rowcheck, columncheck)]);
                }
            }
        }
        return neighbourTiles;
    }

    public void DrawGrid() 
    {
        var GridView = new GameObject("Grid Viewer").AddComponent<GridViewer>();
        GridView.DrawGrid(this);
    }
}

// Only for Drawing
public class GridViewer : MonoBehaviour
{
    Grid gridToView;

    public void DrawGrid(Grid grid)
    {
        gridToView = grid;
        StartCoroutine(CreateTilesAysnc(grid));
    }

    IEnumerator CreateTilesAysnc(Grid grid) // Check for Async
    {
        yield return new WaitForSeconds(1);
        var tileSize = grid.GetTileSize();
        var allTiles = gridToView.GetGridNodes();

        Color displayColor = Color.blue;
        Material mat = new Material(Shader.Find("Unlit/Color"));

        for (int i = 0, c = allTiles.Count; i < c; i++)
        {
            var tile = allTiles[i];
            if (tile.Status.Equals(GridTileState.locked))
            {
                displayColor = Color.yellow;
            }
            else if (tile.Status.Equals(GridTileState.obstacle))
            {
                displayColor = Color.red;
            }
            else if (tile.Status.Equals(GridTileState.unlocked))
            {
                displayColor = Color.green;
            }

            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var co = go.GetComponent<Collider>();
            Destroy(co);
            go.transform.localScale = new Vector3(tileSize.x, 0, tileSize.y);
            go.transform.parent = transform;
            go.transform.position = allTiles[i].WorldPosition;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            mat.color = displayColor;
            go.isStatic = true;
        }
        this.gameObject.isStatic = true;
        StaticBatchingUtility.Combine(this.gameObject);
    }

    private void OnDestroy()
    {
        gridToView.Dispose();
    }
}