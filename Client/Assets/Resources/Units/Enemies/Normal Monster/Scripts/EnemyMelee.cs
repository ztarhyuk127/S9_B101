using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    public float damage;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            Animator animator = player.GetComponent<Animator>();
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
