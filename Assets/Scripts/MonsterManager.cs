using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private Maze maze;
    private Player player;
    public Spider spiderPrefab;
    private List<Spider> spiders;
    public int SpiderNb;
    public int SnakeNb;

    public Snake snakePrefab;
    private List<Snake> snakes;

    private Dijkstra dijkstra;

    public void SetMaze(Maze maze)
    {
        this.maze = maze;
        dijkstra = new Dijkstra(maze);
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    void Start()
    {
        spiders = new List<Spider>();
        snakes = new List<Snake>();
    }

    public void BeginGame()
    {
        // Monsters setup

        //Spiders
        for (int i = 0; i < SpiderNb; i++)
        {
            Spider s = Instantiate(spiderPrefab) as Spider;
            spiders.Add(s);
            s.SetManager(this);
            s.SetMaze(maze);
            s.SetLocation(maze.GetCell(maze.RandomCoordinates));
        }

        // Snakes
        for (int i = 0; i < SpiderNb; i++)
        {
            Snake s = Instantiate(snakePrefab) as Snake;
            snakes.Add(s);
            s.SetManager(this);
            s.SetMaze(maze);
            s.SetLocation(maze.GetCell(maze.RandomCoordinates));
        }
    }

    public void EndGame()
    {
        foreach (Spider s in spiders)
        {
            Destroy(s.gameObject);
        }
        spiders.Clear();

        foreach (Snake s in snakes)
        {
            Destroy(s.gameObject);
        }
        snakes.Clear();
    }

    public MazeCell RandomDestination()
    {
        IntVector2 coord = maze.RandomCoordinates;
        return maze.GetCell(coord);
    }

    public MazeCell RandomRoomDestination(MazeRoom room)
    {
        return room.RandomCell();
    }

    public List<MazeCell> Path(MazeCell currentCell, MazeCell targetCell)
    {
        return dijkstra.Path(currentCell, targetCell);
    }
}
