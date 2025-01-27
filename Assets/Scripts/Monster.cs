using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    private MonsterManager manager;
    private MazeCell currentCell;
    private MazeCell targetCell;
    private List<MazeCell> path;
    public float generationStepDelay;
    private Rigidbody rb;
    private Maze mazeInstance;
    private float speed;
    private float threshold = 0.1f;
    private MazeRoom room;
    private bool playerSpotted;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerSpotted = false;
        //Debug.Log("rigidbody" + rb);
    }

    public void SetManager(MonsterManager manager)
    {
        this.manager = manager;
    }

    public void SetLocation(MazeCell cell)
    {
        transform.position = cell.transform.localPosition;
        currentCell = cell;
        cell.room.Add(this);
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }


    private void AskRandomDestination()
    {
        //targetCell = manager.RandomDestination();
        targetCell = manager.RandomRoomDestination(currentCell.room);
    }

    private IEnumerator SpotPlayer()
    {
        // Player detection (if player in field of view)
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Labyrinth", "Player");
        // Does the ray intersect any objects excluding the player layer
        if ((Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask)) && hit.collider.tag == "Player")
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Player spotted");
            List<MazeCell> tempPath = manager.PathToPlayer(currentCell);
            if (tempPath != null)
            { 
                path = tempPath; 
                playerSpotted = true;
                yield return new WaitForSeconds(3);
                playerSpotted = false;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //Debug.Log("Did not Hit");
        }

    }

    void FixedUpdate()
    {

        if (!playerSpotted) {
            StartCoroutine(SpotPlayer());
        }
        
        while (path == null || path.Count <= 1)
        {
            if (playerSpotted)
            {
                path = manager.PathToPlayer(currentCell);
                if (path == null)
                {
                    playerSpotted = false;
                }
            }
            else
            {
                //Debug.Log("while monster update");
                AskRandomDestination();
                path = manager.Path(currentCell, targetCell);
            }
        }
        
        Vector3 direction = path[1].transform.position - transform.position;
        direction.Normalize();
        if (direction.magnitude > 0.1f) // Avoid rotating for very small movements
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 4); ;
        }

        //Vector3 movement = transform.TransformDirection(direction);
        rb.AddForce(direction * speed);
    }

    public void SetCell(MazeCell cell)
    {
        if (cell.coordinates == path[1].coordinates)
        {
            //Debug.Log("Arrived at " + cell.coordinates);
            path.RemoveAt(0);
        }
        currentCell = cell;
    }

    public void SetMaze(Maze maze)
    {
        mazeInstance = maze;
    }   

    void Update()
    {
        if ((path != null) && (path.Count > 1) && (Distance(transform.position, path[1].transform.position) < threshold))
        {
            IntVector2 size = mazeInstance.size;
            int xPos = Mathf.FloorToInt(transform.position.x + size.x * 0.5f);
            int zPos = Mathf.FloorToInt(transform.position.z + size.z * 0.5f);
            SetCell(mazeInstance.GetCell(new IntVector2(xPos, zPos)));
        }
    }

    float Distance(Vector3 a, Vector3 b)
    {
        float dx = a.x - b.x;
        float dz = a.z - b.z;
        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}
