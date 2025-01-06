using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressManager: MonoBehaviour
{
    private Maze maze;
    private Player player;
    private Dijkstra dijkstra;
    [SerializeField] private Stress stress;
    private float heartRate;
    public Material floorHelp;
    public MonsterManager monsterManager;


    [SerializeField] private float spawnProbability = 0.1f;
    [SerializeField] private float spawnProbabilityThreshold = 0.8f;
    public float playerSpeed;
    [SerializeField] private float playerMinSpeed = 5;
    [SerializeField] private float playerMaxSpeed = 15;

    public float monsterSpeed;
    [SerializeField] private float monsterMinSpeed = 14;
    [SerializeField] private float monsterMaxSpeed = 25;


    private void Start()
    {
        monsterManager.SetSpeed(monsterSpeed);
        //heartRate = stress.GetHeartRate();
    }

    public void SetMaze(Maze maze)
    {
        this.maze = maze;
        dijkstra = new Dijkstra(maze);
    }

    public void SetPlayer(Player player)
    {        
        this.player = player;
        player.SetSpeed(playerSpeed);
    }

    void Update()
    {
        // if heart variation is negative
        // help player
        // TODO adjust
        if ((stress.StressVariation() < 0) && (Input.GetKeyDown(KeyCode.H)))
        {
            // accelerate player
            playerSpeed = playerSpeed - 0.1f * stress.StressVariation(); // With a minus because stress is negative
            // slow down monsters
            monsterSpeed = monsterSpeed + 0.1f * stress.StressVariation(); // With a plus because stress is negative

            // show help for path
            int nb_help = Mathf.Abs(Mathf.FloorToInt(stress.StressVariation()));
            MazeCell origin = player.GetCurrentCell();
            MazeCell destination = maze.GetDestination();

            List<MazeCell> path = dijkstra.Path(origin, destination);
            for (int i = 0; i < Mathf.Min(path.Count-1, nb_help); i++)
            {
                // TODO: change direction
                path[i].SetHelping(true, path[i].GetDirection(path[i+1]));
            }
        }


        // if heart variation is positive
        // stress player
        if ((stress.StressVariation() > 0) && (Input.GetKeyDown(KeyCode.G)))
        {
            // slow down player
            playerSpeed = playerSpeed - 0.1f * stress.StressVariation();
            // accelerate monsters
            monsterSpeed = monsterSpeed + 0.1f * stress.StressVariation();

        }

    }

    public List<MazeCell> GetPath(MazeCell from, MazeCell to)
    {
        return dijkstra.Path(from, to);
    }


}