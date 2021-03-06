﻿using UnityEngine.Networking;
using UnityEngine;
using System.ComponentModel;
using UnityEngine.UI;

[AddComponentMenu("Network/NetworkManagerHUD")]
[RequireComponent(typeof(NetworkManager))]
[EditorBrowsable(EditorBrowsableState.Never)]
public class NetworkUI : MonoBehaviour
{
    public NetworkManager manager;
    [SerializeField] public bool showGUI = true;
    [SerializeField] public int offsetX;
    [SerializeField] public int offsetY;

    //public InputField nameInput;
    Camera lobbyCamera;

    // Runtime variable
    bool m_ShowServer;

    void Awake()
    {
        manager = GetComponent<NetworkManager>();
        //lobbyCamera = GameObject.Find("LobbyCamera").GetComponent<Camera>();
    }


    void OnGUI()
    {
        if (!showGUI)
            return;

        //int xpos = 10 + offsetX;
        //int ypos = 40 + offsetY;
        int xpos = offsetX, ypos = offsetY;
        const int spacing = 24; 

        bool noConnection = (manager.client == null || manager.client.connection == null ||
                             manager.client.connection.connectionId == -1);

        if (!manager.IsClientConnected() && !NetworkServer.active && manager.matchMaker == null)
        {
            if (noConnection)
            {
                if (UnityEngine.Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Host"))
                    {
                        if (NameIsValid())
                        {
                            //lobbyCamera.gameObject.SetActive(false);
                            manager.StartHost();
                        }
                        else UserInterfaceController.nameInput.placeholder.GetComponent<Text>().text = "Name cannot be blank";
                    }
                    ypos += spacing;
                }

                if (GUI.Button(new Rect(xpos, ypos, 105, 20), "IP connect"))
                {
                    if (NameIsValid())
                    {
                        //lobbyCamera.gameObject.SetActive(false);
                        manager.StartClient();
                    }
                    else UserInterfaceController.nameInput.placeholder.GetComponent<Text>().text = "Name cannot be blank";
                }

                manager.networkAddress = GUI.TextField(new Rect(xpos + 100, ypos, 95, 20), manager.networkAddress);
                ypos += spacing;

                if (UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    // cant be a server in webgl build
                    GUI.Box(new Rect(xpos, ypos, 200, 25), "(  WebGL cannot be server  )");
                    ypos += spacing;
                }
            }
            else
            {
                GUI.Label(new Rect(xpos, ypos, 200, 20), "Connecting to " + manager.networkAddress + ":" + manager.networkPort + "..");
                ypos += spacing;


                if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Cancel Connection Attempt"))
                {
                    manager.StopClient();
                }
            }
        }
        else
        {
            if (NetworkServer.active)
            {
                string serverMsg = "Server: port=" + manager.networkPort;
                if (manager.useWebSockets)
                {
                    serverMsg += " (Using WebSockets)";
                }
                //GUI.Label(new Rect(xpos, ypos, 300, 20), serverMsg);
                ypos += spacing;
            }
            if (manager.IsClientConnected())
            {
                //GUI.Label(new Rect(xpos, ypos, 300, 20), "Client: address=" + manager.networkAddress + " port=" + manager.networkPort);
                ypos += spacing;
            }
        }

        if (manager.IsClientConnected() && !ClientScene.ready)
        {
            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Client Ready"))
            {
                ClientScene.Ready(manager.client.connection);

                if (ClientScene.localPlayers.Count == 0)
                {
                    ClientScene.AddPlayer(0);
                }
            }
            ypos += spacing;
        }
        if (NetworkServer.active || manager.IsClientConnected())
        {
            if (GUI.Button(new Rect(xpos, ypos, 200, 20), "Stop"))
            {
                manager.StopHost();
                UserInterfaceController.TransitionToLobbyUI();
                //lobbyCamera.gameObject.SetActive(true);
            }
            ypos += spacing;
        }
        if (!NetworkServer.active && !manager.IsClientConnected() && noConnection)
        {
            ypos += 10;
            if (UnityEngine.Application.platform == RuntimePlatform.WebGLPlayer)
            {
                GUI.Box(new Rect(xpos - 5, ypos, 220, 25), "(WebGL cannot use Match Maker)");
                return;
            }
        }
    }

    private bool NameIsValid()
    {
        return (UserInterfaceController.nameInput.text != null && UserInterfaceController.nameInput.text.Length > 0 && UserInterfaceController.nameInput.text.Length < 50);
    }

}