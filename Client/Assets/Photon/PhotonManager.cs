using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1"; // ���� ����
    // ��ũ��Ʈ�� ���۵��ڸ���
    void Start()
    {
        // ���ӿ� �ʿ��� ����(���� ����) ����
        PhotonNetwork.GameVersion = gameVersion;
        // ������ ������ ������ ������ ���� ���� �õ�
        PhotonNetwork.ConnectUsingSettings();

        //Invoke("SpawnPlayer", 0.1f);
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("������ ���� ����");
        Connect();
    }
    public void Connect()
    {
        // ������ ������ �������̶��
        if (PhotonNetwork.IsConnected)
        {  
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // ������ �������� ������ �õ�
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // �ִ� 4���� ���� ������ ����� ����
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("��������ϴ�");
        //  ĳ���� ���� ������ �迭�� ����
        Transform point = GameObject.Find("SpawnPoint").GetComponentInChildren<Transform>();
        //int idx = Random.Range(1, points.Length);
        // ĳ���͸� ����
        PhotonNetwork.Instantiate("Units/Charactors/Player/Prefabs/FireFighter_A", point.position, point.rotation, 0);
    }

    void SpawnPlayer()
    {
        ////  ĳ���� ���� ������ �迭�� ����
        //Transform point = GameObject.Find("SpawnPoint").GetComponentInChildren<Transform>();
        ////int idx = Random.Range(1, points.Length);
        //// ĳ���͸� ����
        //PhotonNetwork.Instantiate("Units/Charactors/Player/Prefabs/FireFighter_A", point.position, point.rotation, 0);
    }
}
