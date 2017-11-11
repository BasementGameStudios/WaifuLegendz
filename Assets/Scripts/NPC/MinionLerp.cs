using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MinionLerp : NetworkBehaviour
{
    Vector3 nextPosition; //next position of the most recent update from server
    Vector3 previousPosition; //previous position before updates from server
    Quaternion rotation;
    public float updateRate = 0.2f; //how long we want to wait between position updates
    float progress, startTime;

    private void Start()
    {
        previousPosition = transform.position;
        nextPosition = transform.position;

        if (isServer) StartCoroutine(UpdatePosition());
        
    }

    private void Update()
    {
        LerpPosition();
    }

    IEnumerator UpdatePosition()
    {
        while (enabled)
        {
            CmdSendPosition(transform.position, transform.rotation);
            yield return new WaitForSeconds(updateRate); //don't block client
        }
    }

    void LerpPosition()
    {
        if (isServer) return; 
        float timePassed = Time.time - startTime;
        progress = timePassed / updateRate;

        transform.position = Vector3.Lerp(previousPosition, nextPosition, progress); //Starting position, position that it's going to, a float for smoothing (smoothing based on your frame rate)
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.1f); //Time.deltatime * smooth
    }

    [Command]
    void CmdSendPosition(Vector3 position, Quaternion newRotation)
    {
        RpcReceivePosition(position, newRotation); 
    }

    [ClientRpc]
    void RpcReceivePosition(Vector3 position, Quaternion newRotation)
    {
        if (isServer) return;
       // print("transform rpc pos: " + transform.position);
        nextPosition = position;
        rotation = newRotation;
        startTime = Time.time; //for interpolation
        previousPosition = transform.position;
    }

}
