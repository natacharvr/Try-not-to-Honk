using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class GameManager : MonoBehaviour
{
    // maze
    public Maze mazePrefab;
    private Maze mazeInstance;

    //player
    public string userName;

    public Player playerPrefab;
    public MouseLookAround playerCamera;
    private Player playerInstance;
    private MouseLookAround playerCameraInstance;

    //monsters
    public MonsterManager monsterManager;

    // stress manager
    public StressManager stressManager;

    // UI
    public GameObject endPanelWin;
    public GameObject endPanelLoose;
    public GameUI gameUI;
    public float complexity;
    private float MaxTime;

    // Audio
    [SerializeField] private AudioClip rainSound;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BeginGame());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RestartGame();
        }


        if (playerInstance != null)
        {
            bool win = playerInstance.GetCurrentCell().coordinates == mazeInstance.GetDestination().coordinates;
            if (win) {
                Win();
            }
            else if (gameUI.Loose())
            {
                Loose();
            }
        }
    }

    private IEnumerator BeginGame()
    {
        Debug.Log("Game Started");

        //UI
        endPanelWin.SetActive(false);
        endPanelLoose.SetActive(false);
        gameUI.gameObject.SetActive(false);
        //timerText.gameObject.SetActive(false);

        Camera.main.clearFlags = CameraClearFlags.Skybox;
        Camera.main.rect = new Rect(0f, 0f, 1f, 1f);

        //maze
        mazeInstance = Instantiate(mazePrefab) as Maze;
        yield return StartCoroutine(mazeInstance.Generate());

        //player
        playerInstance = Instantiate(playerPrefab) as Player;
        playerInstance.SetLocation(mazeInstance.GetCell(mazeInstance.startCoordinates));
        playerInstance.SetMaze(mazeInstance);
        playerInstance.gameUI = gameUI;
        //player camera
        playerCameraInstance = Instantiate(playerCamera) as MouseLookAround;
        playerCameraInstance.SetPlayer(playerInstance);

        //Debug.Log("Player Instance " + playerInstance);
        // stress manager
        stressManager.LoadStressUser(userName);
        stressManager.SetPlayer(playerInstance);
        stressManager.SetMaze(mazeInstance);

        // monsters
        monsterManager.SetMaze(mazeInstance);
        monsterManager.SetPlayer(playerInstance);
        monsterManager.BeginGame();

        Camera.main.clearFlags = CameraClearFlags.Depth;
        Camera.main.rect = new Rect(0f, 0f, 0.5f, 0.5f);

        // Timer
        MaxTime = CalculateTime();
        gameUI.Initialize(MaxTime * complexity);
        gameUI.gameObject.SetActive(true);

        // Audio 
        SoundFXManager.instance.PlaySoundFXClip(rainSound, transform, 0.5f);

    }

    public void Win()
    {
        endPanelWin.SetActive(true);
        playerInstance.gameObject.SetActive(false);
        playerCameraInstance.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(false);
        stressManager.SaveStressUser();
    }

    public void Loose()
    {
        endPanelLoose.SetActive(true);
        playerInstance.gameObject.SetActive(false);
        playerCameraInstance.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(false);
        stressManager.SaveStressUser();
    }

    public void RestartGame()
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
        monsterManager.EndGame();
        StartCoroutine(BeginGame());
    }
    float CalculateTime()
    {
        return stressManager.GetPath(mazeInstance.GetOrigin(), mazeInstance.GetDestination()).Count;
    }
}
