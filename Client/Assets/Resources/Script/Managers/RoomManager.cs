using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform _playerListBox;
    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private GameObject _mainUI;
    [SerializeField] private GameObject _selectCharacter;

    private void Start()
    {
        // 플레이어 카드 생성
        GameObject playerItem = PhotonNetwork.Instantiate("Prefabs/UI/PlayerItemUI", _playerListBox.position, _playerListBox.rotation);
        playerItem.transform.SetParent(GameObject.Find("PlayerListBox").transform, false);
        _roomName.text = (string)PhotonNetwork.CurrentRoom.CustomProperties["RoomName"];
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    { 

    }

    public void BackToLobby()
    {
        LoadingManager.OnClickExit();
    }

    public void OnClickSelectCharacter()
    {
        _mainUI.SetActive(false);
        _selectCharacter.SetActive(true);
    }

    public void OnClickSelectCharacterConfirm()
    {
        _mainUI.SetActive(true);
        _selectCharacter.SetActive(false);
    }

}
