using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviourPunCallbacks
{
    [SerializeField] RankManager _rankManager;
    [SerializeField] PhotonView _PV;

    private Timer _timer;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        

        if (PhotonNetwork.IsMasterClient)
        {
            _timer = FindObjectOfType<Timer>();
            // Client Master의 기록 가져오기
            int recordTime = _timer.GetIntegerTime();

            // PhotonNetwork로부터 Player들의 ID 가져오기
            var playerList = PhotonNetwork.PlayerList;
            List<int> userIds = new List<int>();

            foreach (var player in playerList)
            {
                userIds.Add(int.Parse(player.NickName[(player.NickName.LastIndexOf('#') + 1)..]));
            }

            _rankManager.Save(recordTime, userIds, GetRecords);
        }

    }

    public void OnClickBackToLobby()
    {
        LoadingManager.OnClickExit();
    }

    public void GetRecords(int recordId)
    {
        _PV.RPC("GetRecordsRPC", RpcTarget.AllBuffered, recordId);
    }

    [PunRPC]
    public void GetRecordsRPC(int recordId)
    {
        Debug.Log("GetRecordsRPC");
        _rankManager.GetRecordWithRank(recordId);
    }

}
