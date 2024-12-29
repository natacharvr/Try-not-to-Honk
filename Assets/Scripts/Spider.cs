using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    public void SetLocation(MazeCell cell)
    {
        transform.position = cell.transform.localPosition;
        
    }
}
