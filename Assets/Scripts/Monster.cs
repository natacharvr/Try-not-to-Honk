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
    public float speed = 10f;
    private float threshold = 0.1f;
    private MazeRoom room;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
    }


    private void AskRandomDestination()
    {
        //targetCell = manager.RandomDestination();
        targetCell = manager.RandomRoomDestination(currentCell.room);
    }

    void FixedUpdate()
    {
        while (path == null || path.Count <= 1)
        {
            AskRandomDestination();
            path = manager.Path(currentCell, targetCell);
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
        if ((path.Count > 1) && (Distance(transform.position, path[1].transform.position) < threshold))
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
}
