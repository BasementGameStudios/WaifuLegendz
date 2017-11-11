using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiabloCam : MonoBehaviour {

    public float scrollSpeed = 20f;
    public float scrollSensitivity = 1f;
    public Vector3 offset;
    public float minFov = 10.0f;
    public float maxFov = 60.0f;

    public float topBarrier = 0.97f, //top 3% of the screen to be the top barrier
        botBarrier = 0.03f, //bottom 3% of screen
        leftBarrier = 0.97f, //left 3% screen = left barrier
        rightBarrier = 0.03f;

    private bool lockOnToPlayer = true;
    private float fov;
    public Transform playerTransform;

    float MIN_X = 0 , MAX_X = 999;
    float MIN_Y = 10, MAX_Y = 100;
    float MIN_Z = 0, MAX_Z = 999;

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Y)) lockOnToPlayer = !lockOnToPlayer;

        if (!lockOnToPlayer)
        {
            if (Input.mousePosition.y >= Screen.height * topBarrier)
                transform.Translate(Vector3.forward * Time.deltaTime * scrollSpeed, Space.World);

            if (Input.mousePosition.y <= Screen.height * botBarrier)
                transform.Translate(Vector3.back * Time.deltaTime * scrollSpeed, Space.World);

            if (Input.mousePosition.x >= Screen.width * rightBarrier)
                transform.Translate(Vector3.right * Time.deltaTime * scrollSpeed, Space.World);

            if (Input.mousePosition.x <= Screen.width * leftBarrier)
                transform.Translate(Vector3.left * Time.deltaTime * scrollSpeed, Space.World);
        }
        else
        {
            if (playerTransform != null) 
                transform.position = playerTransform.position + offset;
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, MIN_X, MAX_X),
            Mathf.Clamp(transform.position.y, MIN_Y, MAX_Y),
            Mathf.Clamp(transform.position.z, MIN_Z, MAX_Z));
        
        fov = Camera.main.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;

    }

    public void SetTarget(Transform target)
    {
        playerTransform = target;
    }
}
