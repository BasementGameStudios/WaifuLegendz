﻿using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{

    [SyncVar(hook ="OnChangeHealth")]
    public int currentHealth;

    public bool isAttackable = true;

    public int maxHealth = 100;
    public Image fillImg; 
    public Text hpText;

    Image localFillImg;
    Text localHpText;

    NavMeshAgent playerAgent;

    public GameObject damageText;

    private void Awake()
    {
        currentHealth = maxHealth;
        if(hpText != null)  hpText.text = currentHealth + "/" + maxHealth;
    }

    private void Start()
    {
        if (GetComponent<NavMeshAgent>() != null) playerAgent = GetComponent<NavMeshAgent>();

        if (isLocalPlayer)
        {
            localHpText = GameObject.Find("LocalHpText").GetComponent<Text>();
            localFillImg = GameObject.Find("LocalHpFillImg").GetComponent<Image>();
            localHpText.text = currentHealth + " / " + maxHealth;
        }

    }

    void OnChangeHealth(int health)
    {
        currentHealth = health;

        fillImg.fillAmount = (float)health / maxHealth;
        hpText.text = health + "/" + maxHealth;
        
        
        if(isLocalPlayer)
        {
            localFillImg.fillAmount = (float)health / maxHealth;
            localHpText.text = health + "/" + maxHealth;
        }

        //print("change hp: " + currentHealth + " / " + maxHealth + " divided=" +(float)currentHealth / maxHealth);
    }


    public void Heal(int amountHealed)
    {
        if (!isServer) return;  
        currentHealth += amountHealed;
    }

    [Command]
    public void CmdHeal(int amountHealed)
    {
        print( GetComponent<Chat>().pName + " cmd heal called");
        RpcHeal(amountHealed);
    }

    [ClientRpc]
    public void RpcHeal(int amountHealed)
    {
        currentHealth += amountHealed;
        hpText.text = currentHealth + "/" + maxHealth;
        print(GetComponent<Chat>().pName + " rpc heal called");
    }

    [Command]
    public void CmdTakeTrueDamage(int amount)
    {
        if (!isAttackable) return;
        RpcTakeTrueDamage(amount);
    }

    [ClientRpc]
    public void RpcTakeTrueDamage(int amount)
    {

        Vector3 dmgTextSpawnLocation = transform.position;
        dmgTextSpawnLocation.y += 8f;
        GameObject go = Instantiate(damageText, dmgTextSpawnLocation, damageText.transform.rotation);
        go.GetComponent<TextMesh>().text = "" + amount;
        Destroy(go, 5f);

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            CmdRespawn();
        }
        
    }

    [Command]
    void CmdRespawn()
    {
        RpcRespawn();
    }

    [ClientRpc]
    void RpcRespawn()
    {
        currentHealth = maxHealth; //heal

        if (gameObject.tag == "Player")
        {
            print(gameObject.name + " died");
        }

        if(gameObject.tag == "Minion" || gameObject.tag == "Tower")
        {
            NetworkServer.Destroy(gameObject);
        }

        if (isLocalPlayer)
        {
            if (GetComponent<Team>().faction == Team.Faction.A)
                playerAgent.Warp(GameObject.Find("StartLocationA").transform.position);
            else
                playerAgent.Warp(GameObject.Find("StartLocationB").transform.position);  
        }

    }


}

