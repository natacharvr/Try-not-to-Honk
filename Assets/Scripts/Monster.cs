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
        targetCell = manager.RandomDestination();
    }

    private void FixedUpdate()
    {
        while (path == null || path.Count <= 1)
        {
            AskRandomDestination();
            path = manager.Path(currentCell, targetCell);
        }
        MazeDirection direction = currentCell.GetPassageDirection(path[1]);
        Vector3 movement = new Vector3(direction.ToIntVector2().x, 0, direction.ToIntVector2().z);
        transform.rotation = Quaternion.Euler(direction.ToIntVector2().x, 0, direction.ToIntVector2().z); //direction.ToRotation();
        movement = transform.TransformDirection(movement);
        //Debug.Log("Moving to " + path[1].coordinates.x + "," + path[1].coordinates.z);
        //Debug.Log("Monster movement = " + movement*speed);
        rb.AddForce(movement * speed);
    }

    void Update()
    {
        IntVector2 size = mazeInstance.size;
        int xPos = Mathf.FloorToInt(transform.position.x + size.x * 0.5f);
        int zPos = Mathf.FloorToInt(transform.position.z + size.z * 0.5f);
        //Debug.Log("Current cell = " +xPos + "," + zPos);
        SetCell(mazeInstance.GetCell(new IntVector2(xPos, zPos)));
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
}
