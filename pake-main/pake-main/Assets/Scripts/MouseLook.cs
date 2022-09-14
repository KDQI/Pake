using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MouseLook : NetworkBehaviour
{
    [SerializeField] private Camera cam;
    public float mouseSens;

    [SerializeField] private float crouchAmount; //Arvo, jonka mukaan kamera menee alaspäin kun painetaan crouch nappia

    float xRotation = 0f;

    private int zoom = 40;
    private int normal = 80;
    float smooth = 5;

    public bool isZoomed = false;

    public PlayerMovement pm;

    void Start()
    {
        if (!isLocalPlayer)
        {
            cam.enabled = false;
        }
    }
    void Update()
    {
        if (isLocalPlayer)
        {
            MoveCam();

            if (Input.GetMouseButtonDown(1))
            {
                isZoomed = !isZoomed;
            }
            if (isZoomed)
            {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom, Time.deltaTime * smooth);
                pm.Speed = 8f;
            }
            if (Input.GetMouseButtonUp(1))
            {
                if (!isZoomed)
                {
                    pm.Speed = pm.StartSpeed;
                }
            }
            if (!isZoomed)
            {
                cam.fieldOfView = normal;
            }
        }     
    }

    public void setSens(float sens)
    {
        mouseSens = sens;
    }

    public float GetSens()
    {
        return mouseSens;
    }

    void MoveCam()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        this.transform.Rotate(Vector3.up * mouseX);

        //Crouchaaminen
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            cam.transform.localPosition -= new Vector3(0, crouchAmount, 0);
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            cam.transform.localPosition += new Vector3(0, crouchAmount, 0);
        }
    }
}
