using UnityEngine;

public class MinionAutoAttack : MonoBehaviour {

    public int minionAutoAttackDamage = 10;
    public float attackSpeed = 1.0f;
    float nextTimeToFire = 0f;
    Team.Faction team;

    private void Start()
    {
        team = GetComponent<Team>().faction;
    }

    private void OnTriggerEnter(Collider other)
    {
        //print("hit trigger something");
        Health target = other.transform.GetComponent<Health>();
        if (target != null && other.transform.GetComponent<Team>().faction != team && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / attackSpeed;
            target.CmdTakeTrueDamage(minionAutoAttackDamage);
        }
    }
}
