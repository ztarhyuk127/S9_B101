using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    Boss boss;
    PhotonView pv;

    [Tooltip("현재 패턴이 진행되는 번호")]
    public int patternIdx = 1;

    [Tooltip("최대 패턴의 갯수")]
    public int maxPatternIdx = 3;

    [Header("Common")]
    [SerializeField]
    private GameObject fireLeftHand;
    [SerializeField]
    private GameObject fireLeftArm;
    [SerializeField]
    private GameObject fireRightHand;
    [SerializeField]
    private GameObject fireRightArm;
    [SerializeField]
    private GameObject fireChest;

    [Header("Roar")]
    [SerializeField]
    private GameObject roarEffect;

    [Header("Smash")]
    [Tooltip("데미지")]
    public float sm_damage;

    [Header("DownwardBlow")]
    [Tooltip("데미지")]
    public float db_damage;
    [Tooltip("왼손의 타격 범위")]
    public float db_leftHandRadius;
    [Tooltip("오른손의 타격 범위")]
    public float db_rightHandRadius;
    [Tooltip("타격 위치에 넣을 폭팔 이펙트")]
    [SerializeField]
    GameObject db_Effect;

    [Header("Breath")]
    [SerializeField]
    GameObject breath_Effect;
    [Tooltip("데미지")]
    public float breath_damage;
    [Tooltip("브레스 공격 시 재생되는 사운드")]
    public AudioClip breathClip;

    [Header("AreaFireExplosion")]
    [Tooltip("데미지")]
    public float AFE_damage;
    [Tooltip("폭팔 게이지 모으는 이펙트")]
    [SerializeField]
    GameObject AFE_Charge_effect;
    [Tooltip("중심에서 터지는 폭팔 이펙트")]
    [SerializeField]
    GameObject AFE_effect_1;
    [Tooltip("중심으로부터 퍼지는 폭팔 이펙트")]
    [SerializeField]
    GameObject AFE_effect_2;

    AudioSource audioSource;
    Animator animator;

    void Awake()
    {
        boss = GetComponent<Boss>();
        pv = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public void RandomPattern(int nowPattern)
    {
        while (patternIdx == nowPattern)
        {
            patternIdx = Random.Range(2, maxPatternIdx + 1);
        }
    }

    private void ApplyDamageInRange(Collider[] colliders, float damage)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            Player player = colliders[i].GetComponent<Player>();
            player.health -= damage;
        }
    }

    private void ApplyCollider(GameObject[] targetObjects, bool active)
    {
        for(int i = 0;i < targetObjects.Length;i++)
        {
            CapsuleCollider collider = targetObjects[i].GetComponent<CapsuleCollider>();
            collider.enabled = active;
        }
    }

    private IEnumerator DestroyEffect(GameObject effect, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(effect);
    }

    [PunRPC]
    public void RoarRPC()
    {
        animator.SetTrigger("Roar");
        StartCoroutine(Roar());
    }

    IEnumerator Roar()
    {
        yield return new WaitForSeconds(3.3f);
        roarEffect.SetActive(true);
        audioSource.Play();

        yield return new WaitForSeconds(5.7f);
        roarEffect.SetActive(false);

        yield return new WaitForSeconds(1f + 0.25f);
        patternIdx = 1;
        boss.nowThink = false;
    }

    [PunRPC]
    public void DownwardBlowRPC()
    {
        animator.SetTrigger("DownwardBlow");
        StartCoroutine(DownwardBlow());
    }

    IEnumerator DownwardBlow()
    {
        yield return new WaitForSeconds(3.92f);

        Collider[] leftColliders = Physics.OverlapSphere(boss.leftHand.transform.position, db_leftHandRadius, LayerMask.GetMask("Player"));
        Collider[] rightColliders = Physics.OverlapSphere(boss.rightHand.transform.position, db_rightHandRadius, LayerMask.GetMask("Player"));

        ApplyDamageInRange(leftColliders, db_damage);
        ApplyDamageInRange(rightColliders, db_damage);

        GameObject effectLeft = Instantiate(db_Effect, boss.leftHand.transform.position, Quaternion.identity);
        GameObject effectRight = Instantiate(db_Effect, boss.rightHand.transform.position, Quaternion.identity);

        StartCoroutine(DestroyEffect(effectLeft, 2.5f));
        StartCoroutine(DestroyEffect(effectRight, 2.5f));

        yield return new WaitForSeconds(3.583f + 0.25f);
        patternIdx = 1;
        boss.nowThink = false;
    }

    [PunRPC]
    public void BreathRPC()
    {
        animator.SetTrigger("Breath");
        StartCoroutine(Breath());
    }

    IEnumerator Breath()
    {
        yield return new WaitForSeconds(3.33f);
        audioSource.clip = breathClip;
        audioSource.Play();
        breath_Effect.SetActive(true);

        yield return new WaitForSeconds(9.17f + 0.25f);
        audioSource.Stop();
        breath_Effect.SetActive(false);
        patternIdx = 1;
        boss.nowThink = false;
    }

    [PunRPC]
    public void SmashRPC()
    {
        animator.SetTrigger("Smash");
        StartCoroutine(Smash());
    }

    IEnumerator Smash()
    {
        GameObject[] targets = new GameObject[14];
        targets[0] = boss.rightDownArm;        targets[1] = boss.rightHand;
        targets[2] = boss.rightIndexFinger_1;  targets[3] = boss.rightIndexFinger_2;  targets[4] = boss.rightIndexFinger_3;
        targets[5] = boss.rightMiddleFinger_1; targets[6] = boss.rightMiddleFinger_2; targets[7] = boss.rightMiddleFinger_3;
        targets[8] = boss.rightRingFinger_1;   targets[9] = boss.rightRingFinger_2;   targets[10] = boss.rightRingFinger_3;
        targets[11] = boss.rightThumbFinger_1; targets[12] = boss.rightThumbFinger_2; targets[13] = boss.rightThumbFinger_3;
        ParticleSystem rightHandParticle = fireRightHand.GetComponent<ParticleSystem>();
        ParticleSystem rightArmParticle = fireRightArm.GetComponent<ParticleSystem>();

        yield return new WaitForSeconds(1.4f);
        rightHandParticle.Play();
        rightArmParticle.Play();
        ApplyCollider(targets, true);

        yield return new WaitForSeconds(1.58f);
        ApplyCollider(targets, false);
        rightHandParticle.Stop();
        rightArmParticle.Stop();

        yield return new WaitForSeconds(2f + 0.25f);
        patternIdx = 1;
        boss.nowThink = false;
    }

    [PunRPC]
    public void AreaFireExplosionRPC()
    {
        animator.SetTrigger("AreaFireExplosion");
        StartCoroutine(AreaFireExplosion());
    }

    IEnumerator AreaFireExplosion()
    {
        GameObject chargeEffect = Instantiate(AFE_Charge_effect, boss.spine.transform.position, Quaternion.identity);
        StartCoroutine(DestroyEffect(chargeEffect, 4.5f));
        yield return new WaitForSeconds(4f);

        GameObject effect1 = Instantiate(AFE_effect_1, boss.spine.transform.position - Vector3.up * 2, Quaternion.identity);
        StartCoroutine(DestroyEffect(effect1, 3.5f));

        GameObject effect2 = Instantiate(AFE_effect_2, boss.spine.transform.position - Vector3.up * 2, Quaternion.identity);
        StartCoroutine(DestroyEffect(effect2, 3.5f));

        Collider[] middleExplosion = Physics.OverlapSphere(boss.spine.transform.position - Vector3.up * 2, 45f, LayerMask.GetMask("Player"));
        ApplyDamageInRange(middleExplosion, AFE_damage);

        yield return new WaitForSeconds(7f + 0.25f);
        patternIdx = 1;
        boss.nowThink = false;
    }
}
