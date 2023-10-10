using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // ����Ƽ�� ���� ������Ʈ
using Photon.Realtime; // ���� ���� ���̺귯��
using UnityEngine.UI;
using TMPro;
using ExitGames.Client.Photon;

// �κ� ���� ������ â ���� �Ŵ���
public class BrowserManager : MonoBehaviourPunCallbacks
{
    // �κ� ���� ��ư
    [SerializeField] private Button _makeRoom;
    [SerializeField] private Button _quickJoin;
    [SerializeField] private Button _inviteJoin;
    [SerializeField] private TMP_InputField _inviteCodeField;
    [SerializeField] private TextMeshProUGUI _connectStatus;

    // �� ����Ʈ ����
    [SerializeField] private RoomItemUI _roomItemUIPrefab;
    [SerializeField] private Transform _roomListBox;

    // �� ����� ����
    [SerializeField] private TMP_InputField _roomName;
    [SerializeField] private GameObject _makeRoomBox;
    [SerializeField] private TMP_Text _makeRoomText;
    [SerializeField] private GameObject _quickJoinBox;

    private string _gameVersion = "1.1";
    private Dictionary<RoomInfo, RoomItemUI> _cachedRoomList = new Dictionary<RoomInfo, RoomItemUI> ();

    private void Awake()
    {
        // fake user
        // PlayerPrefs.SetString("Nickname", "fakeUser");

        _makeRoom.interactable = false;
        _quickJoin.interactable = false;
        _inviteJoin.interactable = false;
        _connectStatus.text = "������ ���� �� �Դϴ�";
        _connectStatus.color = Color.red;
        _makeRoomBox.SetActive (false);
        _quickJoinBox.SetActive (false);


        // ���� ���� ����
        PhotonNetwork.GameVersion = _gameVersion;
        // �г��� ����
        PhotonNetwork.NickName = PlayerPrefs.GetString("Nickname");
        // ������ ���� ����
        PhotonNetwork.ConnectUsingSettings();
    }

    #region PhotonCallbacks

    // ������ ���� ���� �� �ݹ�
    public override void OnConnectedToMaster()
    {
        _makeRoom.interactable = true;
        _quickJoin.interactable = true;
        _inviteJoin.interactable = true;
        _connectStatus.text = "���� ���� : Online";
        _connectStatus.color = Color.green;

        PhotonNetwork.JoinLobby();
    }

    // ���� ���� �� �ݹ�
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause);
        _makeRoom.interactable = false;
        _quickJoin.interactable = false;
        _inviteJoin.interactable = false;
        _connectStatus.text = "���� ���� ������";
        _connectStatus.color = Color.red;
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("�� ���� ����!");
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("You are MasterClient");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("�� ���� ����!");
        Debug.Log(returnCode + " : " + message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("�� ���� ����!");
        Debug.Log($"InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        PhotonNetwork.LoadLevel("Room");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("�� ���� ����!");
        Debug.Log(returnCode + " : " + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("���� �� ���� ����!!");
        Debug.Log(returnCode + " : " + message);
        _quickJoinBox.SetActive(true);

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("�� ��� ������Ʈ!");
        UpdateCachedRoomList(roomList);
    }

    #endregion



    public void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];

            if (info.RemovedFromList && _cachedRoomList.ContainsKey(info))
            {
                // �� ������Ʈ�� ����� ������ �������ش�
                Destroy(_cachedRoomList[info].gameObject);
                _cachedRoomList.Remove(info);
            }
            else
            {
                // �Ѹ� ���� ���� �ǳʶٱ�
                if (roomList[i].PlayerCount == 0) { continue; }

                RoomItemUI newRoomItem;

                // �������� ������ �ִٸ�?
                if (_cachedRoomList.ContainsKey(info))
                {
                    newRoomItem = _cachedRoomList[info];
                }
                else // ���� �뿡 ������ ���� �ű� ���̶��?
                {
                    newRoomItem = Instantiate(_roomItemUIPrefab);
                    _cachedRoomList[info] = newRoomItem;
                }

                // ������׿� ���� �� ���� ����
                newRoomItem._browserManager = this;
                newRoomItem.transform.SetParent(_roomListBox);
                newRoomItem.transform.localScale = Vector3.one;
                newRoomItem.SetRoomId(roomList[i].Name);
                newRoomItem.SetName((string)roomList[i].CustomProperties["RoomName"]);
                newRoomItem.SetHostName((string)roomList[i].CustomProperties["HostName"]);
                newRoomItem.SetCount(roomList[i].PlayerCount.ToString());

                if (roomList[i].PlayerCount >= 3)
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

    public void QuickJoin()
    {
        // ���� ����
        //PhotonNetwork.JoinRandomRoom(null, 1);
        // �ÿ� ����
        PhotonNetwork.JoinRandomRoom(null, 3);
    }

    public void JoinRoom(string roomId)
    {
        PhotonNetwork.JoinRoom(roomId);
    }

    public void JoinInvitedRoom()
    {
        PhotonNetwork.JoinRoom(_inviteCodeField.text);
    }

    public void CreateRoom()
    {
        // �� �̸� ����
        string roomName = _roomName.text;
        if (string.IsNullOrEmpty(roomName))
        {
            _makeRoomText.text = "�� �̸��� �Է��� �ּ���!";
            return;
        }
        // �� �Ӽ� ����
        RoomOptions roomOptions = new RoomOptions();
        // ����
        //roomOptions.MaxPlayers = 1;     // �ִ� ���� ������ ��: 20�� 
        // �ÿ�
        roomOptions.MaxPlayers = 3;     // �ִ� ���� ������ ��: 20�� 
        roomOptions.IsOpen = true;       // ���� ���� ����
        roomOptions.IsVisible = true;    // ���� ����
        roomOptions.PublishUserId = true; // �� PhotonPlayer.UserId�� ����� �� userID�� ���� ����
        roomOptions.CustomRoomProperties = new Hashtable() { { "HostName", PlayerPrefs.GetString("Nickname") }, { "RoomName", roomName } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "HostName", "RoomName" };

        Debug.Log(PlayerPrefs.GetString("Nickname"));
        Debug.Log(roomName);
        Debug.Log((string)roomOptions.CustomRoomProperties["HostName"]);
        Debug.Log((string)roomOptions.CustomRoomProperties["RoomName"]);

        // �κ� Ÿ�� ����
        TypedLobby typedLobby = null;

        // �� �̸��� null�� �־� Photon�� �ڵ� �ο��ϵ��� ����
        PhotonNetwork.CreateRoom(null, roomOptions, typedLobby);
    }

    public void OnClickMakeRoom()
    {
        _makeRoomBox.SetActive(true);
    }

    public void OnClickMakeRoomCancel()
    {
        _makeRoomBox.SetActive(false);
    }

    public void OnClickQuickJoinClose()
    {
        _quickJoinBox.SetActive(false);
    }
}
