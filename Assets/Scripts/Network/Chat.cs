using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Chat : NetworkBehaviour {

    public Text chatText;
    public Text playerName;
    public InputField inputField, nameInput;
    public static List<string> messageList = new List<string>();

    public int maxChatlogCount = 100;

    [SyncVar]
    public string pName = "";

    private void Start()
    {
        chatText = GameObject.Find("chatTextContent").GetComponent<Text>();
        inputField = GameObject.Find("chatInputField").GetComponent<InputField>();
        nameInput = GameObject.Find("nameInputField").GetComponent<InputField>();

        if (!isLocalPlayer)
        {
            enabled = false;
            this.gameObject.name = pName + this.GetComponent<NetworkIdentity>().netId;
            //playerName.text = pName;
        }
        if (isLocalPlayer)
        {
            CmdGetPlayerProfile(nameInput.text);
            UserInterfaceController.TransitionToGameUI();
            //GameObject.Find("_GameManager").GetComponent<UserInterfaceController>().ToggleChatBox();
        }
    }

    [Command]
    void CmdGetPlayerProfile(string name)
    {
        RpcSetPlayerProfile(name);
    }

    [ClientRpc]
    void RpcSetPlayerProfile(string name)
    {
        this.pName = name;
        //playerName.text = pName;
        this.gameObject.name = pName + this.GetComponent<NetworkIdentity>().netId;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            inputField.enabled = true;  
            inputField.ActivateInputField();
            if (inputField.text.Length > 0)
            {
                string _message = inputField.text;
                CmdSendChatMessage(pName + ": " + _message);
                inputField.text = "";
                inputField.enabled = false;
            }
        }
    }

    [Command]
    public void CmdSendChatMessage(string message)
    {
        RpcSendChatMessage(message);
    }


    [ClientRpc]
    void RpcSendChatMessage(string message)
    {
        if(messageList.Count > maxChatlogCount)
        {
            messageList.RemoveRange(0, maxChatlogCount/2);
        }
        messageList.Add(message);
        chatText.text = "";
        foreach (string s in messageList)
        {
            chatText.text += s + "\n";
        }
    }

}
