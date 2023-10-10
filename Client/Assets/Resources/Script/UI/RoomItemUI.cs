using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 방 목록 아이템 제어 클래스
public class RoomItemUI : MonoBehaviour
{
    public BrowserManager _browserManager;

    [SerializeField] private TMP_Text _roomName;
    [SerializeField] private TMP_Text _hostName;
    [SerializeField] private TMP_Text _roomCount;
    [SerializeField] private Button _joinButton;

    private string _roomId;

    public void SetRoomId(string roomId)
    {
        _roomId = roomId;
    }

    public void SetName(string roomName)
    {
        _roomName.text = roomName;
    }

    public void SetHostName(string hostName)
    {
        _hostName.text = hostName;
    }

    public void SetCount(string roomCount)
    {
        _roomCount.text = roomCount + "/3";
    }

    public void SetButtonEnable(bool flag)
    {
        _joinButton.interactable = flag;
    }

    public void OnJoinPressed()
    {
        _browserManager.JoinRoom(_roomId);
    }
}
