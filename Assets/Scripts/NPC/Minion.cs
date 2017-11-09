using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

[RequireComponent(typeof(Team))]
public class Minion : NetworkBehaviour {

    public Tower.Lane lane = Tower.Lane.Mid;
    public float aggroRange = 20;

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


        //minionNavMeshAgent.destination = destinations[0].position;
        if (isServer)
        {
            CmdSetDestination(destinations[0].position);
            StartCoroutine(ScanSurroundings(15f, 1f));
            StartCoroutine(CheckPath());
        }

        animator.SetFloat("speed", 0.5f);

    }

    [Command]
    void CmdSetDestination(Vector3 destination)
    {
        print("cmd set destination " + destination);
        RpcSetDestination(destination);
    }

    [ClientRpc]
    void RpcSetDestination(Vector3 destination)
    {
        print("rpc set dest " + destination);
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
                if (hitColliders[i].tag == "Player" || (hitColliders[i].tag == "Minion"  && hitColliders[i].GetComponent<Minion>().team != this.team)
                    || (hitColliders[i].tag == "Tower" && hitColliders[i].GetComponent<Tower>().team != this.team))
                {  
                    if(enemyTransform == null)
                    {
                        isAttacking = true;
                        enemyTransform = hitColliders[i].transform;
                        CmdSetDestination(enemyTransform.position);
                        //minionNavMeshAgent.SetDestination(enemyTransform.position);
                        StartCoroutine(MinionAttack());
                    }
                }
                i++;
            }

        }
    }


    public Collider coll;

    IEnumerator MinionAttack()
    {
        coll = GetComponent<Collider>();

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

            if (Vector3.Distance(transform.position, enemyTransform.position) < aggroRange)
            {
                if (distance > 1)
                {
                    minionNavMeshAgent.isStopped = false;
                    //minionNavMeshAgent.SetDestination(enemyTransform.position);
                    CmdSetDestination(enemyTransform.position);
                }
                else
                {
                    minionNavMeshAgent.isStopped = true;
                }

                transform.LookAt(enemyTransform.position);


                if(distance <= 2f)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("attack")) animator.SetTrigger("attack");
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

            yield return new WaitForSeconds(0.33f);
        }
    }


}
