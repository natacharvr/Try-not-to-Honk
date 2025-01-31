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
    }

    public void BeginGame()
    {
        StartCoroutine(StressInfluence());
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

    private IEnumerator StressInfluence()
    {
        // if heart variation is negative
        Debug.Log("StressInfluence");   
        // help player
        // TODO adjust
        if (stress.StressVariationAbsolute() < 0)
        {
            Debug.Log("StressInfluence: help player");  
            // accelerate player
            playerSpeed = Mathf.Min(15, playerSpeed - 0.1f * stress.StressVariationAbsolute()); // With a minus because stress is negative
            player.SetSpeed(playerSpeed);
            // slow down monsters
            monsterSpeed = Mathf.Max(monsterSpeed + 0.1f * stress.StressVariationAbsolute(), 2); // With a plus because stress is negative
            monsterManager.SetSpeed(monsterSpeed);

            // show help for path
            int nb_help = Mathf.Abs(Mathf.FloorToInt(stress.StressVariationAbsolute()));
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
        if (stress.StressVariationAbsolute() > 0)
        {
            Debug.Log("StressInfluence: stress player");
            // slow down player
            playerSpeed = Mathf.Max(playerSpeed - 0.1f * stress.StressVariationAbsolute(), 1);
            player.SetSpeed(playerSpeed);
            // accelerate monsters
            monsterSpeed = Mathf.Min(15, monsterSpeed + 0.1f * stress.StressVariationAbsolute());
            monsterManager.SetSpeed(monsterSpeed);

            // spawn monsters
            if (Random.value < spawnProbability)
            {
                monsterManager.SpawnMonster();
                Debug.Log("Monster spawned");
            }
        }
        yield return new WaitForSeconds(5);
    }

    public List<MazeCell> GetPath(MazeCell from, MazeCell to)
    {
        return dijkstra.Path(from, to);
    }

}