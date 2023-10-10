using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Tooltip("scale => 0 ~ 1로 늘어나는 시간 조정")]
    public float maxTime = 2f;
    float curTime = 0f;

    [Tooltip("발사직후 최대 스피드에서 점점 느려지는 초기 스피드")]
    public float shootSpeed;
    [Tooltip("발사직후 최대 스피드에서 점점 느려지는 최종 스피드")]
    public float targetShootSpeed;

    [Tooltip("공격됬는지 확인하는 변수")]
    public bool isShoot = false;
    [Tooltip("공격 방향을 지정하는 벡터")]
    public Vector3 shootDir;

    [Tooltip("지형이나 플레이어에게 닿을 시 이펙트 설정")]
    [SerializeField]
    GameObject boomEffect;

    [Tooltip("발사체의 데미지 설정")]
    public float damage;

    [Tooltip("데미지가 들어갈 범위 지정")]
    public float[] damageRanges;

    private float allDamages = 0f;

    [Tooltip("폭팔시 재생할 사운드 설정")]
    public AudioClip[] damageSounds;

    private AudioSource audioSource;

    public Enemy enemyLogic;

    void Awake()
    {
        // 생성직후 scale 0
        transform.localScale = Vector3.zero;
        for (int i = 0; i < damageRanges.Length; i++)
        {
            allDamages += damageRanges[i];
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = damageSounds[Random.Range(0, damageSounds.Length)];
    }

    void Update()
    {
        if (enemyLogic.isDie)
        {
            curTime -= Time.deltaTime * 2;
            transform.localScale = Vector3.one * (curTime * 1.5f / maxTime);
            if (curTime <= 0.1)
            {
                Destroy(gameObject);
            }

            return;
        }

        // 시간의 흐름에 따라서 크기를 키워준다.
        if (curTime < maxTime)
        {
            transform.localScale = Vector3.one * (curTime * 1.5f / maxTime);
            curTime += Time.deltaTime;
        }
        
        // 발사된 직후 속도를 조정한다.
        if (isShoot && shootSpeed > targetShootSpeed)
        {
            shootSpeed -= Time.deltaTime * 5;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // 지형에 충돌한 경우 또는 플레이어에게 충돌한 경우 폭팔함
        if (other.gameObject.CompareTag("Ground")   || other.gameObject.CompareTag("Obstacle")
         || other.gameObject.CompareTag("Building") || other.gameObject.CompareTag("Player"))
        {
            // 폭팔 이펙트 지정
            GameObject newEffect = Instantiate(boomEffect, transform.position, transform.rotation);

            /*
            데미지 공식 (3개 범위라 한다면)

            직격 => 100% : 50 + 30 + 20
            중간 => 50%  :      30 + 20
            약한 => 20%  :           20
             */

            // 현재 폭팔 범위를 지정한다.
            float nowRange = 0f;
            for (int i = 0; i < damageRanges.Length; i++)
            {
                // 폭팔 범위 크기를 점점 늘려가면서 범위 내의 플레이어를 찾는다.
                nowRange += damageRanges[i];
                Collider[] colliders = Physics.OverlapSphere(transform.position, nowRange, LayerMask.GetMask("Player"));
                for (int j = 0; j < colliders.Length; j++)
                {
                    // 플레이어 정보
                    Player player = colliders[j].GetComponent<Player>();
                    Animator animator = player.GetComponent<Animator>();

                    // 죽지 않은 상태의 플레이어 공격.
                    if (!player.isDeath)
                    {
                        // 데미지 공식에 비례하여 데미지 가함
                        player.health -= damage * (damageRanges[i] / allDamages);

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
            // 폭팔 사운드 재생
            audioSource.Play();

            // 일정시간이 지난 후 이펙트 및 총알제거 제거하기
            StartCoroutine(DestroyEffect(newEffect));
        }
    }

    // 이펙트 및 발사체 제거 코루틴 함수
    IEnumerator DestroyEffect(GameObject effectObject)
    {
        // 사이즈를 0으로 만들어 발사체가 보이지 않게 한다.
        transform.localScale = Vector3.zero;
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.enabled = false;

        // 2.5초 뒤에 모든 것을 제거한다.
        // 최대 폭팔 시간을 2.5초로 설정할 것.
        yield return new WaitForSeconds(2.5f);
        Destroy(effectObject);
        Destroy(gameObject);
    }
}
