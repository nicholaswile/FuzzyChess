using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustableCamera : MonoBehaviour
{
    [SerializeField] private Camera mainCam;
    private const float Y_ANGLE_MIN = 23.0f;
    private const float Y_ANGLE_MAX = 89.0f;

    public Transform lookAt;
    public Transform camTransform;

    private Camera cam;

    private float distance = 0.60f;
    private float currentX = -70.0f;
    private float currentY = 15.0f;
    private float sensitivityX = 2.9f;
    private float sensitivityY = 2.3f;
    private float scrollSensitivity = 3.0f;
    private float lockStrengthX = 2.5f;
    private bool xAnimation;
    private bool yAnimation;
    private bool zoomAnimation;

    private void Start()
    {
        camTransform = transform;
        cam = mainCam;
        xAnimation = true;
        yAnimation = true;
        zoomAnimation = true;
    }

    private void Update()
    {

        if (xAnimation)
        {
            currentX += .5f;
            if (currentX == 0f)
                xAnimation = false;
        }
        if (yAnimation)
        {
            currentY += .20f;
            if (currentY >= 40f)
                yAnimation = false;
        }
        if (zoomAnimation)
        {
            GetComponent<Camera>().fieldOfView -= .25f;
            if (GetComponent<Camera>().fieldOfView == 60)
                zoomAnimation = false;
        }



        if (Input.GetMouseButton(1) && !xAnimation)
        {
            currentX += Input.GetAxis("Mouse X") * sensitivityX;
            currentY += -Input.GetAxis("Mouse Y") * sensitivityY;

            //Lock X if current X is in range of these values
            if (currentX < lockStrengthX && currentX > -lockStrengthX)
                currentX = 0;
            else if (currentX < 90 + lockStrengthX && currentX > 90 - lockStrengthX)
                currentX = 90;
            else if (currentX < 180 + lockStrengthX && currentX > 180 - lockStrengthX)
                currentX = 180;
            else if (currentX > -180 - lockStrengthX && currentX < -180 + lockStrengthX)
                currentX = 180;
            else if (currentX < -90 + lockStrengthX && currentX > -90 - lockStrengthX)
                currentX = -90;
            else if (currentX < 270 + lockStrengthX && currentX > 270 - lockStrengthX)
                currentX = -90;

            currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && GetComponent<Camera>().fieldOfView > 36)
        {
            GetComponent<Camera>().fieldOfView -= scrollSensitivity;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0 && GetComponent<Camera>().fieldOfView < 80)
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
