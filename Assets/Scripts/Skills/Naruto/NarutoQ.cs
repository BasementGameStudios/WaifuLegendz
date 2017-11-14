using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class NarutoQ : Skill {

    public GameObject shurikenPrefab;
    public float missleSpeed = 25.0f;
    public float shurikenAliveTime = 5.0f;

    protected override void SkillAnimation()
    {
        CmdTriggerAttackAnimation("attack");
    }

    protected override bool SkillEffect()
    {
        Ray intersectionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit interactionHitInfo; //raycast hit object
        if (Physics.Raycast(intersectionRay, out interactionHitInfo, Mathf.Infinity))
        {
            transform.LookAt(interactionHitInfo.point);
            CmdSpawnShuriken(interactionHitInfo.point);
            return true;
        }
        return false;
    }

    [Command]
    void CmdSpawnShuriken(Vector3 position)
    {
        StartCoroutine(SpawnShuriken(5, position));
    }

    IEnumerator SpawnShuriken(int numOfShurikens, Vector3 position)
    {
        float j = 10;
        Vector3 spawnShurikenPosition = transform.position + -transform.right * j;
        spawnShurikenPosition.y = 5f;

        Vector3 lookAtPosition = position;
        lookAtPosition.y = 5f;

        int i = 0;
        while(i < numOfShurikens)
        {
            GameObject shuriken = Instantiate(shurikenPrefab, spawnShurikenPosition, shurikenPrefab.transform.rotation);
            shuriken.GetComponent<NarutoQProjectile>().damage = (int)(GetComponent<Stats>().AttackDamage * 0.75);
            Physics.IgnoreCollision(shuriken.GetComponent<Collider>(), GetComponent<Collider>());
            shuriken.GetComponent<NarutoQProjectile>().casterNetId = GetComponent<NetworkIdentity>().netId.Value;
            shuriken.transform.LookAt(lookAtPosition);

            shuriken.GetComponent<Rigidbody>().velocity = shuriken.transform.forward * missleSpeed;
            NetworkServer.Spawn(shuriken);
            Destroy(shuriken, shurikenAliveTime);

            j -= 5f;
            spawnShurikenPosition = transform.position + -transform.right * j;
            spawnShurikenPosition.y = 5f;

            i++;
            yield return new WaitForSeconds(0.05f);
        }

    }

}
