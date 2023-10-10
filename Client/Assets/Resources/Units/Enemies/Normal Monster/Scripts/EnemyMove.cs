using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    Rigidbody rigid;
    Animator animator;

    [Tooltip("이동 속도")]
    public float moveSpeed;

    // NavMeshAgent를 쓰기 위한 컴포넌트를 불러온다.
    private NavMeshAgent navMeshAgent;

    [Tooltip("NavMeshAgent 목적지를 설정해준다.")]
    private Transform targetTransform;

    [Tooltip("NavMeshAgent 처음 위치를 설정해준다.")]
    private Transform spawnTransform;

    [Tooltip("NavMeshAgent 도착거리 기본 설정 값")]
    public float remainingDistance = 1f;

    [Tooltip("플레이어 감지 범위 값 설정")]
    public float targetRadius = 10f;

    [Tooltip("플레이어 공격 범위 값 설정")]
    public float attackRadius = 1.5f;

    // 상태 변수값 설정
    public bool isMoving = false;
    public bool isChasing = false;
    public bool isHit = false;
    EnemyAttack enemyAttack;
    Enemy enemyLogic;

    private AudioSource audioSource;
    public AudioClip[] idleClip;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // navMesh가 움직이기 전 초기 위치 설정
        targetTransform = transform;
        spawnTransform = transform;

        // navMesh 활성화 및 이동속도 지정
        navMeshAgent.enabled = true;
        navMeshAgent.speed = moveSpeed;

        // 멈추는 범위(거리) 설정
        navMeshAgent.stoppingDistance = remainingDistance;

        enemyAttack = GetComponent<EnemyAttack>();
        enemyLogic = GetComponent<Enemy>();

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // 호스트가 아니라면 공격 실행 불가
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        // Update문 실행 전 Start
        Invoke(nameof(IdleMove), Random.Range(0f, 5f));
    }

    void Update()
    {
        // 호스트가 아니라면 공격 실행 불가
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (enemyLogic.isDie)
        {
            isMoving = false;
            isChasing = false;
            enemyAttack.isAttack = false;
            animator.SetBool("Walk", false);
            StopAllCoroutines();
        }

        if (!enemyLogic.isDie && !isHit && !enemyAttack.isAttack)
        {
            if (isChasing && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                isChasing = false;
                enemyAttack.isAttack = true;
                animator.SetBool("Walk", false);

                navMeshAgent.SetDestination(transform.position);
                if (enemyAttack.enemyAttackType == EnemyAttack.EnemyAttackType.Range)
                {
                    StartCoroutine(enemyAttack.AttackRange(targetTransform));
                }
                else
                {
                    StartCoroutine(enemyAttack.AttackMelee());
                }
                navMeshAgent.stoppingDistance = remainingDistance;
            }
            else if (isChasing)
            {
                navMeshAgent.SetDestination(targetTransform.position);
            }
            else if (!isChasing)
            {
                // 움직이는 상태에서 도착지점에 가까워지면 멈춘다.
                if (isMoving && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                {
                    isMoving = false;
                    animator.SetBool("Walk", false);
                    StartCoroutine(ReturnPostion());
                }
                Targeting();
            }
        }
    }

    // 1초에 50회의 고정 업데이트를 보장하는 함수.
    void FixedUpdate()
    {
        if (!navMeshAgent.isActiveAndEnabled)
        {
            FixMoveInNav();
        }
    }

    // NavMesh를 사용하는 경우, 반드시 FixedUpdate에 넣어주어야 함.
    // 이는 자동으로 목적지를 향해 가는 도중에 유닛이 스스로 움직이는 것을 방지함.
    void FixMoveInNav()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    public void IdleMove()
    {
        if (!enemyLogic.isDie)
        {
            isMoving = true;
            animator.SetBool("Walk", true);
            navMeshAgent.SetDestination(transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f)));
        }
    }

    IEnumerator ReturnPostion()
    {
        yield return new WaitForSeconds(Random.Range(1f, 10f));
        if (!enemyAttack.isAttack)
        {
            audioSource.clip = idleClip[Random.Range(0, idleClip.Length)];
            audioSource.Play();
            IdleMove();
        }
    }

    void Targeting()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, targetRadius, LayerMask.GetMask("Player"));
        if (colliders.Length > 0)
        {
            isMoving = false;
            isChasing = true;
            animator.SetBool("Walk", true);
            int randomTarget = Random.Range(0, colliders.Length - 1);
            targetTransform = colliders[randomTarget].transform;

            navMeshAgent.stoppingDistance = attackRadius;
            navMeshAgent.SetDestination(targetTransform.position);
            StopCoroutine(ReturnPostion());
        }
    }

    public void ChasePlayer(GameObject player, GameObject Bullet)
    {
        if (!isHit && !enemyAttack.isAttack && !enemyLogic.isDie)
        {
            isMoving = false;
            isHit = true;
            StopCoroutine(ReturnPostion());
            navMeshAgent.stoppingDistance = attackRadius;

            PlayerBullet playerBullet = Bullet.GetComponent<PlayerBullet>();

            switch (playerBullet.bulletType)
            {
                case PlayerBullet.BulletType.Water:
                    animator.SetTrigger("Hit");
                    StartCoroutine(ChaseStart(player, navMeshAgent.speed, navMeshAgent.angularSpeed, 1f));
                    navMeshAgent.speed = 0;
                    navMeshAgent.angularSpeed = 0;
                    break;
                case PlayerBullet.BulletType.WaterSpray:
                    StartCoroutine(ChaseStart(player, navMeshAgent.speed, navMeshAgent.angularSpeed, 0f));
                    break;
            }
        }
    }

    IEnumerator ChaseStart(GameObject player, float speed, float angularSpeed, float time)
    {
        yield return new WaitForSeconds(time);
        navMeshAgent.speed = speed;
        navMeshAgent.angularSpeed = angularSpeed;

        isHit = false;
        isChasing = true;
        animator.SetBool("Walk", true);
        targetTransform = player.transform;
        navMeshAgent.SetDestination(targetTransform.position);
    }
}
