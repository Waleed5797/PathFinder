using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrid : MonoBehaviour
{
    Grid mytestGrid;

    void Start()
    {
        mytestGrid = new Grid(100,100,Vector2.one* 0.5f);
        mytestGrid.DrawGrid();
    }
}