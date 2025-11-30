using System.Collections.Generic;

public enum Types 
{
    Menu,
    Apple,
    Maze,
    Wolf
}

public static class BaseScenesType
{
    public static readonly Dictionary<Types, string> Type = new Dictionary<Types, string>()
    {
        { Types.Menu, "MenuScene" },
        { Types.Apple, "AppleScene" },
        { Types.Maze, "MazeScene" },
        { Types.Wolf, "WolfScene" }
    };
}