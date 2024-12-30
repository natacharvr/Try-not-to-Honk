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

    public void SetManager(MonsterManager manager)
    {
        this.manager = manager;
    }

    public void SetLocation(MazeCell cell)
    {
        transform.position = cell.transform.localPosition;
        currentCell = cell;
        BeginGame();
    }

    private void BeginGame()
    {
        StartCoroutine(Move());
    }

    private void AskRandomDestination()
    {
        targetCell = manager.RandomDestination();
        //Debug.Log(targetCell);
    }

    private IEnumerator Move()
    {
        WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
        while (true)
        {
            if (path != null && path.Count > 1)
            {
                currentCell = path[1];
                path.RemoveAt(0);
                transform.localPosition = currentCell.transform.localPosition;
                transform.rotation = currentCell.transform.localRotation;
            }
            else
            {
                AskRandomDestination();
                path = manager.Path(currentCell, targetCell);
            }
            yield return delay;
        }
    }
}
