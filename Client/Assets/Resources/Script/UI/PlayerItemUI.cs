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
        // RPC�� ���� ������ ���� ��쿡�� ù��° ĳ���ͷ� �ʱ�ȭ
        if (_playerImage.sprite.IsUnityNull())
        {
            _playerImage.sprite = _characters[0];
        }


        if (_photonView.IsMine)
        {
            if (PlayerPrefs.HasKey("CharacterType"))
            {
                int type = PlayerPrefs.GetString("CharacterType")[^1] - 'A';
                // �ڿ��� �ε���
                OnSelectCharacter(type);
            }
            else
            {
                // �⺻ player �Ӽ� ����
                PlayerPrefs.SetString("CharacterType", "FireFighter_A");
            }


            // UI�� ��ư��� �� PlayerItem�� �Լ� ����
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

        // RpcTarget.AllBuffered : ��ɾ ���ۿ� ��Ƶξ� �Ŀ� ���� �����鵵 ��ɾ �����ϵ��� �� (���ݱ����� ��� ������ ������..)
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
            Debug.Log("Ready�������� Ȯ���غ�");
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
        Debug.Log(_photonView.Owner.NickName + " ���� �����ߴ��� üũ�Ѵ���");
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
            Debug.Log(_photonView.Owner.NickName + " �Ϳ� ���� �α���");
            // ������ ���� ����Ʈ�� ������ �ʵ��� ����
            PhotonNetwork.CurrentRoom.IsVisible = false;
            _photonView.RPC("StopBGM", RpcTarget.All);

            GameManager gm = FindAnyObjectByType<GameManager>();
            int nextStageRandom = Random.Range(0, 3);
            gm.PV.RPC("LoadNextStage", RpcTarget.AllBuffered, nextStageRandom);
        }
    }
}
