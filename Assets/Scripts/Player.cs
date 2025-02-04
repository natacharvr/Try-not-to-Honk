using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX;
    private float movementY;
    private float speed;
    private MazeCell currentCell;
    private Maze mazeInstance;
    public GameUI gameUI;

    [SerializeField] private AudioClip[] damageSounds;
    [SerializeField] private AudioClip deathSound;

    //private new AudioSource audio;
    //private AudioSource audioWin;

    //private bool IsCoroutineActive = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //count = 0;
        //AudioSource[] audios = GetComponents<AudioSource>();
        //audio = audios[0];
        //audioWin = audios[1];
        //LoadHighScore();
        //startTime = Time.time;
        //SetTimeText();
        //SetCountText();
        //SetHighScoreText();
        //endPanel.SetActive(false);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    //void SetTimeText()
    //{
    //    scoreText.text = "Time :" + score.ToString("F1");
    //}

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        movement = transform.TransformDirection(movement);
        //Debug.Log("Player movement = " + movement * speed);
        rb.AddForce(movement * speed);
    }

    void Update()
    {
        IntVector2 size = mazeInstance.size;
        int xPos = Mathf.FloorToInt(transform.position.x + size.x * 0.5f);
        int zPos = Mathf.FloorToInt(transform.position.z + size.z * 0.5f);
        SetCell(mazeInstance.GetCell(new IntVector2(xPos, zPos)));
    }



    //public void Replay()
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //}

    public void SetCell(MazeCell cell)
    {
        if (currentCell != null)
        {
            currentCell.OnPlayerExited();
        }

        
        currentCell = cell;
        cell.OnPlayerEntered();
    }

    public void SetLocation(MazeCell cell)
    {
        transform.position = cell.transform.localPosition;
        if (currentCell == null) {
            SetCell(cell);  
        }
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetMaze(Maze maze)
    {
        mazeInstance = maze;
    }
    //public void Back()
    //{
    //    SceneManager.LoadScene("MainMenu");
    //}
    public MazeCell GetCurrentCell()
    {
        return currentCell;
    }

    public void Hit(int damage)
    {
        gameUI.Hit(damage);
        SoundFXManager.instance.PlayRandomSoundFXClip(damageSounds, transform, 0.5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Monster")
        {
            Hit(10);
        }
    }
}
