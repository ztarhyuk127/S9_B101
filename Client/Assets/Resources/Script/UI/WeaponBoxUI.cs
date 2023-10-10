using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBoxUI : MonoBehaviour
{
    [SerializeField] private Image _weapon; // ���� �̹���
    [SerializeField] private TMP_Text _loadedAmmo; // ������ �Ѿ�
    [SerializeField] private Sprite[] _weaponSprites; // ���� ��������Ʈ ���

    private PlayerAttack _playerAttack;     // ĳ���� ������ ������ ��ü
    private PlayerAttack.AttackType _type;   // ĳ���� Ÿ��

    private void Start()
    {
        Debug.Log(_playerAttack);
        SetCheracterType();
        SetWeapon();
    }

    private void Update()
    {
        SetAmmo();
    }

    public void SetPlayerAttack(PlayerAttack playerAttack)
    {
        _playerAttack = playerAttack;
    }

    public void SetCheracterType()
    {
        _type = _playerAttack.attackType;
    }

    public void SetWeapon()
    {
        if (_type == PlayerAttack.AttackType.Range)
        {
            _weapon.sprite = _weaponSprites[0];
        }
        else if (_type == PlayerAttack.AttackType.Melee)
        {
            _weapon.sprite = _weaponSprites[1];
        }
        else if (_type == PlayerAttack.AttackType.Gauge)
        {
            _weapon.sprite = _weaponSprites[2];
        }
    }

    public void SetAmmo()
    {
        if (_type == PlayerAttack.AttackType.Range)
        {
            _loadedAmmo.text = _playerAttack.curBulletValue.ToString();
        }
        else if (_type == PlayerAttack.AttackType.Gauge)
        {
            _loadedAmmo.text = _playerAttack.curGauge.ToString();
        }
    }
}
