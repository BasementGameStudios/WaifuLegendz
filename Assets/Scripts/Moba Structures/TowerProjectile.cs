using UnityEngine;
using UnityEngine.Networking;

public class TowerProjectile : NetworkBehaviour {


    public Transform target;

    public float missleSpeed = 20.0f;

    public int damage = 25;

    public Team.Faction faction = Team.Faction.A;

    private void Update()
    {
        if (!isServer) return;

        if (target != null)
        {
            Vector3 targetPos = target.position;
            targetPos.y = 4f;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, missleSpeed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }   
    }


    private void OnTriggerEnter(Collider other)
    {

        if (!isServer) return;

        if (other.tag == "Tower") return;

        Health target = other.transform.GetComponent<Health>();

        if(target != null && target.GetComponent<Team>()!=null)
        {
            if(target.GetComponent<Team>().faction != this.faction)
            {
                target.CmdTakeTrueDamage(damage);
            }
        }
        else
        {
            return;
        }

        NetworkServer.Destroy(gameObject);
    }

}
