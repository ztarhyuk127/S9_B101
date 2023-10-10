using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Photon.Pun;

public class Enemy : MonoBehaviourPun
{
    [Tooltip("몬스터 체력")]
    public float health;
    private float maxHealth;

    [Tooltip("총알을 맞는 위치에 생성되는 이펙트")]
    public GameObject playerBulletEffect;

    Animator animator;
    public bool isDie = false;
    NavMeshAgent navMeshAgent;
    EnemyAttack enemyAttack;
    EnemyDropItem dropItem;

    public GameObject fire;
    private Vector3 fireSize;

    public PhotonView pv;
    private Collider playertHit;

    public AudioClip[] deathClip;
    private AudioSource audioSource;
    public AudioClip[] hitClip;

    void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyAttack = GetComponent<EnemyAttack>();
        dropItem = GetComponent<EnemyDropItem>();

        fireSize = fire.transform.localScale;
        maxHealth = health;

        pv = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        fire.transform.localScale = fireSize * (health / maxHealth);

        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    return;
        //}

        // 몬스터가 죽었다면
        if (isDie)
        {
            // 현 위치로 고정
            navMeshAgent.speed = 0;
            navMeshAgent.angularSpeed = 0;
        }

        // 체력이 0 이하고 공격 중이지 않다면 죽이기.
        if (!isDie && health <= 0)
        {
            // 죽음 판정
            if (PhotonNetwork.IsMasterClient)
            {
                dropItem.ItemCheck(transform.position);
            }

            pv.RPC("DieEnemy", RpcTarget.All);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 플레이어 총알에 맞았을 경우 작동
        if (!isDie && other.gameObject.CompareTag("PlayerBullet"))
        {
            // 총알의 정보를 가져온다.
            PlayerBullet playerBullet = other.GetComponent<PlayerBullet>();

            // 물에 맞은 이펙트를 보여주고 제거한다.
            GameObject bulletEffect = Instantiate(playerBulletEffect, other.transform.position, Quaternion.identity);
            StartCoroutine(DeleteEffect(bulletEffect));

            audioSource.clip = hitClip[Random.Range(0, hitClip.Length)];
            audioSource.Play();

            // 데미지만큼 체력을 깎는다.
            health -= playerBullet.bulletDamage;
            if (health <= 0) health = 0;

            // 총알 객체를 제거한다.
            Destroy(other.gameObject);
        }

        if (!isDie && other.gameObject.CompareTag("PlayerMelee"))
        {
            PlayerAttack playerAttack = other.GetComponentInParent<PlayerAttack>();

            audioSource.clip = hitClip[Random.Range(0, hitClip.Length)];
            audioSource.Play();

            // 데미지만큼 체력을 깎는다.
            health -= playerAttack.attackMeleeDamage;
            if (health <= 0) health = 0;
        }
    }

    // 이펙트 자동 제거
    IEnumerator DeleteEffect(GameObject effectObject)
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(effectObject);
    }

    [PunRPC]
    void DieEnemy()
    {
        isDie = true;
        health = 0;
        StopAllCoroutines();
        // 애니메이션 설정
        animator.SetInteger("Death", Random.Range(1, 7));
        animator.SetTrigger("DieTrigger");

        audioSource.clip = deathClip[Random.Range(0, deathClip.Length)];
        audioSource.Play();
        StartCoroutine(DieEnemyCorutine());
    }

    // 몬스터 죽는 애니메이션 유지 및 오브젝트 제거
    IEnumerator DieEnemyCorutine()
    {
        yield return new WaitForSeconds(9f);
        Destroy(gameObject);
    }
}
