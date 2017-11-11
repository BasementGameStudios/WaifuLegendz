using UnityEngine.Networking;
using UnityEngine;

public class NaurotoAAProjectile : NetworkBehaviour {


    public int damage;
    public float projectileSpeed = 25.0f;
    public GameObject receiver;
    bool _start;

    public void SetupProjectile(int damage, GameObject receiver)
    {
        this.damage = damage;
        this.receiver = receiver;
        _start = true;
    }

    float yRot = 0;

    private void Update()
    {
        if(_start)
        {
            if (receiver == null)
            {
                NetworkServer.Destroy(gameObject);
                return;
            }

            Vector3 targetPosition = receiver.transform.position;
            targetPosition.y += 2f;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * projectileSpeed);
            
        }

        transform.Rotate(0f, 5f, 0f);

    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        GameObject hit = other.gameObject;
        if (hit.gameObject.layer == LayerMask.NameToLayer("LocalPlayer")) return;
        if (hit.tag == "Minion"
            || hit.tag == "Player"
            || hit.tag == "Tower")
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.CmdTakeTrueDamage(damage);
            }
        }

        NetworkServer.Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isServer) return;

        GameObject hit = collision.gameObject;

        if(hit.tag == "Minion"
            ||hit.tag == "Player"
            ||hit.tag == "Tower")
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.CmdTakeTrueDamage(damage);
            }
        }

        NetworkServer.Destroy(gameObject);
    }



}
