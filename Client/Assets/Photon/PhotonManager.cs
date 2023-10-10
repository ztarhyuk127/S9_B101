using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1"; // 게임 버전
    // 스크립트가 시작되자마자
    void Start()
    {
        // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = gameVersion;
        // 설정한 정보를 가지고 마스터 서버 접속 시도
        PhotonNetwork.ConnectUsingSettings();

        //Invoke("SpawnPlayer", 0.1f);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 접속");
        Connect();
    }
    public void Connect()
    {
        // 마스터 서버에 접속중이라면
        if (PhotonNetwork.IsConnected)
        {  
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // 마스터 서버로의 재접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // 최대 4명을 수용 가능한 빈방을 생성
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("만들었습니다");
        //  캐릭터 출현 정보를 배열에 저장
        Transform point = GameObject.Find("SpawnPoint").GetComponentInChildren<Transform>();
        //int idx = Random.Range(1, points.Length);
        // 캐릭터를 생성
        PhotonNetwork.Instantiate("Units/Charactors/Player/Prefabs/FireFighter_A", point.position, point.rotation, 0);
    }

    void SpawnPlayer()
    {
        ////  캐릭터 출현 정보를 배열에 저장
        //Transform point = GameObject.Find("SpawnPoint").GetComponentInChildren<Transform>();
        ////int idx = Random.Range(1, points.Length);
        //// 캐릭터를 생성
        //PhotonNetwork.Instantiate("Units/Charactors/Player/Prefabs/FireFighter_A", point.position, point.rotation, 0);
    }
}
