using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstra
{
    Maze maze;
    MazeCell origin, destination;
    List<Node> nodes;

    public Dijkstra(Maze m)
    {
        //Debug.Log("Dijkstra Init");
        maze = m;
        nodes = new List<Node>();
        MazeCell[,] cellsMatrix = maze.GetCells();

        List<MazeCell> cells = new List<MazeCell>();
        foreach (var value in cellsMatrix)
        {
            cells.Add(value);
        }

        cells.Add(maze.GetDestination());
        cells.Add(maze.GetOrigin());
        MazeCell currentCell;
        Node currentNode;

        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i] != null)
            {
                currentCell = cells[i];
                currentNode = new Node(currentCell);
                for (MazeDirection direction = MazeDirection.North; direction <= MazeDirection.West; direction++)
                {
                    if ((currentCell.GetEdge(direction) != null) && currentCell.GetEdge(direction) is MazePassage)
                    {
                        if (nodes.Exists(n => n.GetCell().coordinates == currentCell.GetEdge(direction).otherCell.coordinates))
                        {
                            Node neighbor = nodes.Find(n => n.GetCell().coordinates == currentCell.GetEdge(direction).otherCell.coordinates);
                            //Debug.Log("neighbor : " +neighbor.GetCell().coordinates.x);
                            //Debug.Log("currentNode : " + currentNode.GetCell().coordinates.x);

                            currentNode.AddNeighbor(neighbor);
                            neighbor.AddNeighbor(currentNode);
                        }
                    }
                }
                nodes.Add(currentNode);
            }
        
        }

    }

    private void CalculatePath()
    {
        Node start = nodes.Find(n => n.GetCell().coordinates == origin.coordinates);
        Node end = nodes.Find(n => n.GetCell().coordinates == destination.coordinates);

        start.SetDistance(0);

        foreach (Node neighbor in start.GetNeighbors())
        {
            // Call recursive method
            CalculateDistance(start, neighbor, end);
        }

    }

    private void CalculateDistance(Node current, Node neighbor, Node end)
    {
        if (neighbor.GetDistance() > current.GetDistance() + 1)
        {
            neighbor.SetDistance(current.GetDistance() + 1);
            neighbor.SetPrevious(current);
            if (neighbor == end)
            {
                return;
            }
            foreach (Node n in neighbor.GetNeighbors())
            {
                CalculateDistance(neighbor, n, end);
            }
        }
    }

    private List<MazeCell> GetPath()
    {
        List<MazeCell> path = new List<MazeCell>();
        Node current = nodes.Find(n => n.GetCell().coordinates == destination.coordinates);
        while (current != null)
        {
            path.Add(current.GetCell());
            current = current.GetPrevious();
        }
        path.Reverse();
        return path;
    }

    private void Reset()
    {
        foreach (Node n in nodes)
        {
            n.SetDistance(int.MaxValue);
            n.SetPrevious(null);
        }
    }

    public List<MazeCell> Path(MazeCell origin, MazeCell destination)
    {
        this.origin = origin;
        this.destination = destination;
        //Debug.Log("Path from " + origin.coordinates + " to " + destination.coordinates);
        CalculatePath();
        List<MazeCell> path = GetPath();
        Reset();
        return path;
    }

    public class Node
    {
        private List<Node> neighbors;
        private MazeCell cell;
        private int distance;
        private Node previous;

        public Node(MazeCell cell)
        {
            neighbors = new List<Node>();
            this.cell = cell;
            distance = int.MaxValue;
            previous = null;
        }

        public void AddNeighbor(Node neighbor)
        {
            neighbors.Add(neighbor);
        }

        public void SetDistance(int d)
        {
            distance = d;
        }

        public void SetPrevious(Node p)
        {
            previous = p;
        }

        public int GetDistance()
        {
            return distance;
        }

        public Node GetPrevious()
        {
            return previous;
        }

        public List<Node> GetNeighbors()
        {
            return neighbors;
        }

        public MazeCell GetCell()
        {
            return cell;
        }
    }
}
