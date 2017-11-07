using UnityEngine;

public class PlayerMouseLook : MouseLook {

    public float mouseSensitivity = 100.0f;
    public float clampAngle = 90.0f;
    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis

    public Camera playerCam;

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    private void FixedUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        Quaternion localRotation = Quaternion.Euler(0f, rotY, 0f);
        Quaternion camRotation = Quaternion.Euler(rotX, rotY, 0f);
        transform.rotation = localRotation;
        playerCam.transform.rotation = camRotation;
    }
}
