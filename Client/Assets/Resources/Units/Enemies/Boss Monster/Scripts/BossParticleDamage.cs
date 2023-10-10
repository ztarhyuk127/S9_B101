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
                    // ��ƼŬ Ÿ�Կ� ���� ������ ���ϴ� ����� �ٸ��� ��.
                    switch (type)
                    {
                        case ParticleType.Breath:
                            player.health -= bossAttack.breath_damage;
                            break;
                        case ParticleType.AreaFireExplosion:
                            player.health -= bossAttack.AFE_damage;
                            break;
                    }

                    // ���� ü���� �� �޾Ҵٸ� ���� �ִϸ��̼� ���.
                    if (player.health <= 0)
                    {
                        player.isDeath = true;
                        animator.SetTrigger("Death");
                        StartCoroutine(player.PlayerDeath());
                    }
                    else
                    {
                        // ���� �÷��̾ ���� �ִϸ��̼��� ������� �ʾҴٸ� �����Ѵ�.
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
