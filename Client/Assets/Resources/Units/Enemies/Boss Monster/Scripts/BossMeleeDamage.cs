using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMeleeDamage : MonoBehaviour
{
    BossAttack bossAttack;
    void Awake()
    {
        bossAttack = GetComponentInParent<BossAttack>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            Animator animator = player.GetComponent<Animator>();
            if (player != null)
            {
                if (!player.isDeath)
                {
                    player.health -= bossAttack.sm_damage;

                    // 만약 체력이 다 달았다면 죽음 애니메이션 재생.
                    if (player.health <= 0)
                    {
                        player.isDeath = true;
                        animator.SetTrigger("Death");
                        StartCoroutine(player.PlayerDeath());
                    }
                    else
                    {
                        // 현재 플레이어가 맞은 애니메이션이 실행되지 않았다면 실행한다.
                        if (!player.isHit)
                        {
                            player.isHit = true;
                            animator.SetTrigger("Hit");
                            StartCoroutine(player.PlayerHit());
                        }
                    }
                }
            }
        }
    }
}
