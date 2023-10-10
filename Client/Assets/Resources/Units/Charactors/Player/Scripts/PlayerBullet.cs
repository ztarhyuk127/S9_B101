using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    // �Ѿ��� Ÿ�� ����
    public enum BulletType { Water, WaterSpray };
    [Tooltip("�Ѿ� Ÿ���� �����ϴ� ����")]
    public BulletType bulletType;

    [Tooltip("�Ѿ� ������")]
    public float bulletDamage;

    [Tooltip("������ �÷��̾ �����ϴ� ����")]
    public GameObject attackPlayer;

    // RigidBody�� Trigger üũ �� ��ü�� �浹�ϸ� �߻��ϴ� �Լ�
    void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // �Ѿ��� �Ʒ��� (�±׷� ������) ��ü�� �ε����� ��쿡 �۵�
        if (other.gameObject.CompareTag("Building") || other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Obstacle"))
        {
            // �Ѿ��� �����Ѵ�.
            Destroy(gameObject);
        }

        // �Ѿ��� ���� �ε����� �۵��ϴ� �Լ�
        if (other.gameObject.CompareTag("Enemy"))
        {
            bool isEnemyLive = other.gameObject.GetComponent<Enemy>().isDie;
            EnemyMove enemy = other.gameObject.GetComponent<EnemyMove>();
            if (enemy != null && !isEnemyLive)
            {
                enemy.ChasePlayer(attackPlayer, gameObject);
            }
        }

        if (other.gameObject.CompareTag("Boss"))
        {
            Boss boss = other.gameObject.GetComponent<Boss>();
            boss.pv.RPC("BossHitByPlayer", RpcTarget.All, bulletDamage);
        }
    }
}
