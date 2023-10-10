using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireParticleDamage : MonoBehaviour
{
    public float damage = 0f;

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
                    player.health -= damage;

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
