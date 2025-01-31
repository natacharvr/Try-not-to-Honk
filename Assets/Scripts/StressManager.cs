using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressManager : MonoBehaviour
{
    private Maze maze;
    private Player player;
    private Dijkstra dijkstra;
    [SerializeField] private Stress stress;
    [SerializeField] private SoundFXManager soundFXManager;
    private float heartRate;
    public MonsterManager monsterManager;
    [SerializeField] private AudioClip[] laughSounds;
    private bool gameEnded = false;


    [SerializeField] private float spawnProbability = 0.1f;
    [SerializeField] private float spawnProbabilityThreshold = 0.8f;
    public float playerSpeed;
    [SerializeField] private float playerMinSpeed = 5;
    [SerializeField] private float playerMaxSpeed = 15;

    public float monsterSpeed;

    private bool isStormStarted = false;
    private bool isMusicStarted = false;

    public void BeginGame()
    {
        gameEnded = false;
        monsterManager.SetSpeed(monsterSpeed);
        StartCoroutine(StressInfluence());
    }

    public void EndGame()
    {
        gameEnded = true;
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
        isStormStarted = false;
        isMusicStarted = false;
        while (!gameEnded)
        {
            // if heart variation is negative
            Debug.Log("StressInfluence");
            // help player
            // TODO adjust
            if (stress.StressVariationAbsolute() < 0 && stress.StressVariationTendancy() < 5)
            {
                Debug.Log("StressInfluence: help player");
                if (isMusicStarted)
                {
                    soundFXManager.StartFadeOutBackgroundMusic(2, 5);
                    isMusicStarted = false;
                }
                else if (isStormStarted)
                {
                    soundFXManager.StartFadeOutBackgroundMusic(1, 5);
                    soundFXManager.StartFadeInBackgroundMusic(0, 5, 1.0f);
                    isStormStarted = false;
                } 

                // accelerate player
                playerSpeed = Mathf.Min(15, playerSpeed - 0.1f * stress.StressVariationAbsolute()); // With a minus because stress is negative
                player.SetSpeed(playerSpeed);
                // slow down monsters
                monsterSpeed = Mathf.Max(monsterSpeed + 0.1f * stress.StressVariationAbsolute(), 10); // With a plus because stress is negative
                monsterManager.SetSpeed(monsterSpeed);
                monsterManager.SetPerception(10 + 0.1f * stress.StressVariationAbsolute());
                monsterManager.SetChaseTime(2 + 0.1f * stress.StressVariationAbsolute());

                // show help for path
                int nb_help = Mathf.Abs(Mathf.FloorToInt(stress.StressVariationAbsolute()));
                MazeCell origin = player.GetCurrentCell();
                MazeCell destination = maze.GetDestination();

                List<MazeCell> path = dijkstra.Path(origin, destination);
                for (int i = 0; i < Mathf.Min(path.Count - 1, nb_help); i++)
                {
                    // TODO: change direction
                    path[i].SetHelping(true, path[i].GetDirection(path[i + 1]));
                }
            }


            // if heart variation is positive
            // stress player
            if (stress.StressVariationAbsolute() > 0 && stress.StressVariationTendancy() > 0)
            {
                Debug.Log("StressInfluence: stress player");
                if (!isStormStarted)
                {
                    Debug.Log("Storm started");
                    soundFXManager.StartFadeOutBackgroundMusic(0, 5);
                    soundFXManager.StartFadeInBackgroundMusic(1, 5, 0.5f);
                    isStormStarted = true;
                }
                else if (!isMusicStarted)
                {
                    Debug.Log("Music started");
                    soundFXManager.StartFadeInBackgroundMusic(2, 5, 1.0f);
                    isMusicStarted = true;
                }
                if (Random.value < 0.2)
                {
                    soundFXManager.PlayRandomSoundFXClip(laughSounds, player.transform);
                }
                // slow down player
                playerSpeed = Mathf.Max(playerSpeed - 0.1f * stress.StressVariationAbsolute(), 5);
                player.SetSpeed(playerSpeed);
                // accelerate monsters
                monsterSpeed = Mathf.Min(20, monsterSpeed + 0.1f * stress.StressVariationAbsolute());
                monsterManager.SetSpeed(monsterSpeed);
                monsterManager.SetPerception(10 + 0.1f * stress.StressVariationAbsolute());
                monsterManager.SetChaseTime(2 + 0.1f * stress.StressVariationAbsolute());

                // spawn monsters
                if (Random.value < spawnProbability)
                {
                    monsterManager.SpawnMonster();
                    Debug.Log("Monster spawned");
                }
            }
            yield return new WaitForSeconds(10);
        }
    }
    

    public List<MazeCell> GetPath(MazeCell from, MazeCell to)
    {
        return dijkstra.Path(from, to);
    }

}