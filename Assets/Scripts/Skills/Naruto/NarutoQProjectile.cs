using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class NarutoQProjectile : NetworkBehaviour {


    public AudioSource impactSound;
    public int damage;
    public uint casterNetId;

    public float shurikenRotationSpeed = 360f;

    private void Update()
    {
        transform.Rotate(Vector3.up * shurikenRotationSpeed * Time.deltaTime, Space.Self);
    }

    bool hasCollided;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<NarutoQProjectile>()!=null) {
            if (other.transform.GetComponent<NarutoQProjectile>().casterNetId == this.casterNetId)
            {
                return;
            }
        }

        hasCollided = true;

        if (!isServer) return;

        Health target = other.transform.GetComponent<Health>();
        if(target != null && target.GetComponent<NetworkIdentity>().netId.Value != casterNetId)
        {
            target.CmdTakeTrueDamage(damage);
        }

        GetComponent<Collider>().enabled = false;

        NetworkServer.Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if(hasCollided)
        AudioSource.PlayClipAtPoint(impactSound.clip, transform.position);
    }


}
