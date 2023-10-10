using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    [SerializeField] private Image _playerIcon;
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private Image _healthBar;
    [SerializeField] private TMP_Text _health;

    public Color _highColor;
    public Color _middleColor;
    public Color _lowColor;

    [SerializeField] private Image[] _bufList;
    [SerializeField] private TMP_Text[] _bufListText;
    [SerializeField] private Sprite[] _playerIcons;

    public PhotonView _PV;
    public PlayerUIManager _playerUIManager;
    private Player _player;

    private void Start()
    {
        // 모든 플레이어 스크립트 찾기
        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        foreach (Player player in players)
        {
            // 내 소유 플레이어이면 내 상태를 갱신하는 PlayerStatus 생성
            if (player.pv.Owner == _PV.Owner)
            {
                SetPlayer(player);
                player._playerStatusUI = this;
                _playerName.text = _PV.Owner.NickName;
                PlayerAttack.AttackType type = player.playerAttack.attackType;

                switch(type)
                {
                    case PlayerAttack.AttackType.Range:
                        _playerIcon.sprite = _playerIcons[0];
                        break;

                    case PlayerAttack.AttackType.Melee:
                        _playerIcon.sprite = _playerIcons[1];
                        break;

                    case PlayerAttack.AttackType.Gauge:
                        _playerIcon.sprite = _playerIcons[2];
                        break;
                }

                break;
            }
        }

        if (_player.photonView.IsMine)
        {
            _playerName.gameObject.SetActive(false);
        }
        else
        {
            _health.gameObject.SetActive(false);
        }

        _playerUIManager = FindObjectOfType<PlayerUIManager>();

        if (_playerUIManager != null)
        {
            if (_PV.IsMine)
            {
                Debug.Log("이건 내 체력바야");
                transform.SetParent(_playerUIManager._playerStatusBox, false);
            }
            else
            {
                Debug.Log("이건 다른놈 체력바야");
                transform.SetParent(_playerUIManager._friendStatusBox, false);
            }
        }
    }

    private void Update()
    {
        if (_player == null)
        {
            Destroy(gameObject); 
            return;
        }

        SetHealth();
        SetBuf();
    }

    public void SetPlayerUIManager(PlayerUIManager playerUIManager)
    {
        _playerUIManager = playerUIManager;
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public void SetHealth()
    {
        float health = _player.health;
        if (health < 0) health = 0.0f;
        float maxHealth = _player.maxHealth;
        float ratio = health / maxHealth;

        if (ratio > 0.6) 
        {
            _health.color = _highColor;
            _healthBar.color = _highColor;
        }
        else if (ratio > 0.3)
        {
            _health.color = _middleColor;
            _healthBar.color = _middleColor;
        }
        else
        {
            _health.color = _lowColor;
            _healthBar.color = _lowColor;
        }

        _health.text = $"+{(int)health}";
        _healthBar.fillAmount = ratio;

    }

    public void SetBuf()
    {
        int j = 0;
        foreach(int i in _player.countItemUIAmount) {
            if (i > 0)
            {
                if (!_bufList[j].gameObject.activeSelf)
                {
                    _bufList[j].gameObject.SetActive(true);
                }

                if (i > 1)
                {
                    _bufListText[j].text = $"X{i}";
                }
            }
            j++;
        }
    }
}
