using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // 유니티용 포톤 컴포넌트
using Photon.Realtime; // 포톤 서비스 라이브러리
using UnityEngine.UI;
using TMPro;
using ExitGames.Client.Photon;

// 로비 씬의 브라우저 창 제어 매니저
public class BrowserManager : MonoBehaviourPunCallbacks
{
    // 로비 구성 버튼
    [SerializeField] private Button _makeRoom;
    [SerializeField] private Button _quickJoin;
    [SerializeField] private Button _inviteJoin;
    [SerializeField] private TMP_InputField _inviteCodeField;
    [SerializeField] private TextMeshProUGUI _connectStatus;

    // 룸 리스트 구성
    [SerializeField] private RoomItemUI _roomItemUIPrefab;
    [SerializeField] private Transform _roomListBox;

    // 방 만들기 구성
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
        _connectStatus.text = "서버에 연결 중 입니다";
        _connectStatus.color = Color.red;
        _makeRoomBox.SetActive (false);
        _quickJoinBox.SetActive (false);


        // 게임 버전 설정
        PhotonNetwork.GameVersion = _gameVersion;
        // 닉네임 설정
        PhotonNetwork.NickName = PlayerPrefs.GetString("Nickname");
        // 마스터 서버 접속
        PhotonNetwork.ConnectUsingSettings();
    }

    #region PhotonCallbacks

    // 마스터 서버 접속 시 콜백
    public override void OnConnectedToMaster()
    {
        _makeRoom.interactable = true;
        _quickJoin.interactable = true;
        _inviteJoin.interactable = true;
        _connectStatus.text = "서버 상태 : Online";
        _connectStatus.color = Color.green;

        PhotonNetwork.JoinLobby();
    }

    // 연결 종료 시 콜백
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log(cause);
        _makeRoom.interactable = false;
        _quickJoin.interactable = false;
        _inviteJoin.interactable = false;
        _connectStatus.text = "서버 연결 해제됨";
        _connectStatus.color = Color.red;
    }
    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 성공!");
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("You are MasterClient");
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 생성 실패!");
        Debug.Log(returnCode + " : " + message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 접속 성공!");
        Debug.Log($"InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"Player Count = {PhotonNetwork.CurrentRoom.PlayerCount}");
        PhotonNetwork.LoadLevel("Room");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 접속 실패!");
        Debug.Log(returnCode + " : " + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("랜덤 방 접속 실패!!");
        Debug.Log(returnCode + " : " + message);
        _quickJoinBox.SetActive(true);

    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("방 목록 업데이트!");
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
                // 룸 오브젝트도 지우고 정보도 삭제해준다
                Destroy(_cachedRoomList[info].gameObject);
                _cachedRoomList.Remove(info);
            }
            else
            {
                // 한명도 없는 룸은 건너뛰기
                if (roomList[i].PlayerCount == 0) { continue; }

                RoomItemUI newRoomItem;

                // 기존룸의 정보가 있다면?
                if (_cachedRoomList.ContainsKey(info))
                {
                    newRoomItem = _cachedRoomList[info];
                }
                else // 기존 룸에 정보가 없는 신규 룸이라면?
                {
                    newRoomItem = Instantiate(_roomItemUIPrefab);
                    _cachedRoomList[info] = newRoomItem;
                }

                // 변경사항에 맞춰 룸 상태 변경
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
        // 데모 버전
        //PhotonNetwork.JoinRandomRoom(null, 1);
        // 시연 버전
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
        // 룸 이름 정의
        string roomName = _roomName.text;
        if (string.IsNullOrEmpty(roomName))
        {
            _makeRoomText.text = "방 이름을 입력해 주세요!";
            return;
        }
        // 룸 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        // 데모
        //roomOptions.MaxPlayers = 1;     // 최대 동시 접속자 수: 20명 
        // 시연
        roomOptions.MaxPlayers = 3;     // 최대 동시 접속자 수: 20명 
        roomOptions.IsOpen = true;       // 룸의 오픈 여부
        roomOptions.IsVisible = true;    // 공개 여부
        roomOptions.PublishUserId = true; // 각 PhotonPlayer.UserId를 사용해 각 userID에 접근 가능
        roomOptions.CustomRoomProperties = new Hashtable() { { "HostName", PlayerPrefs.GetString("Nickname") }, { "RoomName", roomName } };
        roomOptions.CustomRoomPropertiesForLobby = new string[] { "HostName", "RoomName" };

        Debug.Log(PlayerPrefs.GetString("Nickname"));
        Debug.Log(roomName);
        Debug.Log((string)roomOptions.CustomRoomProperties["HostName"]);
        Debug.Log((string)roomOptions.CustomRoomProperties["RoomName"]);

        // 로비 타입 정의
        TypedLobby typedLobby = null;

        // 방 이름을 null로 주어 Photon이 자동 부여하도록 설정
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
