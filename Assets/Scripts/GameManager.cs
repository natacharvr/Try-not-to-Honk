using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    // maze
    public Maze mazePrefab;
    private Maze mazeInstance;

    //player
    private string userName;

    public Player playerPrefab;
    public MouseLookAround playerCamera;
    private Player playerInstance;
    private MouseLookAround playerCameraInstance;

    //monsters
    public MonsterManager monsterManager;

    // stress manager
    public StressManager stressManager;

    // UI
    public MenuPanel menuPanel;
    public GameObject endPanelWin;
    public GameObject endPanelLoose;
    public ScoreBoard scoreBoard;
    public GameUI gameUI;
    public float complexity;
    private float MaxTime;

    // bitalino
    [SerializeField] private BitalinoScript bitalino;


    // Start is called before the first frame update
    void Start()
    {
        endPanelWin.SetActive(false);
        endPanelLoose.SetActive(false);
        gameUI.gameObject.SetActive(false);
        menuPanel.gameObject.SetActive(true);
        scoreBoard.gameObject.SetActive(false);
        StartCoroutine(bitalinoConnected());
    }

    private IEnumerator bitalinoConnected()
    {
        while (!bitalino.isAcquisitionStarted) // Use the condition directly
        {
            yield return new WaitForSeconds(1);
        }

        menuPanel.bitalinoConnected();
    }

    // Update is called once per frame
    void Update()
        {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    RestartGame();
        //}
    }

    private IEnumerator WinLoose()
    {
        bool stop = false;
        while (!stop)
        {
            if (playerInstance != null)
            {
                bool win = playerInstance.GetCurrentCell().coordinates == mazeInstance.GetDestination().coordinates;
                if (win)
                {
                    Win();
                    stop = true;
                }
                else if (gameUI.Loose())
                {
                    Loose();
                    stop = true;
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator BeginGame()
    {
        //Debug.Log("Game Started");
        userName = menuPanel.GetUsername();
        //Debug.Log("Welcome " + userName);

        //UI
        endPanelWin.SetActive(false);
        endPanelLoose.SetActive(false);
        menuPanel.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(false);
        scoreBoard.gameObject.SetActive(false);
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
        stressManager.SetPlayer(playerInstance);
        stressManager.SetMaze(mazeInstance);
        stressManager.BeginGame();

        scoreBoard.BeginGame(userName);

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
        //SoundFXManager.instance.PlayBackgroundMusic(0, 1.0f, true);
        SoundFXManager.instance.StartFadeInBackgroundMusic(0, 5, 1.0f);

        StartCoroutine(WinLoose());

    }

    public void Win()
    {
        endPanelWin.SetActive(true);
        stressManager.EndGame();
        playerInstance.gameObject.SetActive(false);
        playerCameraInstance.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(false);
        scoreBoard.SaveStressUser();
        SoundFXManager.instance.StopAllSoundFX();
        scoreBoard.gameObject.SetActive(true);
    }

    public void Loose()
    {
        endPanelLoose.SetActive(true);
        stressManager.EndGame();
        playerInstance.gameObject.SetActive(false);
        playerCameraInstance.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(false);
        scoreBoard.SaveStressUser();
        SoundFXManager.instance.StopAllSoundFX();
        scoreBoard.gameObject.SetActive(true);
    }

    public void StartGame()
    {
        StartCoroutine(BeginGame());
    }
    public void RestartGame()
    {
        //Debug.Log("Game Restarted");
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
        SoundFXManager.instance.StopAllSoundFX();

        StartCoroutine(BeginGame());
    }
    float CalculateTime()
    {
        return stressManager.GetPath(mazeInstance.GetOrigin(), mazeInstance.GetDestination()).Count;
    }

    public void QuitGame()
    {
        //Debug.Log("Game is exiting...");
        Application.Quit(); 
    }

    public void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
