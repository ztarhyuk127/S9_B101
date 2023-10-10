using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviour
{
    // 플레이어 공격 타입 변수
    public enum AttackType { Melee, Range, Gauge }
    [Tooltip("플레이어 공격 타입 지정")]
    public AttackType attackType;

    [Header("근거리 공격 설정")]
    [Tooltip("근거리 공격 범위")]
    public BoxCollider attackArea;
    [Tooltip("근거리 공격 데미지")]
    public float attackMeleeDamage;

    [Header("원거리 공격 설정")]
    [Tooltip("총알이 발사될 기준 위치")]
    public Transform bulletPos;
    [Tooltip("총알 프리팹")]
    public GameObject bullet;
    [Tooltip("총알 속도")]
    public float bulletSpeed;
    [Tooltip("총알이 발사되는 위치에 생성되는 이펙트")]
    public GameObject bulletEffect;
    [Tooltip("총알 최대 장전량")]
    public int maxBulletValue = 30;
    [Tooltip("총알 현재 남은량")]
    public int curBulletValue = 30;

    [Header("게이지 공격 설정")]
    [Tooltip("게이지 최대량")]
    public int maxGauge = 200;
    [Tooltip("게이지 현재 보유량")]
    public int curGauge = 200;
    [Tooltip("게이지 충전 속도")]
    public float gaugeChargeSpeed = 0.2f;

    [Tooltip("총알이 향할 위치를 지정하기 위한 카메라")]
    Camera mainCamera;
    Player player;

    public PhotonView pv;

    void Awake()
    {
        mainCamera = Camera.main;
        player = GetComponent<Player>();

        pv = GetComponent<PhotonView>();
    }

    // 캐릭터가 발사 버튼을 눌렀을 때 실행되는 함수
    public void OnAttack(bool isZoom)
    {
        // 메인카메라가 존재할 경우 실행한다.
        if (mainCamera != null)
        {
            // 카메라의 위치와 방향을 얻는다.
            Vector3 cameraPosition = mainCamera.transform.position;
            Vector3 cameraForward = mainCamera.transform.forward;

            // 카메라의 정중앙을 가리키는 Ray를 생성한다.
            Ray centerRay = new Ray(cameraPosition + mainCamera.transform.forward * 5, cameraForward);
            RaycastHit hitInfo;

            // 특정 레이어를 무시하는 LayerMask를 생성한다.
            int playerLayerMask = 1 << LayerMask.NameToLayer("Player");
            int playerBulletLayerMask = 1 << LayerMask.NameToLayer("PlayerBullet");
            int layerMask = ~(playerLayerMask | playerBulletLayerMask);

            // Ray가 충돌하는 정보를 가져온다.
            if (Physics.Raycast(centerRay, out hitInfo, Mathf.Infinity, layerMask))
            {
                pv.RPC("BulletDraw", RpcTarget.All, hitInfo.point, isZoom);
            }
        }
    }

    [PunRPC]
    void BulletDraw(Vector3 point, bool isZoom)
    {
        // 새로운 총알을 생성한다.
        GameObject bulletCreate = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = bulletCreate.GetComponent<Rigidbody>();
        PlayerBullet bulletLogic = bulletCreate.GetComponent<PlayerBullet>();
        bulletLogic.attackPlayer = gameObject;

        // 총알 갯수 감소 및 데미지 증가량 적용
        switch (attackType)
        {
            case AttackType.Range:
                curBulletValue -= 1;
                bulletLogic.bulletDamage += player.attackpower * 0.3f;
                break;
            case AttackType.Gauge:
                curGauge -= 1;
                bulletLogic.bulletDamage += player.attackpower * 0.1f;
                break;
        }

        // 총알이 발사되는 방향을 설정한다.
        // 이때, 줌인 경우와 아닌 경우를 나누고, 줌 상태에서는 정확하게, 아닌 경우에는 총알이 튀는 형식으로 발사된다.
        Vector3 bulletVec = (point - bulletPos.position).normalized * bulletSpeed
            + (isZoom ? Vector3.zero : new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)));
        bulletRigid.AddForce(bulletVec, ForceMode.Impulse);

        // 이펙트 생성
        GameObject effectObject = Instantiate(bulletEffect, point, Quaternion.identity);
        StartCoroutine(DeleteEffect(effectObject));

        // 보류중
        // coroutine을 설정하여 일정시간이 지난 뒤에 총알을 삭제한다.
        StartCoroutine(DestroyBullet(bulletCreate));
    }

    public IEnumerator OnAttackMelee(AudioSource audioSource)
    {
        attackArea.enabled = true;
        yield return new WaitForSeconds(1.2f);
        audioSource.Play();

        yield return new WaitForSeconds(1.5f);
        attackArea.enabled = false;
    }

    // 이펙트 자동 제거
    IEnumerator DeleteEffect(GameObject effectObject)
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(effectObject);
    }

    // 총알이 사라지는 코루틴 함수
    IEnumerator DestroyBullet(GameObject targetBullet)
    {
        yield return new WaitForSeconds(5f);
        Destroy(targetBullet);
    }
}
