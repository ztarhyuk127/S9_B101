using Photon.Pun;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerItemUI : MonoBehaviourPun
{
    [SerializeField] private PhotonView _photonView;
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private Image _playerImage;
    [SerializeField] private Toggle _playerToggle;
    [SerializeField] private Sprite[] _characters;

    [SerializeField] private GameObject _playerUIPrefab;

    private void Awake()
    {
        transform.SetParent(GameObject.Find("PlayerListBox").transform, false);
    }
    private void Start()
    {
        _playerName.text = _photonView.Owner.NickName;
        // RPC를 통한 지정이 없을 경우에만 첫번째 캐릭터로 초기화
        if (_playerImage.sprite.IsUnityNull())
        {
            _playerImage.sprite = _characters[0];
        }


        if (_photonView.IsMine)
        {
            if (PlayerPrefs.HasKey("CharacterType"))
            {
                int type = PlayerPrefs.GetString("CharacterType")[^1] - 'A';
                // 뒤에서 인덱싱
                OnSelectCharacter(type);
            }
            else
            {
                // 기본 player 속성 지정
                PlayerPrefs.SetString("CharacterType", "FireFighter_A");
            }


            // UI의 버튼들과 내 PlayerItem의 함수 연결
            GameObject.Find("ReadyButton").GetComponent<Button>().onClick.AddListener(this.OnReadyPressed);

            GameObject.Find("SelectGunButton").GetComponent<Button>().onClick.AddListener(() => { this.OnSelectCharacter(0); });
            GameObject.Find("SelectMeleeButton").GetComponent<Button>().onClick.AddListener(() => { this.OnSelectCharacter(1); });
            GameObject.Find("SelectGaugeButton").GetComponent<Button>().onClick.AddListener(() => { this.OnSelectCharacter(2); });
            GameObject.Find("CharacterSelectBox").SetActive(false);
        }
    }

    public void OnReadyPressed()
    {
        _playerToggle.isOn = !_playerToggle.isOn;

        // RpcTarget.AllBuffered : 명령어를 버퍼에 담아두어 후에 들어온 유저들도 명령어를 실행하도록 함 (지금까지의 모든 내용을 실행함..)
        _photonView.RPC("SetReady", RpcTarget.AllBuffered, _playerToggle.isOn);
    }

    public void OnSelectCharacter(int type)
    {
        if (type == 0)
        {
            PlayerPrefs.SetString("CharacterType", "FireFighter_A");
        }
        else if (type == 1)
        {
            PlayerPrefs.SetString("CharacterType", "FireFighter_B");
        }
        else if (type == 2)
        {
            PlayerPrefs.SetString("CharacterType", "FireFighter_C");
        }

        _playerImage.sprite = _characters[type];
        _photonView.RPC("SetCharacter", RpcTarget.AllBuffered, type);
    }

    [PunRPC]
    public void SetReady(bool ready)
    {
        Debug.Log(_photonView.Owner.NickName + " SetReady work");
        _playerToggle.isOn = ready;

        if (ready && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Ready눌렀으니 확인해봐");
            ReadyToStart();
        }
    }

    [PunRPC]
    public void SetCharacter(int type)
    {
        Debug.Log(_photonView.Owner.NickName + " SetCharacter work");
        _playerImage.sprite = _characters[type];
    }

    [PunRPC]
    public void StopBGM()
    {
        BgmManager bgmManager = FindObjectOfType<BgmManager>();
        if (bgmManager != null)
        {
            bgmManager.Stop();
        }
    }

    public void ReadyToStart()
    {
        Debug.Log(_photonView.Owner.NickName + " 전부 레디했는지 체크한다잉");
        var playerItems = GameObject.FindGameObjectsWithTag("PlayerItem");
        int playerCount = 0;
        foreach (GameObject item in playerItems)
        {
            if (item.GetComponentInChildren<Toggle>().isOn)
            {
                playerCount++;
            }
        }

        if (playerCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log(_photonView.Owner.NickName + " 와우 전부 로그인");
            // 시작한 방은 리스트에 보이지 않도록 설정
            PhotonNetwork.CurrentRoom.IsVisible = false;
            _photonView.RPC("StopBGM", RpcTarget.All);

            GameManager gm = FindAnyObjectByType<GameManager>();
            int nextStageRandom = Random.Range(0, 3);
            gm.PV.RPC("LoadNextStage", RpcTarget.AllBuffered, nextStageRandom);
        }
    }
}
