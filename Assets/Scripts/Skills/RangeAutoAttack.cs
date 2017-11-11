using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RangeAutoAttack : AutoAttack {

    public GameObject autoAttackProjectilePrefab;
    public Transform handPosition;

    protected override IEnumerator ApplyBasicAttack(GameObject other, int damage)
    {
        while (true && isAttacking)
        {
            if (Time.time >= nextTimeToFire)
            {
                if (other == null || Vector3.Distance(transform.position, other.transform.position) > currentStatReference.Range) break;    
                nextTimeToFire = Time.time + 1f / attackSpeed;
                transform.LookAt(other.transform);
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("attack")) CmdTriggerAttackAnimation();//animator.SetTrigger("attack");
                CmdLaunchRangedAutoAttack(other, damage);
            }
            yield return null;
        }
    }

    [Command]
    void CmdLaunchRangedAutoAttack(GameObject receiver, int damage)
    {
        GameObject projectile = Instantiate(autoAttackProjectilePrefab, handPosition.position, autoAttackProjectilePrefab.transform.rotation);
        projectile.GetComponent<NaurotoAAProjectile>().SetupProjectile(damage, receiver);
        NetworkServer.Spawn(projectile);
        Destroy(projectile, 100f);
    }

}
