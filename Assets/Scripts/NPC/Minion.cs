using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(Team))]
public class Minion : NetworkBehaviour {

    public Tower.Lane lane = Tower.Lane.Mid;
    public float scanAggroRange = 50.0f;
    public float maxAggroRange = 75.0f;
    public GameObject autoAttackHand;

    private Animator animator;
    private NavMeshAgent minionNavMeshAgent;

    private List<Transform> destinations = new List<Transform>();
    private Transform finalDestination;
    private Transform enemyTransform;

    private int destCounter;
    private bool isAttacking;

    Team.Faction team;

    private void Start()
    {
        team = GetComponent<Team>().faction;
        var towers = GameObject.FindGameObjectsWithTag("Tower");
        var nexuses = GameObject.FindGameObjectsWithTag("Nexus");

        foreach (var nexus in nexuses)
            if (nexus.transform.GetComponent<Team>().faction != this.team)
                finalDestination = nexus.transform;

        foreach (var tower in towers)
        {
            if (tower.transform.GetComponent<Tower>().team != this.team 
                && tower.transform.GetComponent<Tower>().lane == this.lane)
                destinations.Add(tower.transform);
        }

        destinations = destinations.OrderBy(go => go.name).ToList();

        minionNavMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetFloat("speed", 0.5f);

        if (isServer)
        {

            if(destinations.Count < 1)
            {
                if(finalDestination != null)
                {
                    minionNavMeshAgent.destination = finalDestination.position;
                }
                else
                {
                    NetworkServer.Destroy(gameObject);
                    return;
                }
            }
            else
            {
                minionNavMeshAgent.destination = destinations[0].position;
            }

            StartCoroutine(ScanSurroundings(scanAggroRange, 1f));
            StartCoroutine(CheckPath());
        }
        else
        {
            autoAttackHand.SetActive(false);
        }


    }

    [Command]
    void CmdSetDestination(Vector3 destination)
    {
        //print("cmd set destination " + destination);
        RpcSetDestination(destination);
    }

    [ClientRpc]
    void RpcSetDestination(Vector3 destination)
    {
        minionNavMeshAgent.destination = destination;
    }


    IEnumerator CheckPath()
    {
        bool isAttackingNexus = false;

        while (!isAttackingNexus)
        {
            yield return new WaitForSeconds(1f);

            if (isAttacking) continue;

            if (destinations[destCounter] == null)
            {
                if (destCounter < destinations.Count - 1) 
                {
                    //minionNavMeshAgent.destination = destinations[++destCounter].position;
                    CmdSetDestination(destinations[++destCounter].position);
                }
                else if (destCounter == destinations.Count - 1)
                {
                    //minionNavMeshAgent.destination = finalDestination.position;
                    CmdSetDestination(finalDestination.position);
                    isAttackingNexus = true;
                }
            }
        }
    }

    private IEnumerator ScanSurroundings(float radius, float timeBetweenScans)
    {
        int i;

        while (true)
        {
            yield return new WaitForSeconds(timeBetweenScans);

            if (isAttacking) continue; 

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

            i = 0;
            while (i < hitColliders.Length)
            {
                if (hitColliders[i].GetComponent<Team>() != null && hitColliders[i].GetComponent<Team>().faction != this.team)
                {
                    if (hitColliders[i].tag == "Player" 
                        || hitColliders[i].tag == "Minion"
                        || hitColliders[i].tag == "Tower")
                    {
                        if (enemyTransform == null)
                        {
                            isAttacking = true;
                            enemyTransform = hitColliders[i].transform;
                            CmdSetDestination(enemyTransform.position); //minionNavMeshAgent.SetDestination(enemyTransform.position);
                            StartCoroutine(MinionAttack()); 
                        }
                    }
                }
                i++;
            }

        }
    }


    IEnumerator MinionAttack()
    {
        Collider coll = GetComponent<Collider>();

        while (isAttacking)
        {
            if (enemyTransform == null) {
                isAttacking = false;
                minionNavMeshAgent.isStopped = false;
                if (destinations[destCounter]!=null) CmdSetDestination(destinations[destCounter].position);
                //minionNavMeshAgent.destination = destinations[destCounter].position;
                break;
            }


            Vector3 closestPoint = coll.ClosestPointOnBounds(enemyTransform.position);
            float distance = Vector3.Distance(closestPoint, enemyTransform.position);

            if (Vector3.Distance(transform.position, enemyTransform.position) < maxAggroRange)
            {
                if (distance > 1)
                {
                    minionNavMeshAgent.isStopped = false;
                    CmdSetDestination(enemyTransform.position); //minionNavMeshAgent.SetDestination(enemyTransform.position);
                }
                else
                {
                    minionNavMeshAgent.isStopped = true;
                }

                transform.LookAt(enemyTransform.position);


                if(distance <= 5f)
                {
                    CmdTriggerAttackAnimation(); //if (!animator.GetCurrentAnimatorStateInfo(0).IsName("attack")) animator.SetTrigger("attack");
                }
            }
            else
            { 
                enemyTransform = null;
                isAttacking = false;
                minionNavMeshAgent.isStopped = false;
                //minionNavMeshAgent.destination = destinations[destCounter].position;
                CmdSetDestination(destinations[destCounter].position);
            }

            yield return new WaitForSeconds(0.333f);
        }
    }

    [Command]
    void CmdTriggerAttackAnimation()
    {
        RpcTriggerAttackAnimation();
    }

    [ClientRpc]
    void RpcTriggerAttackAnimation()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("attack")) animator.SetTrigger("attack");
    }

}
