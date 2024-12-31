using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    public IntVector2 size;
    public MazeCell cellPrefab;
    private MazeCell[,] cells;
    private MazeCell startCell;
    public IntVector2 startCoordinates
    {
        get
        {
            return startCell.coordinates;
        }
    }
    private MazeCell endCell;
    public MazePassage passagePrefab;
    public MazeWall wallPrefab;
    public MazeDoor doorPrefab;

    [Range(0f, 1f)]
    public float doorProbability;
    public MazeRoomSettings[] roomSettings;
    public MazeRoomSettings extremities;

    public MazeCell GetDestination()
    {
        return endCell;
    }
    public MazeCell GetOrigin()
    {
        return startCell;
    }
    //public void Generate()
    //{
    //    cells = new MazeCell[sizeX, sizeZ];
    //    for (int x = 0; x < sizeX; x++)
    //    {
    //        for (int z = 0; z < sizeZ; z++)
    //        {
    //            CreateCell(x, z);
    //        }
    //    }
    //}

    public float generationStepDelay;

    public IntVector2 RandomCoordinates
    {
        get
        {
            return new IntVector2(Random.Range(0, size.x), Random.Range(0, size.z));
        }
    }

    public bool ContainsCoordinates(IntVector2 coordinate)
    {
        return coordinate.x >= 0 && coordinate.x < size.x && coordinate.z >= 0 && coordinate.z < size.z;
    }

    public MazeCell GetCell(IntVector2 coordinates)
    { 
        if (startCell != null && endCell != null)
        {
            if (coordinates == startCell.coordinates)
            {
                return startCell;
            }
            else if (coordinates == endCell.coordinates)
            {
                return endCell;
            }
            else if (!ContainsCoordinates(coordinates))
            {
                return null;
            }
        }

        return cells[coordinates.x, coordinates.z];
    }

    public MazeCell[,] GetCells()
    {
        return cells;
    }

    public IEnumerator Generate()
    {
        WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
        cells = new MazeCell[size.x, size.z];
        List<MazeCell> activeCells = new List<MazeCell>();
        DoFirstGenerationStep(activeCells);
        while (activeCells.Count > 0)
        {
            yield return delay;
            DoNextGenerationStep(activeCells);
        }
        CreateStartAndEnd();

        for (int i = 0; i < rooms.Count; i++)
        {
            //rooms[i].Hide();
        }
        //IntVector2 coordinates = RandomCoordinates;
        //while (ContainsCoordinates(coordinates) && GetCell(coordinates) == null)
        //{
        //    yield return delay;
        //    CreateCell(coordinates);
        //    coordinates += MazeDirections.RandomValue.ToIntVector2();
        //}

    }

    private void DoFirstGenerationStep(List<MazeCell> activeCells)
    {
        MazeCell newCell = CreateCell(RandomCoordinates);
        newCell.Initialize(CreateRoom(-1));
        activeCells.Add(newCell);
    }

    private void DoNextGenerationStep(List<MazeCell> activeCells)
    {
        int currentIndex = activeCells.Count - 1;
        MazeCell currentCell = activeCells[currentIndex];
        if (currentCell.IsFullyInitialized)
        {
            activeCells.RemoveAt(currentIndex);
            return;
        }
        MazeDirection direction = currentCell.RandomUninitializedDirection;
        IntVector2 coordinates = currentCell.coordinates + direction.ToIntVector2();
        if (ContainsCoordinates(coordinates) && GetCell(coordinates) == null)
        {
            MazeCell neighbor = GetCell(coordinates);
            if (neighbor == null)
            {
                neighbor = CreateCell(coordinates);
                CreatePassage(currentCell, neighbor, direction);
                activeCells.Add(neighbor);
            }
            else
            {
                CreateWall(currentCell, neighbor, direction);
            }
        }
        else
        {
            CreateWall(currentCell, null, direction);
        }
    }

    private MazeCell CreateCell(IntVector2 coordinates)
    {
        MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
        cells[coordinates.x, coordinates.z] = newCell;
        newCell.coordinates = coordinates;
        newCell.name = "Maze Cell " + coordinates.x + ", " + coordinates.z;
        newCell.transform.parent = transform;
        newCell.transform.localPosition = new Vector3(coordinates.x - size.x * 0.5f + 0.5f, 0f, coordinates.z - size.z * 0.5f + 0.5f);
        return newCell;
    }

    private void CreatePassage(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazePassage prefab = Random.value < doorProbability ? doorPrefab : passagePrefab;
        MazePassage passage = Instantiate(prefab) as MazePassage;
        passage.Initialize(cell, otherCell, direction);
        passage = Instantiate(prefab) as MazePassage;
        if (passage is MazeDoor)
        {
            otherCell.Initialize(CreateRoom(cell.room.settingsIndex));
        }
        else
        {
            otherCell.Initialize(cell.room);
        }
        passage.Initialize(otherCell, cell, direction.GetOpposite());
    }

    private void CreateWall(MazeCell cell, MazeCell otherCell, MazeDirection direction)
    {
        MazeWall wall = Instantiate(wallPrefab) as MazeWall;
        wall.Initialize(cell, otherCell, direction);
        if (otherCell != null)
        {
            wall = Instantiate(wallPrefab) as MazeWall;
            wall.Initialize(otherCell, cell, direction.GetOpposite());
        }
    }

    private List<MazeRoom> rooms = new List<MazeRoom>();

    private MazeRoom CreateRoom(int indexToExclude)
    {
        MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
        newRoom.settingsIndex = Random.Range(0, roomSettings.Length);
        if (newRoom.settingsIndex == indexToExclude)
        {
            newRoom.settingsIndex = (newRoom.settingsIndex + 1) % roomSettings.Length;
        }
        newRoom.settings = roomSettings[newRoom.settingsIndex];
        rooms.Add(newRoom);
        return newRoom;
    }

    private void CreateStartAndEnd()
    {
        IntVector2 start = new IntVector2(Random.Range(0, size.x), -1);
        IntVector2 end = new IntVector2(Random.Range(0, size.x), size.z);

        startCell = Instantiate(cellPrefab) as MazeCell;
        startCell.coordinates = start;
        startCell.name = "Start Cell " + start.x + ", " + start.z;
        startCell.transform.parent = transform;
        startCell.transform.localPosition = new Vector3(start.x - size.x * 0.5f + 0.5f, 0f, start.z - size.z * 0.5f + 0.5f);

        endCell = Instantiate(cellPrefab) as MazeCell;
        endCell.coordinates = end;
        endCell.name = "End Cell " + end.x + ", " + end.z;
        endCell.transform.parent = transform;
        endCell.transform.localPosition = new Vector3(end.x - size.x * 0.5f + 0.5f, 0f, end.z - size.z * 0.5f + 0.5f);

        MazeCell currentCell;
        IntVector2 coordinates;
        MazeCell neighbor;

        MazeRoom newRoom = ScriptableObject.CreateInstance<MazeRoom>();
        newRoom.settingsIndex = -1;

        newRoom.settings = extremities;
        rooms.Add(newRoom);

        startCell.Initialize(newRoom);
        endCell.Initialize(newRoom);

        for (MazeDirection direction = MazeDirection.North; direction <= MazeDirection.West; direction++)
        { 
            currentCell = startCell;
            coordinates = currentCell.coordinates + direction.ToIntVector2();
            neighbor = GetCell(coordinates);
            if (neighbor == null)
            {
                CreateWall(currentCell, neighbor, direction);
            }
            else
            {
                Destroy(neighbor.GetEdge(direction.GetOpposite()).gameObject);
                MazePassage passage = Instantiate(doorPrefab) as MazePassage;
                passage.Initialize(startCell, neighbor, direction); 
                passage = Instantiate(doorPrefab) as MazePassage;
                passage.Initialize(neighbor, startCell, direction.GetOpposite());

            }

            currentCell = endCell;
            coordinates = currentCell.coordinates + direction.ToIntVector2();
            neighbor = GetCell(coordinates);
            if (neighbor == null)
            {
                CreateWall(currentCell, neighbor, direction);
            }
            else
            {
                Destroy(neighbor.GetEdge(direction.GetOpposite()).gameObject);
                MazePassage passage = Instantiate(doorPrefab) as MazePassage;
                passage.Initialize(endCell, neighbor, direction);
                passage = Instantiate(doorPrefab) as MazePassage;
                passage.Initialize(neighbor, endCell, direction.GetOpposite());
            }

        }
    }
    
}
