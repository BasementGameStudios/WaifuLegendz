using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour { 
    
    [SerializeField]
    Behaviour[] componentsToDisable;
    [SerializeField]
    GameObject healthBar;


    private void Start()
    {
        if (!isLocalPlayer)  DisableComponents();
        //if (isLocalPlayer)  DisableHealthBar();
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)    componentsToDisable[i].enabled = false;
    }

    void DisableHealthBar()
    {
        healthBar.SetActive(false);
    }

}
