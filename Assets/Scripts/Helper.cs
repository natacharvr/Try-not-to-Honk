using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour
{
    private Maze maze;
    private Player player;
    private Dijkstra dijkstra;
    // Stress stresslevel;
    public Material floorHelp;

    public void SetMaze(Maze maze)
    {
        this.maze = maze;
        dijkstra = new Dijkstra(maze);
    }

    public void SetPlayer(Player player)
    {
        //Debug.Log("In helper : " + player);
        this.player = player;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            // if stress is low;
            MazeCell origin = player.GetCurrentCell();
            MazeCell destination = maze.GetDestination();

            List<MazeCell> path = dijkstra.Path(origin, destination);
            for (int i = 0; i < path.Count; i++)
            {
                path[i].transform.GetChild(0).GetComponent<Renderer>().material = floorHelp;
            }
        }
    }
}