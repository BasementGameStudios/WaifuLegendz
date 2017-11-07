using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceController : MonoBehaviour {

    public GameObject chatPanel;
    #region Lobby Interface Elements
    public static CanvasGroup canvasGroup;
    public static InputField nameInput;
    #endregion

    void Start () {
        chatPanel = GameObject.Find("ChatPanel");
        nameInput = GameObject.Find("nameInputField").GetComponent<InputField>();
        nameInput.text = "Megumin";
        canvasGroup = nameInput.GetComponent<CanvasGroup>();
    }

    public static void TransitionToGameUI()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 0f;
        nameInput.enabled = false;
        canvasGroup.blocksRaycasts = false; 
    }

    public static void TransitionToLobbyUI()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 1f;
        nameInput.enabled = true;
        canvasGroup.blocksRaycasts = true;
    }

    bool toggle = true;

    public void ToggleChatBox()
    {
        return; 

        if (toggle)
        {
            chatPanel.GetComponent<CanvasGroup>().alpha = 0f;
            chatPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
            toggle = false;
        }
        else
        {
            chatPanel.GetComponent<CanvasGroup>().alpha = 0.7f;
            chatPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
            toggle = true;
        }
    }
}
