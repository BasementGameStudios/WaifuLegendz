using UnityEngine;

public class FaceCamera : MonoBehaviour {


    private void LateUpdate()
    {
        if(Camera.main != null)
            transform.LookAt(Camera.main.transform);
    }

}
