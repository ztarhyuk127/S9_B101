using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    Animator animator;
    EnemyMove enemyMove;
    NavMeshAgent navMeshAgent;

    // ���� Ÿ�� ������ ���� Ÿ�� ����
    public enum EnemyAttackType { Melee, Range };

    [Tooltip("���� ���� Ÿ�� ����")]
    public EnemyAttackType enemyAttackType;

    [Tooltip("���� ����ü ����")]
    [SerializeField]
    private GameObject projectile;

    [Tooltip("����ü ��ġ ����")]
    [SerializeField]
    private Transform projectilePos;

    [Tooltip("����ü �ӵ� ����")]
    [SerializeField]
    private float projectileSpeed = 1f;

    [Tooltip("���Ÿ� �ִϸ��̼� �ð� ����")]
    public float animationSpeed = 2f;

    [Tooltip("���� ���Ͱ� ���������� Ȯ���ϴ� ����")]
    public bool isAttack = false;

    private Vector3 nowTarget;
    private GameObject new_projectile;
    private EnemyProjectile projectileLogic;
    private PhotonView pv;
    public AudioSource attackAudio;
    public AudioSource shootAudio;
    public AudioClip[] attackMeleeClip;

    void Awake()
    {
        animator = GetComponent<Animator>();
        enemyMove = GetComponent<EnemyMove>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        pv = GetComponent<PhotonView>();
    }

    public IEnumerator AttackRange(Transform target)
    {
        transform.LookAt(target.position);
        animator.SetTrigger("Attack3");
        attackAudio.Play();

        pv.RPC("InstantiateProjectile", RpcTarget.All, target.position);

        float speed = navMeshAgent.speed;
        float angularSpeed = navMeshAgent.angularSpeed;
        navMeshAgent.speed = 0;
        navMeshAgent.angularSpeed = 0;

        yield return new WaitForSeconds(animationSpeed);

        shootAudio.Play();
        pv.RPC("ShootProjectile", RpcTarget.All);

        yield return new WaitForSeconds(0.5f);

        navMeshAgent.speed = speed;
        navMeshAgent.angularSpeed = angularSpeed;

        isAttack = false;
        enemyMove.isChasing = false;
        enemyMove.IdleMove();
    }

    [PunRPC]
    public void InstantiateProjectile(Vector3 pos)
    {
        nowTarget = pos;
        new_projectile = Instantiate(projectile, projectilePos.position, projectilePos.rotation);
        projectileLogic = new_projectile.GetComponent<EnemyProjectile>();
        projectileLogic.shootSpeed = projectileSpeed;
        projectileLogic.targetShootSpeed = (projectileSpeed - 15f) > 1 ? (projectileSpeed - 15f) : 1f;
        projectileLogic.enemyLogic = gameObject.GetComponent<Enemy>();
    }

    [PunRPC]
    public void ShootProjectile()
    {
        projectileLogic.shootDir = (nowTarget + Vector3.up * Random.Range(0f, 2f) - projectilePos.position).normalized;
        new_projectile.GetComponent<Rigidbody>().velocity = projectileLogic.shootDir * projectileSpeed;
        projectileLogic.isShoot = true;
    }

    public IEnumerator AttackMelee()
    {
        animator.SetInteger("AttackType", Random.Range(0, 2));
        animator.SetTrigger("Attack");
        attackAudio.clip = attackMeleeClip[Random.Range(0, attackMeleeClip.Length)];
        attackAudio.Play();

        float speed = navMeshAgent.speed;
        float angularSpeed = navMeshAgent.angularSpeed;
        navMeshAgent.speed = 0;
        navMeshAgent.angularSpeed = 0;

        yield return new WaitForSeconds(4f);

        navMeshAgent.speed = speed;
        navMeshAgent.angularSpeed = angularSpeed;

        isAttack = false;
        enemyMove.isChasing = false;
        enemyMove.IdleMove();
    }
}
