using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze
{
    public MazeGeneratorCell[,] cells;
    public Vector2Int finishPosition;
}

public class MazeGeneratorCell
{
    public int X;
    public int Y;

    public bool WallLeft = true;
    public bool WallBottom = true;
    public bool WallRight = false;
    public bool WallFront = false;
    public bool Floor = true;

    public bool Visited = false;
    public int Distance;
}
