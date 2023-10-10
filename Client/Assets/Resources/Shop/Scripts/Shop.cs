using JetBrains.Annotations;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    Player player;
    PlayerAttack playerAttack;
    PlayerMove playerMove;
    bool hasPurchased;
    GameObject targetPlayer;

    public void Enter(GameObject enterPlayer)
    {
        
        if (hasPurchased == false) 
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            targetPlayer = enterPlayer;
            player = targetPlayer.GetComponent<Player>();
            playerAttack = targetPlayer.GetComponent<PlayerAttack>();
            playerMove = targetPlayer.GetComponent<PlayerMove>();
            uiGroup.anchoredPosition = Vector3.zero;
            Debug.Log("enter");
        }
        else
        {
            Debug.Log("이미 보급을 받았군. 다른 동료를 위해 배려해주게.");
            return;
        }
    }

    public void Exit()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        player.isShop = false;
        uiGroup.anchoredPosition = Vector3.down * 1000;
        Debug.Log("exit");
    }

    public void Buy_attack()
    {
        if (hasPurchased == false) 
        {
            player.attackpower += 5f;
            hasPurchased = true;
            Exit();
            
        }
        else
        {
            Debug.Log("공증? 어림없다.");
        }
    }

    public void Buy_Health()
    {
        if (hasPurchased == false)
        {
            player.maxHealth += 50;
            player.health = player.maxHealth;
            hasPurchased = true;
            Exit();
        }
        else
        {
            Debug.Log("체력? 어림없다.");
        }
    }
    
    public void Buy_speed()
    {
        if (hasPurchased == false)
        {
            playerMove.moveSpeed += 1f;
            hasPurchased = true;
            Exit();
        }
        else
        {
            Debug.Log("이속? 어림없다.");
        }
    }
    public void Buy_special()
    {
        if (hasPurchased == false)
        {
            if (playerAttack.attackType == PlayerAttack.AttackType.Range)
            {
                playerMove.moveSpeed += 1.0f;

            }
            else if (playerAttack.attackType == PlayerAttack.AttackType.Melee)
            {
                playerMove.moveSpeed += 1.2f;
            }
            else if (playerAttack.attackType == PlayerAttack.AttackType.Gauge)
            {
                playerMove.moveSpeed += 0.9f;
            }
            hasPurchased = true;
        }
    }

}
