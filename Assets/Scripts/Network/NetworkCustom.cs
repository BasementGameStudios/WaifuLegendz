using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class NetworkCustom : NetworkManager
{
    public GameObject[] playerPrefabs;
    public GameObject[] playerPrefabsLobby;
    public Vector3 offset;
    private GameObject lobbySelectionModelPrefab;
    int chosenCharacter = 2;


    private void Start()
    {
        SelectLobbyPrefab(0);
    }

    //subclass for sending network messages
    public class NetworkMessage : MessageBase
    {
        public int chosenClass;
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        NetworkMessage message = extraMessageReader.ReadMessage<NetworkMessage>();
        int selectedClass = message.chosenClass;
        //Debug.Log("server add with message " + selectedClass);

        if (selectedClass == 0)
        {
            //GameObject player = Instantiate(Resources.Load("Characters/A", typeof(GameObject))) as GameObject;
            GameObject player = Instantiate(playerPrefabs[0]);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        if (selectedClass == 1)
        {
            //GameObject player = Instantiate(Resources.Load("Characters/B", typeof(GameObject))) as GameObject;
            GameObject player = Instantiate(playerPrefabs[1]);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        if (selectedClass == 2)
        {
            //GameObject player = Instantiate(Resources.Load("Characters/B", typeof(GameObject))) as GameObject;
            GameObject player = Instantiate(playerPrefabs[2]);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        Destroy(lobbySelectionModelPrefab);
        NetworkMessage test = new NetworkMessage();
        test.chosenClass = chosenCharacter;
        ClientScene.AddPlayer(conn, 0, test);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        //base.OnClientSceneChanged(conn);
    }

    public void Button1() //naruto atm
    {
        chosenCharacter = 0;
        SelectLobbyPrefab(0);
    }

    public void Button2()
    {
        chosenCharacter = 1;
        SelectLobbyPrefab(1);
    }

    public void Button3()
    {
        chosenCharacter = 2;
        SelectLobbyPrefab(2);
    }

    void SelectLobbyPrefab(int index)
    {
        Destroy(lobbySelectionModelPrefab);
        lobbySelectionModelPrefab = Instantiate(playerPrefabsLobby[index], Camera.main.transform.position + offset, playerPrefabsLobby[0].transform.rotation);
        lobbySelectionModelPrefab.transform.LookAt(Camera.main.transform);
    }

}
//https://forum.unity.com/threads/unet-spawning-different-player-prefabs-solved.387747/#post-2523107
