using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBoxUI : MonoBehaviour
{
    [SerializeField] private Image _weapon; // 무기 이미지
    [SerializeField] private TMP_Text _loadedAmmo; // 장전된 총알
    [SerializeField] private Sprite[] _weaponSprites; // 무기 스프라이트 목록

    private PlayerAttack _playerAttack;     // 캐릭터 정보를 가져올 객체
    private PlayerAttack.AttackType _type;   // 캐릭터 타입

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
