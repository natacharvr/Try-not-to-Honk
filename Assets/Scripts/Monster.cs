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
    private float perception = 10;
    private float chaseTime = 2;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerSpotted = false;
    }

    public void SetPerception(float perception)
    {
        this.perception = perception;
    }
     public void SetChaseTime(float chaseTime)
    {
        this.chaseTime = chaseTime;
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
        targetCell = manager.RandomRoomDestination(currentCell.room);
    }

    private bool isSpotting = false;

    private IEnumerator SpotPlayer()
    {
        if (isSpotting) yield break;  // Prevent multiple coroutine instances
        isSpotting = true;

        float visionAngle = 45f; // Adjust the vision cone angle
        int rayCount = 5; // Number of rays in the cone
        float stepAngle = visionAngle / (rayCount - 1);

        bool playerSeen = false;
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Labyrinth", "Player");

        for (int i = 0; i < rayCount; i++)
        {
            float angle = -visionAngle / 2 + i * stepAngle;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

            if (Physics.Raycast(transform.position, direction, out hit, perception, layerMask) && hit.collider.CompareTag("Player"))
            {
                Debug.DrawRay(transform.position, direction * hit.distance, Color.yellow);
                Debug.Log("Player spotted!");
                playerSeen = true;
                break; // Stop checking after spotting the player
            }
            else
            {
                Debug.DrawRay(transform.position, direction * perception, Color.white);
            }
        }

        if (playerSeen)
        {
            List<MazeCell> tempPath = manager.PathToPlayer(currentCell);
            if (tempPath != null)
            {
                path = tempPath;
                playerSpotted = true;
                yield return new WaitForSeconds(chaseTime);  // Wait before checking again
            }
        } else
        {
            isSpotting = false;
            yield break;
        }


        // **Only reset playerSpotted if we lose sight for multiple checks**
        bool lostSight = true;
        for (int i = 0; i < 3; i++)  // Check 3 times before giving up
        {
            yield return new WaitForSeconds(1);
            for (int j = 0; j < rayCount; j++)
            {
                float angle = -visionAngle / 2 + j * stepAngle;
                Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

                if (Physics.Raycast(transform.position, direction, out hit, perception, layerMask) && hit.collider.CompareTag("Player"))
                {
                    lostSight = false;
                    break;
                }
            }
            if (!lostSight) break;
        }

        if (lostSight)
        {
            Debug.Log("Lost sight of player");
            playerSpotted = false;
        }

        isSpotting = false;
    }


    void FixedUpdate()
    {
        if (!playerSpotted) {
            StartCoroutine(SpotPlayer());
        }
        int attempts = 0;

        while( (path == null || path.Count <= 1) && attempts < 10)
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
            attempts++;
        }

        if (path == null || path.Count == 0)
        {
            rb.velocity = Vector3.zero;  // Stop movement
            return;
        }


        Vector3 direction;
        if (path.Count > 1)
        {
             direction = path[1].transform.position - transform.position;
        } else if (path.Count == 1)
        {
            direction = manager.PlayerPos() - transform.position;
        }
        else
        {
            direction = Vector3.zero;
        }
        direction.Normalize();
        //if (direction.magnitude > 0.1f) // Avoid rotating for very small movements
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(direction);
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 4);
        //}
        direction.y = 0;

        if (direction.magnitude > 0.1f) // Avoid rotating for very small movements
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 4);
        }

        // Ensure the monster stays upright
        Vector3 fixedRotation = transform.eulerAngles;
        fixedRotation.x = 0;
        fixedRotation.z = 0;
        transform.eulerAngles = fixedRotation;
        //Vector3 movement = transform.TransformDirection(direction);
        rb.AddForce(direction * speed);
        //rb.velocity = direction * speed;
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
