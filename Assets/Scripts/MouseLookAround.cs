using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLookAround : MonoBehaviour
{
    private Transform cameraPosition;
    public GameObject particlesPrefabs;
    private GameObject rain;

    float rotationX = 0f;
    float rotationY = 0f;

    public float sensitivity = 10f;

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        rain = Instantiate(particlesPrefabs);
        rain.transform.position = new Vector3(transform.position.x, 3, transform.position.z);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        rotationY += Input.GetAxis("Mouse X") * sensitivity;
        rotationX += Input.GetAxis("Mouse Y") * -1 * sensitivity;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0); 
        cameraPosition.rotation = Quaternion.Euler(0, rotationY, 0);
        transform.position = cameraPosition.position;
        //transform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ);
        //rotationY = Input.GetAxis("Mouse Z") * sensitivity;
        rain.transform.position = new Vector3(transform.position.x, 3, transform.position.z);
    }

    public void SetPlayer(Player player)
    {
        cameraPosition = player.transform;
    }

    void OnDestroy()
    {
        Destroy(rain);
    }
}
