using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MazeGeneratorNew
{
    public int Width = 6;
    public int Height = 6;

    public MazeGeneratorCell[,] GenerateMaze()
    {
        Width = Random.Range(3, 7);
        Height = Random.Range(3, 7);
        MazeGeneratorCell[,] cells = new MazeGeneratorCell[Width, Height];

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                cells[x, y] = new MazeGeneratorCell { X = x, Y = y };
            }
        }

        for (int x = 0; x < cells.GetLength(0); x++)
        {
            cells[x, Height - 1].WallFront = true;
        }

        for (int y = 0; y < cells.GetLength(1); y++)
        {
           cells[Width - 1, y].WallRight = true;
        }

        RemoveAlgorithm(cells);


        return cells;
    }

    private void RemoveAlgorithm(MazeGeneratorCell[,] maze)
    {
        MazeGeneratorCell current = maze[0, 0];
        current.Visited = true;
        current.Distance = 0;

        Stack<MazeGeneratorCell> stack = new Stack<MazeGeneratorCell>();
        do
        {
            List<MazeGeneratorCell> unvisitedNeighbours = new List<MazeGeneratorCell>();

            int x = current.X;
            int y = current.Y;

            if (x > 0 && !maze[x - 1, y].Visited) unvisitedNeighbours.Add(maze[x - 1, y]);
            if (y > 0 && !maze[x, y - 1].Visited) unvisitedNeighbours.Add(maze[x, y - 1]);
            if (x < Width - 1 && !maze[x + 1, y].Visited) unvisitedNeighbours.Add(maze[x + 1, y]);
            if (y < Height - 1 && !maze[x, y + 1].Visited) unvisitedNeighbours.Add(maze[x, y + 1]);

            if (unvisitedNeighbours.Count > 0)
            {
                MazeGeneratorCell chosen = unvisitedNeighbours[UnityEngine.Random.Range(0, unvisitedNeighbours.Count)];
                RemoveWall(current, chosen);

                chosen.Visited = true;
                stack.Push(chosen);
                chosen.Distance = stack.Count;
                current = chosen;
            }
            else
            {
                current = stack.Pop();
            }

        } while (stack.Count > 0);
    }

    private void RemoveWall(MazeGeneratorCell current, MazeGeneratorCell chosen)
    {
        if (current.X == chosen.X)
        {
            if (current.Y > chosen.Y) current.WallBottom = false;
            else chosen.WallBottom = false;
        }
        else
        {
            if (current.X > chosen.X) current.WallLeft = false;
            else chosen.WallLeft = false;
        }
    }

    public MazeGeneratorCell PlaceMazeExit(MazeGeneratorCell[,] maze)
    {
        MazeGeneratorCell furthest = maze[0, 0];

        for (int x = 0; x < maze.GetLength(0); x++)
        {
            for (int y = 0; y < maze.GetLength(1); y++)
            {
                if (maze[x, y].Distance > furthest.Distance)
                {
                    furthest = maze[x, y];
                }
            }
        }

        return furthest;
    }
}
