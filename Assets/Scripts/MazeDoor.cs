using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDoor : MazePassage
{
    public Transform hinge;
    public float angleOpen, closeRot, speed;
    private float openRot;
    public bool opening;

    private MazeDoor OtherSideOfDoor
    {
        get
        {
            return otherCell.GetEdge(direction.GetOpposite()) as MazeDoor;
        }
    }

    private static Quaternion
        normalRotation = Quaternion.Euler(0f, 90f, 0f),
        mirroredRotation = Quaternion.Euler(0f, -90f, 0f);

    //private bool isMirrored;

    public override void Initialize(MazeCell primary, MazeCell other, MazeDirection direction)
    {
        base.Initialize(primary, other, direction);
        if (OtherSideOfDoor != null)
        {
            //hinge.gameObject.SetActive(false);
            //isMirrored = true;
            hinge.localScale = new Vector3(-1f, 1f, 1f);
            Vector3 p = hinge.localPosition;
            p.x = -p.x;
            hinge.localPosition = p;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child != hinge)
            {
                child.GetComponent<Renderer>().material = cell.room.settings.wallMaterial;
            }
        }
    }
    public override void OnPlayerEntered()
    {
        OtherSideOfDoor.cell.room.Show();
        ToggleDoor();
    }

    public override void OnPlayerExited()
    {
        ToggleDoor();
    }

    void Update()
    {
        Vector3 currentRot = hinge.localEulerAngles;
        if (opening)
        {
            if (Mathf.Abs(currentRot.y) < Mathf.Abs(openRot))
            {
                Quaternion targetRotation = Quaternion.Euler(currentRot.x, openRot, currentRot.z);
                hinge.localRotation = Quaternion.Slerp(hinge.localRotation, targetRotation, Time.deltaTime * speed);

                //hinge.localEulerAngles = Vector3.Lerp(currentRot, new Vector3(currentRot.x, openRot, currentRot.z), Time.deltaTime * speed);
            }
        }
        else
        {
            if (Mathf.Abs(currentRot.y) > closeRot)
            {
                Quaternion targetRotation = Quaternion.Euler(currentRot.x, closeRot, currentRot.z);
                hinge.localRotation = Quaternion.Slerp(hinge.localRotation, targetRotation, Time.deltaTime * speed);

                //hinge.localEulerAngles = Vector3.Lerp(currentRot, new Vector3(currentRot.x, closeRot, currentRot.z), Time.deltaTime * speed);
            }
        }
    }

    void ToggleInDoor()
    {
        openRot = angleOpen;
        opening = !opening;
    }

    void ToggleDoor()
    {
        OtherSideOfDoor.ToggleInDoor();
        ToggleInDoor();
    }
}