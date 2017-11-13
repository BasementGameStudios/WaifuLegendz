using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class KnightRProjectile : NetworkBehaviour {

    public GameObject particleEffect;
    public float damageRadius = 15.0f;
    [SyncVar(hook = "OnChangeDmg")]
    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag != "Ground") return;
        particleEffect.SetActive(true);

        GetComponent<Rigidbody>().isKinematic = true;

        if (!isServer) return;

        var colls = Physics.OverlapSphere(particleEffect.transform.position, damageRadius);

        foreach (var col in colls)
        {
            Health target = col.transform.GetComponent<Health>();
            if (target != null && target.gameObject.layer != LayerMask.NameToLayer("LocalPlayer"))
            {
                print("r projectile dmg: " + damage);
                target.CmdTakeTrueDamage(damage);
            }
        }
    }

    void OnChangeDmg(int newDmg)
    {
        damage = newDmg;
    }
}
