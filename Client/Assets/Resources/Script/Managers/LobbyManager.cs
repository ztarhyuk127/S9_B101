using TMPro;
using UnityEngine;

// 로비 왼쪽, 오른쪽 밑 닉네임 표시 제어 매니저
public class LobbyManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _rightPanel;
    [SerializeField]
    private GameObject _browserBox;
    [SerializeField]
    private GameObject _rankBox;
    [SerializeField]
    private TMP_Text _nickname;

    private void Awake()
    {
        _nickname.text = PlayerPrefs.GetString("Nickname");
        _rightPanel.SetActive(false);
        _browserBox.SetActive(false);
        _rankBox.SetActive(false);
    }

    private void Start()
    {
        BgmManager bgmManager = FindObjectOfType<BgmManager>();
        if (bgmManager != null)
        {
            bgmManager.Play();
        }
    }

    public void OnClickGameStart()
    {
        _rightPanel.SetActive(true);
        _browserBox.SetActive(true);
        _rankBox.SetActive(false);
    }

    public void OnClickRanking()
    {
        _rightPanel.SetActive(true);
        _browserBox.SetActive(false);
        _rankBox.SetActive(true);
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

}
