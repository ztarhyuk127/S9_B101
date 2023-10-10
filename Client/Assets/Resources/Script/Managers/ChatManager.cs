using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public GameObject chatScroll;
    public TMP_InputField chatInput;
    public CanvasGroup chatInputCg;
    public Transform chatContent;
    public GameObject chatText;
    public PhotonView PV;

    private bool inputSelected = false;

    private void Awake()
    {
        chatScroll.SetActive(false);
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return) && !inputSelected)
        {
            chatInput.Select();
        }
    }

    public void OnEndEditCallback()
    {
        if (chatInput.text == null || chatInput.text.Length == 0) 
        {
            return;
        }

        string msg = chatInput.text;
        string user = PlayerPrefs.GetString("Nickname");
        user = user[..user.LastIndexOf("#")];

        string chat = $"{user} : {msg}";
        PV.RPC("SendChat", RpcTarget.All, chat);
        chatInput.text = "";
        chatInput.ActivateInputField();
    }

    public void OnSelectCallback()
    {
        chatInputCg.alpha = 1.0f;
        inputSelected = true;
    }

    public void OnDeSelectCallback()
    {
        chatInputCg.alpha = 0.0f;
        inputSelected = false;
    }

    [PunRPC]
    public void SendChat(string chat)
    {
        GameObject newChat = Instantiate(chatText, chatContent); 
        newChat.GetComponent<TextMeshProUGUI>().text = chat;
    }

    public void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        chatScroll.SetActive(true);
        SceneManager.sceneLoaded -= LoadSceneEnd;
    }
}
