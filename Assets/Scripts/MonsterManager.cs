using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    private Maze maze;
    private Player player;
    public Spider spiderPrefab;
    private List<Spider> spiders;

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
        spiders.Add(Instantiate(spiderPrefab) as Spider);
        spiders.Add(Instantiate(spiderPrefab) as Spider);
        spiders.Add(Instantiate(spiderPrefab) as Spider);

        foreach (Spider s in spiders)
        {
            s.SetManager(this);
            s.SetLocation(maze.GetCell(maze.RandomCoordinates));
        }

        snakes.Add(Instantiate(snakePrefab) as Snake);
        snakes.Add(Instantiate(snakePrefab) as Snake);
        snakes.Add(Instantiate(snakePrefab) as Snake);

        foreach (Snake s in snakes)
        {
            s.SetManager(this);
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
        //Debug.Log("random coordonneesssss : " + coord);
        return maze.GetCell(coord);
    }

    public List<MazeCell> Path(MazeCell currentCell, MazeCell targetCell)
    {
        return dijkstra.Path(currentCell, targetCell);
    }
}
