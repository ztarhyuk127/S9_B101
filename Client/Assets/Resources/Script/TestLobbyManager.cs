using Photon.Pun; // ����Ƽ�� ���� ������Ʈ��
using Photon.Realtime; // ���� ���� ���� ���̺귯��
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestLobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1"; // ���� ����

    public TextMeshProUGUI connectionInfoText; // ��Ʈ��ũ ������ ǥ���� �ؽ�Ʈ
    public Button enterButton; // �� ����or����� ��ư
    public TextMeshProUGUI idInputField; // �Է��� ���̵� �ؽ�Ʈ
    public TextMeshProUGUI roomIdInputField;

    //[SerializeField] private InputField _roomInput;
    [SerializeField] private TestRoomItemUI _roomItemUIPrefab;
    [SerializeField] private Transform _roomListParent;

    private Dictionary<RoomInfo, TestRoomItemUI> cachedRoomList = new Dictionary<RoomInfo, TestRoomItemUI>();
    private List<TestRoomItemUI> _roomList = new List<TestRoomItemUI>();

    // Start is called before the first frame update
    void Awake()
    {
        // ���� ���� �����鿡�� �ڵ����� ���� �ε�
        // ���� ������ �� ��� ��������
        //PhotonNetwork.AutomaticallySyncScene = true;
        // ���ӿ� �ʿ��� ����(���� ����) ����
        PhotonNetwork.GameVersion = gameVersion;
        // �� ���� ��ư�� ��� ��Ȱ��ȭ
        enterButton.interactable = false;
        // ������ �õ� ������ �ؽ�Ʈ�� ǥ��
        connectionInfoText.text = "Please Sign In...";
    }

    public void Login()
    {
        // ���� ���̵� �Ҵ�
        PhotonNetwork.NickName = idInputField.text;
        Debug.Log($"userId : {PhotonNetwork.NickName}");
        // ������ ������ ������ ������ ���� ���� �õ�
        PhotonNetwork.ConnectUsingSettings();
        //Received OnSerialization for view ID 2001. We have no such PhotonView! Ignore this if you're joining or leaving a room. State: Joined
        // �� ���װ� ����� ���� ��
        //PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region PhotonCallbacks

    // ������ ���� ���� ������ �ڵ� ����
    public override void OnConnectedToMaster()
    {
        // �� ���� ��ư�� Ȱ��ȭ
        enterButton.interactable = true;
        // ���� ���� ǥ��
        connectionInfoText.text = "Sign In Success";
        connectionInfoText.color = Color.green;
        PhotonNetwork.JoinLobby();
    }

    // ���� �����...
    public override void OnDisconnected(DisconnectCause cause)
    {
        // �� ���� ��ư�� ��Ȱ��ȭ
        enterButton.interactable = false;
        // ���� ���� ǥ��
        connectionInfoText.text = "Sign In Failed... Retrying...";
        // ������ �������� ������ �õ�
        PhotonNetwork.ConnectUsingSettings();
    }
    
    public override void OnJoinedLobby()
    {
        Debug.Log($"InLobby = {PhotonNetwork.InLobby}");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Update Room List");
        UpdateCachedRoomList(roomList);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        
        PhotonNetwork.LoadLevel("Room");
        //PhotonNetwork.LoadLevel("CharactorTest");

    }

    public override void OnCreatedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("You are MasterClient");
        }
    }

    #endregion

    public void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        // ���� ���� roomList�� �����°��� �ƴ�
        // ���� ������� ��, �ش� ���� roominfo�� ���´�. 
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            // ���� ���� ���� ����Ʈ�� ������ ���
            if (info.RemovedFromList)
            {
                // �� ������Ʈ�� ����� ������ �������ش�
                Destroy(cachedRoomList[info].gameObject);
                cachedRoomList.Remove(info);
            }
            else
            {
                // �Ѹ� ���� ���� �ǳʶٱ�
                if (roomList[i].PlayerCount == 0) { continue; }
                TestRoomItemUI newRoomItem;
                // �������� ������ �ִٸ�?
                if (cachedRoomList.ContainsKey(info))
                {
                    newRoomItem = cachedRoomList[info];
                } // ���� �뿡 ������ ���� �ű� ���̶��?
                else
                {
                    newRoomItem = Instantiate(_roomItemUIPrefab);
                    cachedRoomList[info] = newRoomItem;
                }
                // ������׿� ���� �� ���� ����
                newRoomItem.LobbyManagerParent = this;
                newRoomItem.SetName(roomList[i].Name);
                newRoomItem.SetCount(roomList[i].PlayerCount.ToString());
                newRoomItem.transform.SetParent(_roomListParent);
                if(roomList[i].PlayerCount >= 3)
                {
                    newRoomItem.SetButtonEnable(false);
                }
                else
                {
                    newRoomItem.SetButtonEnable(true);
                }
            }
        }
    }

    // ���� ������ ���� ������ ����
    public void JoinOrCreate()
    {
        // �� �̸� ����
        string roomId = roomIdInputField.text;
        // �� �Ӽ� ����
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 3;     // �ִ� ���� ������ ��: 20�� 
        roomOptions.IsOpen = true;       // ���� ���� ����
        roomOptions.IsVisible = true;    // ���� ����
        roomOptions.PublishUserId = true; // �� PhotonPlayer.UserId�� ����� �� userID�� ���� ����
        // �κ� Ÿ�� ����
        TypedLobby typedLobby = null;

        if (string.IsNullOrEmpty(roomId) == false)
        {
            PhotonNetwork.JoinOrCreateRoom(roomId, roomOptions, typedLobby);
        }
    }

    public void JoinRoom(string roomId)
    {
        PhotonNetwork.JoinRoom(roomId);
    }
}
