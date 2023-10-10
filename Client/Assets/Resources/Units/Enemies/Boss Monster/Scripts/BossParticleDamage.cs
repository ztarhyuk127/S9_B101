using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossParticleDamage : MonoBehaviour
{
    BossAttack bossAttack;
    enum ParticleType { Breath, AreaFireExplosion, FirePillar, Meteor };
    [SerializeField]
    ParticleType type;

    void Awake()
    {
        bossAttack = GetComponentInParent<BossAttack>();
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            Animator animator = player.GetComponent<Animator>();
            if (player != null)
            {
                if (!player.isDeath)
                {
                    // 파티클 타입에 따라서 데미지 가하는 방법을 다르게 함.
                    switch (type)
                    {
                        case ParticleType.Breath:
                            player.health -= bossAttack.breath_damage;
                            break;
                        case ParticleType.AreaFireExplosion:
                            player.health -= bossAttack.AFE_damage;
                            break;
                    }

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
