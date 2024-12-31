using UnityEngine;
using System.Collections.Generic;

public class MazeRoom : ScriptableObject
{
    public int settingsIndex;

    public MazeRoomSettings settings;

    private List<MazeCell> cells = new List<MazeCell>();
    private List<Monster> monsters = new List<Monster>();

    private bool isActive = true;

    public void Add(MazeCell cell)
    {
        cell.room = this;
        cells.Add(cell);
        if (isActive ) {
            cell.Show();
        }
        else
        {
            cell.Hide();
        }
    }

    public void Add(Monster monster)
    {
        monsters.Add(monster);
        if (isActive)
        {
            monster.Show();
        }
        else
        {
            monster.Hide();
        }
    }

    public void Hide()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].Hide();
        }
        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].Hide();
        }
        isActive = false;
    }

    public void Show()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].Show();
        }

        for (int i = 0; i < monsters.Count; i++)
        {
            monsters[i].Show();
        }
        isActive = true;
    }

    public MazeCell RandomCell()
    {
        return cells[Random.Range(0, cells.Count)];
    }
}