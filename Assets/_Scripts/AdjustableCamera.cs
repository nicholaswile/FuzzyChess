using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustableCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    private const float Y_ANGLE_MIN = 10.0f;
    private const float Y_ANGLE_MAX = 70.0f;

    public Transform lookAt;
    public Transform camTransform;

    private Camera cam;

    private float distance = 0.60f;
    private float currentX = 0.0f;
    private float currentY = 32.0f;
    private float sensitivityX = 3.0f;
    private float sensitivityY = 2.0f;
    private float scrollSensitivity = 3.0f;

    private void Start()
    {
        camTransform = transform;
        cam = mainCam;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            currentX += Input.GetAxis("Mouse X")*sensitivityX;
            currentY += -Input.GetAxis("Mouse Y")*sensitivityY;
            currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
        }
        if (Input.GetAxis ("Mouse ScrollWheel") > 0 && GetComponent<Camera>().fieldOfView > 36)
        {
            GetComponent<Camera>().fieldOfView -= scrollSensitivity;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && GetComponent<Camera>().fieldOfView < 55)
        {
            GetComponent<Camera>().fieldOfView += scrollSensitivity;
        }
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, -5);
        camTransform.position = lookAt.position + rotation * dir;
        camTransform.LookAt(lookAt.position);
    }
}
