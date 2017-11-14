using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.AI;

public class MinionSpawn : NetworkBehaviour {

    public GameObject minionPrefab;
    public float timeBetweenSpawns = 5.0f;
    public int minionsPerWave = 3;

    public override void OnStartServer()
    {
        if (isServer) StartCoroutine(SpawnMinion());
        //Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Minion"), LayerMask.NameToLayer("Minion"));
    }


    private void Update()
    {
        if (!isServer) return;
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Ray intersectionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit interactionHitInfo; //raycast hit object
            if (Physics.Raycast(intersectionRay, out interactionHitInfo, Mathf.Infinity))
            {

                Vector3 sourcePosition = interactionHitInfo.point;
                NavMeshHit closestHit;

                if (NavMesh.SamplePosition(sourcePosition, out closestHit, 500, 1))
                {
                    sourcePosition = closestHit.position;
                }
                else
                {
                    //Debug.Log("...");
                }

                GameObject minionInstance = Instantiate(minionPrefab, sourcePosition, minionPrefab.transform.rotation);
                minionInstance.name = minionInstance.name + id++;
                NetworkServer.Spawn(minionInstance);
                print("f1");
            }
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            var minions = GameObject.FindGameObjectsWithTag("Minion");
            foreach(var minion in minions)
            {
                NetworkServer.Destroy(minion);
            }
        }


    }

    int id = 0;

    IEnumerator SpawnMinion()
    {
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            for(int i=0; i < minionsPerWave; i++)
            {
                GameObject minionInstance = Instantiate(minionPrefab, transform.position, minionPrefab.transform.rotation); 
                minionInstance.name = minionInstance.name + id++;
                NetworkServer.Spawn(minionInstance);
                yield return new WaitForSeconds(0.4f);
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

}
