using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class MinionSpawn : NetworkBehaviour {

    public GameObject minionPrefab;
    public float timeBetweenSpawns = 5.0f;
    public int minionsPerWave = 3;

    private void Start()
    {
        print(isServer);
        if(isServer) StartCoroutine(SpawnMinion());
    }


    IEnumerator SpawnMinion()
    {
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            for(int i=0; i < minionsPerWave; i++)
            {
                GameObject minionInstance = Instantiate(minionPrefab, transform.position, minionPrefab.transform.rotation); //GameObject minionInstance = 
                NetworkServer.Spawn(minionInstance);
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }


    [Command]
    void CmdSetAuthority(NetworkIdentity grabID, NetworkIdentity playerID)
    {
        grabID.AssignClientAuthority(connectionToClient);
    }

}
