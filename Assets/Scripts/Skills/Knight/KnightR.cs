using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Summons sword in front of player, in the sky
//after a few seconds, drops down to impact location and damages in an aoe location
//should apply cc as well - todo

public class KnightR : Skill {

    public GameObject ultPrefab;
    public float skillRange = 75.0f;

    protected override void SkillAnimation()
    {
        CmdTriggerAttackAnimation("skillR");
    }

    protected override bool SkillEffect()
    {
        //base.SkillEffect();
        Ray intersectionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit interactionHitInfo; //raycast hit object
        if (Physics.Raycast(intersectionRay, out interactionHitInfo, Mathf.Infinity))
        {
            if(Vector3.Distance(transform.position, interactionHitInfo.point) <= skillRange)
            {
                transform.LookAt(interactionHitInfo.point);
                CmdSpawnSwordFromHeaven(interactionHitInfo.point);
                return true;
            }
        }

        return false;
    }

    [Command]
    void CmdSpawnSwordFromHeaven(Vector3 spawnSwordPosition)
    {
        spawnSwordPosition.y += 150f;
        GameObject sword = Instantiate(ultPrefab, spawnSwordPosition, ultPrefab.transform.rotation);
        print("cmd set dmg: " + damage);
        sword.GetComponent<KnightRProjectile>().damage = (int)GetComponent<Stats>().AttackDamage;
        //sword.GetComponent<Rigidbody>().velocity = sword.transform.up * 30f;
        NetworkServer.Spawn(sword);
        Destroy(sword, 15f);
    }


}
