using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TestRoomItemUI : MonoBehaviour
{
    public TestLobbyManager LobbyManagerParent;
    [SerializeField] private Text _roomName;
    [SerializeField] private Text _roomCount;
    [SerializeField] private UnityEngine.UI.Button _joinButton;
    public void SetName(string roomName)
    {
        _roomName.text = roomName;
    }

    public void SetCount(string roomCount)
    {
        _roomCount.text = roomCount + " / 3";
    }

    public void SetButtonEnable(bool flag)
    {
        _joinButton.interactable = flag;
    }

    public void OnJoinPressed()
    {
        LobbyManagerParent.JoinRoom(_roomName.text);
    }
}
