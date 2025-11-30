using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [HideInInspector] public int mazeX, mazeY;
    public void SetMazePosition(int x, int y)
    {
        mazeX = x;
        mazeY = y;
    }

    public GameObject Floor;
    public GameObject WallLeft;
    public GameObject WallRight;
    public GameObject WallBottom;
    public GameObject WallFront;

    [Header("Trap")]
    public GameObject TrapTrigger;
    public GameObject TrapVisual;
}
