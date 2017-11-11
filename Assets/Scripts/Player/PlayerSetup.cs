using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour { 
    
    [SerializeField]
    Behaviour[] componentsToDisable;

    private void Start()
    {
        if (!isLocalPlayer)  DisableComponents();
        if (isLocalPlayer)   gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
        
    }


    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)    componentsToDisable[i].enabled = false;
    }


}
