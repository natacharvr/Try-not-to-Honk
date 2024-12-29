using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Maze mazePrefab;
    private Maze mazeInstance;
    public Player playerPrefab;
    public MouseLookAround playerCamera;
    public Spider spiderPrefab;
    public Helper helper;

    private Player playerInstance;
    private MouseLookAround playerCameraInstance;
    private List<Spider> spiders;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BeginGame());
        spiders = new List<Spider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }
    }

    private IEnumerator BeginGame()
    {
        Debug.Log("Game Started");
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
        mazeInstance = Instantiate(mazePrefab) as Maze;
        yield return StartCoroutine(mazeInstance.Generate());
        playerInstance = Instantiate(playerPrefab) as Player;
        playerCameraInstance = Instantiate(playerCamera) as MouseLookAround;
        playerCameraInstance.SetPlayer(playerInstance);
        playerInstance.SetLocation(mazeInstance.GetCell(mazeInstance.startCoordinates));
        playerInstance.SetMaze(mazeInstance);

        Debug.Log("Player Instance " + playerInstance);
        helper.SetPlayer(playerInstance);
        helper.SetMaze(mazeInstance);

        // Monsters setup
        spiders.Add(Instantiate(spiderPrefab) as Spider);
        spiders.Add(Instantiate(spiderPrefab) as Spider);
        spiders.Add(Instantiate(spiderPrefab) as Spider);

        foreach (Spider s in spiders)
        {
            s.SetLocation(mazeInstance.GetCell(mazeInstance.RandomCoordinates));
        }


        Camera.main.clearFlags = CameraClearFlags.Depth;
        Camera.main.rect = new Rect(0f, 0f, 0.5f, 0.5f);

    }

    private void RestartGame()
    {
        Debug.Log("Game Restarted");
        StopAllCoroutines();
        Destroy(mazeInstance.gameObject);
        if (playerInstance != null)
        {
            Destroy(playerInstance.gameObject);
        }
        if (playerCameraInstance != null) {
            Destroy(playerCameraInstance.gameObject);
        }
        foreach (Spider s in spiders)
        {
            Destroy(s.gameObject);
        }
        spiders = new List<Spider>();
        StartCoroutine(BeginGame());
    }
}
