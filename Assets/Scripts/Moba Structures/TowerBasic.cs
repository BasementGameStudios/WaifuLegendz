using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Tower))]
public class TowerBasic : NetworkBehaviour {


    public Transform projectileSpawnLocation;

    public GameObject towerProjectile;

    Team.Faction faction;


    public Transform currentTarget;

    private void Start()
    {
        faction = GetComponent<Tower>().team;
       // if(isServer)StartCoroutine(TowerScan());
    }

    public override void OnStartServer()
    {
        if (isServer)
            StartCoroutine(TowerScan());
    }



    IEnumerator TowerScan()
    {
        while (true)
        {
            var colls = Physics.OverlapSphere(transform.position, 40f);
            foreach (var col in colls)
            {

                Health target = col.transform.GetComponent<Health>();
                if (target != null && (target.tag == "Player"
                    || target.tag =="Minion"))
                {
                    if (target.transform.GetComponent<Team>().faction != this.faction)
                    {
                        currentTarget = target.transform;
                        StartCoroutine(TargetEnemy());
                    }
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    bool isAttacking;

    IEnumerator TargetEnemy()
    {
        if (!isAttacking)
        {
            isAttacking = true;


            CmdSpawnTowerProjectile();

            while (isAttacking)
            {
                CmdSpawnTowerProjectile();
                yield return new WaitForSeconds(1f);
                if (Vector3.Distance(transform.position, currentTarget.position) > 50f)
                {
                    isAttacking = false;

                }
            }

            isAttacking = false;
        }
    }


    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player"
            || other.transform.tag == "Minion")
        {
            if(other.transform.GetComponent<Team>().faction != this.faction)
            {
                currentTarget = other.transform;
                StartCoroutine(TargetEnemy());
            }
        }
    }*/


    [Command]
    void CmdSpawnTowerProjectile()
    {
        GameObject projectileInstance = (GameObject)Instantiate(towerProjectile,
            projectileSpawnLocation.position, towerProjectile.transform.rotation);

        projectileInstance.GetComponent<TowerProjectile>().faction = this.faction;
        projectileInstance.GetComponent<TowerProjectile>().target = currentTarget;
        NetworkServer.Spawn(projectileInstance);

        Destroy(projectileInstance, 60f);
    }

}
