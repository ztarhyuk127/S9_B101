using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    // 총알의 타입 변수
    public enum BulletType { Water, WaterSpray };
    [Tooltip("총알 타입을 지정하는 변수")]
    public BulletType bulletType;

    [Tooltip("총알 데미지")]
    public float bulletDamage;

    [Tooltip("공격한 플레이어를 지정하는 변수")]
    public GameObject attackPlayer;

    // RigidBody의 Trigger 체크 시 물체와 충돌하면 발생하는 함수
    void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // 총알이 아래의 (태그로 지정된) 물체에 부딪혔을 경우에 작동
        if (other.gameObject.CompareTag("Building") || other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Obstacle"))
        {
            // 총알을 제거한다.
            Destroy(gameObject);
        }

        // 총알이 적과 부딪히면 작동하는 함수
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
