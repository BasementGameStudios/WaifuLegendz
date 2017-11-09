using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{

    private NavMeshAgent playerAgent;
    private Animator animator;

    private void Start()
    {
        playerAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (isLocalPlayer)
        {
            Camera.main.GetComponent<DiabloCam>().setTarget(transform);
        }
    }



    private void Update()
    {
        if (isLocalPlayer)
        {
            if ((Input.GetMouseButtonDown(1)) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) //see if we're hovering over UI and don't send out a ray 
                GetInteraction();
        }

        
        if(playerAgent.remainingDistance > 1)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
                animator.SetFloat("speed", 1f);
        }
        else
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                animator.SetFloat("speed", 0f);
        }

    }

    void GetInteraction()
    {
        Ray intersectionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit interactionHitInfo; //raycast hit object
        if (Physics.Raycast(intersectionRay, out interactionHitInfo, Mathf.Infinity))
        {
            //print(interactionHitInfo.point);
            playerAgent.destination = interactionHitInfo.point;
            CmdMovePlayer(interactionHitInfo.point);
        }
    }

    [Command]
    void CmdMovePlayer(Vector3 endPos)
    {
        RpcMovePlayer(endPos);
    }

    [ClientRpc]
    void RpcMovePlayer(Vector3 endPos)
    {
        if (isLocalPlayer)
            return;
        playerAgent.destination = endPos;
    }

}
