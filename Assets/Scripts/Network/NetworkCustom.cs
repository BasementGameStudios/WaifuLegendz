using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class NetworkCustom : NetworkManager
{
    int chosenCharacter = 0;
    public GameObject[] playerPrefabs;

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
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
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
    }

    public void Button2()
    {
        chosenCharacter = 1;
    }

}
//https://forum.unity.com/threads/unet-spawning-different-player-prefabs-solved.387747/#post-2523107
