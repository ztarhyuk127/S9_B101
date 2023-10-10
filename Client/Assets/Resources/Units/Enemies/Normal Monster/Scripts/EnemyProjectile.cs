using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Tooltip("scale => 0 ~ 1�� �þ�� �ð� ����")]
    public float maxTime = 2f;
    float curTime = 0f;

    [Tooltip("�߻����� �ִ� ���ǵ忡�� ���� �������� �ʱ� ���ǵ�")]
    public float shootSpeed;
    [Tooltip("�߻����� �ִ� ���ǵ忡�� ���� �������� ���� ���ǵ�")]
    public float targetShootSpeed;

    [Tooltip("���݉���� Ȯ���ϴ� ����")]
    public bool isShoot = false;
    [Tooltip("���� ������ �����ϴ� ����")]
    public Vector3 shootDir;

    [Tooltip("�����̳� �÷��̾�� ���� �� ����Ʈ ����")]
    [SerializeField]
    GameObject boomEffect;

    [Tooltip("�߻�ü�� ������ ����")]
    public float damage;

    [Tooltip("�������� �� ���� ����")]
    public float[] damageRanges;

    private float allDamages = 0f;

    [Tooltip("���Ƚ� ����� ���� ����")]
    public AudioClip[] damageSounds;

    private AudioSource audioSource;

    public Enemy enemyLogic;

    void Awake()
    {
        // �������� scale 0
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

        // �ð��� �帧�� ���� ũ�⸦ Ű���ش�.
        if (curTime < maxTime)
        {
            transform.localScale = Vector3.one * (curTime * 1.5f / maxTime);
            curTime += Time.deltaTime;
        }
        
        // �߻�� ���� �ӵ��� �����Ѵ�.
        if (isShoot && shootSpeed > targetShootSpeed)
        {
            shootSpeed -= Time.deltaTime * 5;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ������ �浹�� ��� �Ǵ� �÷��̾�� �浹�� ��� ������
        if (other.gameObject.CompareTag("Ground")   || other.gameObject.CompareTag("Obstacle")
         || other.gameObject.CompareTag("Building") || other.gameObject.CompareTag("Player"))
        {
            // ���� ����Ʈ ����
            GameObject newEffect = Instantiate(boomEffect, transform.position, transform.rotation);

            /*
            ������ ���� (3�� ������ �Ѵٸ�)

            ���� => 100% : 50 + 30 + 20
            �߰� => 50%  :      30 + 20
            ���� => 20%  :           20
             */

            // ���� ���� ������ �����Ѵ�.
            float nowRange = 0f;
            for (int i = 0; i < damageRanges.Length; i++)
            {
                // ���� ���� ũ�⸦ ���� �÷����鼭 ���� ���� �÷��̾ ã�´�.
                nowRange += damageRanges[i];
                Collider[] colliders = Physics.OverlapSphere(transform.position, nowRange, LayerMask.GetMask("Player"));
                for (int j = 0; j < colliders.Length; j++)
                {
                    // �÷��̾� ����
                    Player player = colliders[j].GetComponent<Player>();
                    Animator animator = player.GetComponent<Animator>();

                    // ���� ���� ������ �÷��̾� ����.
                    if (!player.isDeath)
                    {
                        // ������ ���Ŀ� ����Ͽ� ������ ����
                        player.health -= damage * (damageRanges[i] / allDamages);

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
            // ���� ���� ���
            audioSource.Play();

            // �����ð��� ���� �� ����Ʈ �� �Ѿ����� �����ϱ�
            StartCoroutine(DestroyEffect(newEffect));
        }
    }

    // ����Ʈ �� �߻�ü ���� �ڷ�ƾ �Լ�
    IEnumerator DestroyEffect(GameObject effectObject)
    {
        // ����� 0���� ����� �߻�ü�� ������ �ʰ� �Ѵ�.
        transform.localScale = Vector3.zero;
        SphereCollider collider = GetComponent<SphereCollider>();
        collider.enabled = false;

        // 2.5�� �ڿ� ��� ���� �����Ѵ�.
        // �ִ� ���� �ð��� 2.5�ʷ� ������ ��.
        yield return new WaitForSeconds(2.5f);
        Destroy(effectObject);
        Destroy(gameObject);
    }
}
