using Photon.Pun; // 유니티용 포톤 컴포넌트들
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestLobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1"; // 게임 버전

    public TextMeshProUGUI connectionInfoText; // 네트워크 정보를 표시할 텍스트
    public Button enterButton; // 룸 접속or만들기 버튼
    public TextMeshProUGUI idInputField; // 입력한 아이디 텍스트
    public TextMeshProUGUI roomIdInputField;

    //[SerializeField] private InputField _roomInput;
    [SerializeField] private TestRoomItemUI _roomItemUIPrefab;
    [SerializeField] private Transform _roomListParent;

    private Dictionary<RoomInfo, TestRoomItemUI> cachedRoomList = new Dictionary<RoomInfo, TestRoomItemUI>();
    private List<TestRoomItemUI> _roomList = new List<TestRoomItemUI>();

    // Start is called before the first frame update
    void Awake()
    {
        // 같은 룸의 유저들에게 자동으로 씬을 로딩
        // 절대 쓰지마 이 기능 오류난다
        //PhotonNetwork.AutomaticallySyncScene = true;
        // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = gameVersion;
        // 룸 접속 버튼을 잠시 비활성화
        enterButton.interactable = false;
        // 접속을 시도 중임을 텍스트로 표시
        connectionInfoText.text = "Please Sign In...";
    }

    public void Login()
    {
        // 유저 아이디 할당
        PhotonNetwork.NickName = idInputField.text;
        Debug.Log($"userId : {PhotonNetwork.NickName}");
        // 설정한 정보를 가지고 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();
        //Received OnSerialization for view ID 2001. We have no such PhotonView! Ignore this if you're joining or leaving a room. State: Joined
        // 위 버그가 생기는 악의 축
        //PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region PhotonCallbacks

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster()
    {
        // 룸 접속 버튼을 활성화
        enterButton.interactable = true;
        // 접속 정보 표시
        connectionInfoText.text = "Sign In Success";
        connectionInfoText.color = Color.green;
        PhotonNetwork.JoinLobby();
    }

    // 연결 종료시...
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 룸 접속 버튼을 비활성화
        enterButton.interactable = false;
        // 접속 정보 표시
        connectionInfoText.text = "Sign In Failed... Retrying...";
        // 마스터 서버로의 재접속 시도
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
        // 룸이 전부 roomList에 들어오는것이 아닌
        // 룸이 변경됐을 때, 해당 룸의 roominfo만 들어온다. 
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            // 만약 룸이 삭제 리스트에 존재할 경우
            if (info.RemovedFromList)
            {
                // 룸 오브젝트도 지우고 정보도 삭제해준다
                Destroy(cachedRoomList[info].gameObject);
                cachedRoomList.Remove(info);
            }
            else
            {
                // 한명도 없는 룸은 건너뛰기
                if (roomList[i].PlayerCount == 0) { continue; }
                TestRoomItemUI newRoomItem;
                // 기존룸의 정보가 있다면?
                if (cachedRoomList.ContainsKey(info))
                {
                    newRoomItem = cachedRoomList[info];
                } // 기존 룸에 정보가 없는 신규 룸이라면?
                else
                {
                    newRoomItem = Instantiate(_roomItemUIPrefab);
                    cachedRoomList[info] = newRoomItem;
                }
                // 변경사항에 맞춰 룸 상태 변경
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

    // 방이 있으면 접속 없으면 생성
    public void JoinOrCreate()
    {
        // 룸 이름 정의
        string roomId = roomIdInputField.text;
        // 룸 속성 정의
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 3;     // 최대 동시 접속자 수: 20명 
        roomOptions.IsOpen = true;       // 룸의 오픈 여부
        roomOptions.IsVisible = true;    // 공개 여부
        roomOptions.PublishUserId = true; // 각 PhotonPlayer.UserId를 사용해 각 userID에 접근 가능
        // 로비 타입 정의
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
