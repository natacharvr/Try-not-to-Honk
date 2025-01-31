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

    // user scores 
    private string userName;
    private int heartSpiderMax = 0;
    private int heartSerpentMax = 0;
    private int heartStressMax = 0;


    private void Start()
    {
        monsterManager.SetSpeed(monsterSpeed);
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
        // help player
        // TODO adjust
        if (stress.StressVariationAbsolute() < 0)
        {
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
            // slow down player
            playerSpeed = Mathf.Max(playerSpeed - 0.1f * stress.StressVariationAbsolute(), 1);
            player.SetSpeed(playerSpeed);
            // accelerate monsters
            monsterSpeed = Mathf.Min(15, monsterSpeed + 0.1f * stress.StressVariationAbsolute());
            monsterManager.SetSpeed(monsterSpeed);

        }
        yield return new WaitForSeconds(10);
    }

    public List<MazeCell> GetPath(MazeCell from, MazeCell to)
    {
        return dijkstra.Path(from, to);
    }

    public void SaveStressUser()
    {
        PlayerPrefs.SetInt(userName+"SpiderStress", heartSpiderMax);
        PlayerPrefs.SetInt(userName+"SerpentStress", heartSpiderMax);
        PlayerPrefs.SetInt(userName+"Stress", heartStressMax);
    }

    public void LoadStressUser(string user)
    {
        userName = user;
        heartSpiderMax = PlayerPrefs.GetInt(user + "SpiderStress", 0);
        heartSerpentMax = PlayerPrefs.GetInt(user + "SerpentStress", 0);
        heartStressMax = PlayerPrefs.GetInt(user + "Stress", 0);
    }

}