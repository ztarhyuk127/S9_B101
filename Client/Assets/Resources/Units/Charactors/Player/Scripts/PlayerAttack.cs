using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAttack : MonoBehaviour
{
    // �÷��̾� ���� Ÿ�� ����
    public enum AttackType { Melee, Range, Gauge }
    [Tooltip("�÷��̾� ���� Ÿ�� ����")]
    public AttackType attackType;

    [Header("�ٰŸ� ���� ����")]
    [Tooltip("�ٰŸ� ���� ����")]
    public BoxCollider attackArea;
    [Tooltip("�ٰŸ� ���� ������")]
    public float attackMeleeDamage;

    [Header("���Ÿ� ���� ����")]
    [Tooltip("�Ѿ��� �߻�� ���� ��ġ")]
    public Transform bulletPos;
    [Tooltip("�Ѿ� ������")]
    public GameObject bullet;
    [Tooltip("�Ѿ� �ӵ�")]
    public float bulletSpeed;
    [Tooltip("�Ѿ��� �߻�Ǵ� ��ġ�� �����Ǵ� ����Ʈ")]
    public GameObject bulletEffect;
    [Tooltip("�Ѿ� �ִ� ������")]
    public int maxBulletValue = 30;
    [Tooltip("�Ѿ� ���� ������")]
    public int curBulletValue = 30;

    [Header("������ ���� ����")]
    [Tooltip("������ �ִ뷮")]
    public int maxGauge = 200;
    [Tooltip("������ ���� ������")]
    public int curGauge = 200;
    [Tooltip("������ ���� �ӵ�")]
    public float gaugeChargeSpeed = 0.2f;

    [Tooltip("�Ѿ��� ���� ��ġ�� �����ϱ� ���� ī�޶�")]
    Camera mainCamera;
    Player player;

    public PhotonView pv;

    void Awake()
    {
        mainCamera = Camera.main;
        player = GetComponent<Player>();

        pv = GetComponent<PhotonView>();
    }

    // ĳ���Ͱ� �߻� ��ư�� ������ �� ����Ǵ� �Լ�
    public void OnAttack(bool isZoom)
    {
        // ����ī�޶� ������ ��� �����Ѵ�.
        if (mainCamera != null)
        {
            // ī�޶��� ��ġ�� ������ ��´�.
            Vector3 cameraPosition = mainCamera.transform.position;
            Vector3 cameraForward = mainCamera.transform.forward;

            // ī�޶��� ���߾��� ����Ű�� Ray�� �����Ѵ�.
            Ray centerRay = new Ray(cameraPosition + mainCamera.transform.forward * 5, cameraForward);
            RaycastHit hitInfo;

            // Ư�� ���̾ �����ϴ� LayerMask�� �����Ѵ�.
            int playerLayerMask = 1 << LayerMask.NameToLayer("Player");
            int playerBulletLayerMask = 1 << LayerMask.NameToLayer("PlayerBullet");
            int layerMask = ~(playerLayerMask | playerBulletLayerMask);

            // Ray�� �浹�ϴ� ������ �����´�.
            if (Physics.Raycast(centerRay, out hitInfo, Mathf.Infinity, layerMask))
            {
                pv.RPC("BulletDraw", RpcTarget.All, hitInfo.point, isZoom);
            }
        }
    }

    [PunRPC]
    void BulletDraw(Vector3 point, bool isZoom)
    {
        // ���ο� �Ѿ��� �����Ѵ�.
        GameObject bulletCreate = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = bulletCreate.GetComponent<Rigidbody>();
        PlayerBullet bulletLogic = bulletCreate.GetComponent<PlayerBullet>();
        bulletLogic.attackPlayer = gameObject;

        // �Ѿ� ���� ���� �� ������ ������ ����
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

        // �Ѿ��� �߻�Ǵ� ������ �����Ѵ�.
        // �̶�, ���� ���� �ƴ� ��츦 ������, �� ���¿����� ��Ȯ�ϰ�, �ƴ� ��쿡�� �Ѿ��� Ƣ�� �������� �߻�ȴ�.
        Vector3 bulletVec = (point - bulletPos.position).normalized * bulletSpeed
            + (isZoom ? Vector3.zero : new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)));
        bulletRigid.AddForce(bulletVec, ForceMode.Impulse);

        // ����Ʈ ����
        GameObject effectObject = Instantiate(bulletEffect, point, Quaternion.identity);
        StartCoroutine(DeleteEffect(effectObject));

        // ������
        // coroutine�� �����Ͽ� �����ð��� ���� �ڿ� �Ѿ��� �����Ѵ�.
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

    // ����Ʈ �ڵ� ����
    IEnumerator DeleteEffect(GameObject effectObject)
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(effectObject);
    }

    // �Ѿ��� ������� �ڷ�ƾ �Լ�
    IEnumerator DestroyBullet(GameObject targetBullet)
    {
        yield return new WaitForSeconds(5f);
        Destroy(targetBullet);
    }
}
