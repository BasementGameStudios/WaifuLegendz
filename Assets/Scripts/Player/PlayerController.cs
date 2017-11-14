using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public string HeroName;

    private NavMeshAgent playerAgent;
    private Animator animator;

    public Transform head;

    private void Start()
    {
        playerAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (isLocalPlayer)
        {
            Camera.main.GetComponent<DiabloCam>().SetTarget(transform);
            if (GetComponent<Team>().faction == Team.Faction.A)
            {
                playerAgent.Warp(GameObject.Find("StartLocationA").transform.position);
            }
            else
            {
                playerAgent.Warp(GameObject.Find("StartLocationB").transform.position);
            }
        }
        
    }



    private void Update()
    {
        if (isLocalPlayer)
        {
            if ((Input.GetMouseButtonDown(1)) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) //see if we're hovering over UI and don't send out a ray 
                GetInteraction();
        }

        
        if(playerAgent.remainingDistance > 1f)
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
                animator.SetFloat("speed", 1f);
        }
        else
        {
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animator.SetFloat("speed", 0f);                
            }
        }

    }

    private void LateUpdate()
    {
        if (head != null && playerAgent.remainingDistance < 1f)
        {
            print(head.rotation);
            head.LookAt(Camera.main.transform);
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
