using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEnd_1 : MonoBehaviourPun
{ 
    public GameManager gm;

    public bool isTriggered = false;

    void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            Debug.Log("도착 다음 스테이지 ㄱㄱ");
            
            if (!isTriggered && player.photonView.IsMine)
            {
                isTriggered = true;
                gm.PV.RPC("ReadyNextStage", RpcTarget.AllBuffered);
            }
        }
    }
}
