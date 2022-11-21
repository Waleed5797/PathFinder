using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Grid;
using System.Linq;

public class PathFinder : MonoBehaviour
{
    public Transform StartPoint;
    public Transform DestinationPoint;

    GridTile startingNode;

    GridTile EndingNode;

    Grid myGrid;
    TestGrid nodedraw;
    bool pathfound;
    bool positionUpdated;

    List<(int, int)> NodesToDraw = new List<(int, int)>();

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        nodedraw = GameObject.FindObjectOfType<TestGrid>();
        myGrid = new Grid(100, 100, Vector2.one * 0.5f);

        startingNode = new GridTile( Vector3.zero);
        EndingNode = new GridTile(Vector3.zero);


        startingNode.SetIndex(myGrid.GetNodeIndexFromWroldPosition(StartPoint.position));
        EndingNode.SetIndex(myGrid.GetNodeIndexFromWroldPosition(DestinationPoint.position));

        DrawEndingNodes();
        //nodedraw.DrawNodes(pathnodes, nodedraw.SpawnObject, myGrid.GetTileSize(), Color.red);
        FindPath(StartPoint.position, DestinationPoint.position);
    }

    void DrawEndingNodes()
    {
        NodesToDraw.Clear();
        NodesToDraw.Add(startingNode.GetIndex());
        NodesToDraw.Add(EndingNode.GetIndex());
    }

    public void FindPath(Vector3 startingPosition, Vector3 endPosition)
    {

        startingNode.SetIndex(myGrid.GetNodeIndexFromWroldPosition(endPosition));
        EndingNode.SetIndex(myGrid.GetNodeIndexFromWroldPosition(startingPosition));
        List<GridTile> OpenNodes = new List<GridTile>();
        List<GridTile> ClosedNodes = new List<GridTile>();

        OpenNodes.Add(startingNode);

        while (OpenNodes.Count > 0)
        {
            var currentNode = OpenNodes[0];

            for (int i = 0; i < OpenNodes.Count; i++)
            {
                if (OpenNodes[i].fcost < currentNode.fcost || OpenNodes[i].fcost == currentNode.fcost && OpenNodes[i].Hcost < currentNode.Hcost)
                {
                    currentNode = OpenNodes[i];
                }
            }
            OpenNodes.Remove(currentNode);
            ClosedNodes.Add(currentNode);

            if (currentNode.Equals(EndingNode))
            {
                Retracepath(startingNode, currentNode);
                return;
            }
            var neighbourlist = myGrid.GetNeighbourNodes(currentNode);

            //nodedraw.DrawNodes(neighbourlist.Select(x => (x.index)).ToList(), nodedraw.SpawnObject, myGrid.GetTileSize(), Color.black,"Neighbour");

            foreach (var neighbourTile in neighbourlist)
            {
                if (!neighbourTile.isWalkable || ClosedNodes.Contains(neighbourTile))
                {
                    continue;
                }

                int newCost = currentNode.Gcost + GetDiffenceBetweenNode(currentNode, neighbourTile);
                if (newCost < neighbourTile.Gcost || !OpenNodes.Contains(neighbourTile))
                {
                    neighbourTile.Gcost = newCost;
                    neighbourTile.Hcost = GetDiffenceBetweenNode(neighbourTile, EndingNode);
                    neighbourTile.parent = currentNode;


                    if (!OpenNodes.Contains(neighbourTile))
                    {
                        OpenNodes.Add(neighbourTile);
                    }
                }
            }
        }
    }

    void Retracepath(GridTile start, GridTile end)
    {
        List<GridTile> path = new List<GridTile>();
        var startit = end;
        while (!startit.Equals(start))
        {
            path.Add(startit);
            startit = startit.parent;
        }
        path.Reverse();
    }

    int GetDiffenceBetweenNode(GridTile firstTile, GridTile secondTile)
    {
        int rowDiff = Mathf.Abs(firstTile.GetIndex().Item1 - secondTile.GetIndex().Item1) * 10;
        int columnDiff = Mathf.Abs(firstTile.GetIndex().Item2 - secondTile.GetIndex().Item2) * 14;

        return rowDiff + columnDiff;
    }

}