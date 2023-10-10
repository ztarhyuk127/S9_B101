using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviourPun
{
    [Tooltip("�÷��̾� ������ ����")]
    private Vector3 moveVec;

    [Tooltip("�÷��̾� ������ �ӵ�")]
    public float moveSpeed = 5f;

    [Tooltip("�÷��̾� ���� ����")]
    public float jumpForce = 2f;

    [Tooltip("�÷��̾� �߶� ����(�⺻ �߷°�)")]
    public float gravity = -9.81f;

    [Tooltip("ĳ���� �̵� ���� ������Ʈ")]
    CharacterController characterController;

    [Tooltip("���� ī�޶� ��� ����Ʈ ����")]
    [SerializeField]
    private CinemachineVirtualCamera[] virtualCameras;
    // ���� ī�޶��� �� ����
    private int virtualCamerasCount = 6;

    // ��� ���� ī�޶� ��� ����Ʈ ����
    private CinemachineVirtualCamera[] totalVirtualCameras;

    [Tooltip("ī�޶� ����ٴ� transform ����")]
    public Transform followTransform;

    // ȸ�� �ӵ� ����
    private float rotateSpeedX = 2;
    private float rotateSpeedY = 0.8f;

    // ���� ȸ�� ���� �ִ�ġ
    private float limitMinX = -60;
    private float limitMaxX = 50;

    // X, Y�� ���� ȸ�� ��
    private float eulerAngleX;
    private float eulerAngleY;

    [Tooltip("ī�޶� ȸ�� ������ ĳ���� ȸ���� ����")]
    [SerializeField]
    Transform cameraTransform;

    [Tooltip("���ؼ� �ؽ��� ����Ʈ")]
    public Texture[] crosshair;

    [Tooltip("���ؼ� �̹��� ����")]
    private RawImage crosshairImage;

    [Tooltip("�÷��̾� �ִϸ����� ����")]
    private Animator animator;
    [Tooltip("ĳ���� ���� ������ ������Ʈ")]
    private Transform spine;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        // �ִϸ��̼ǿ��� ��ü�� ��������
        animator = GetComponent<Animator>();
        spine = animator.GetBoneTransform(HumanBodyBones.Spine);
    }

    public void initCamera()
    {
        // Crosshair �̹��� ���
        crosshairImage = GameObject.Find("Crosshair").GetComponentInChildren<RawImage>();

        // �÷��̾ ��ô�ϴ� ���� ī�޶���� ã�� ��ü ����ī�޶� ����Ʈ�� ���
        totalVirtualCameras = GameObject.FindObjectsOfType<CinemachineVirtualCamera>();
        // ���� ī�޶���� ��� ����Ʈ 
        virtualCameras = new CinemachineVirtualCamera[virtualCamerasCount];

        // ī�޶� ��ġ ������ ã�� ���
        cameraTransform = GameObject.FindObjectOfType<Camera>().transform;

        // ������ ���� ī�޶󺰷� ȭ�� ��ȣ ����
        foreach (var cam in totalVirtualCameras)
        {
            if (cam.Name == "Player Follow Camera")
                virtualCameras[0] = cam;
            else if (cam.Name == "Zoom Camera")
                virtualCameras[1] = cam;
            else if (cam.Name == "Player Follow Down Camera")
                virtualCameras[2] = cam;
            else if (cam.Name == "Player Follow Up Camera")
                virtualCameras[3] = cam;
            else if (cam.Name == "Zoom Up Camera")
                virtualCameras[4] = cam;
            else if (cam.Name == "Zoom Down Camera")
                virtualCameras[5] = cam;
            cam.Follow = followTransform;
            cam.LookAt = followTransform;
        }
    }

    // ��ǻ�� ������ �������� ����Ǵ� �Լ�
    void Update()
    {
        // ĳ���Ͱ� ���߿� �ִ� ��� �߷°��� �־���
        if (!characterController.isGrounded)
        {
            moveVec.y += gravity * Time.deltaTime;
            animator.SetBool("isJump", true);
        }
        else
        {
            animator.SetBool("isJump", false);
        }

        // ĳ���� ��Ʈ�ѷ��� ������.
        characterController.Move(moveVec * moveSpeed * Time.deltaTime);
    }

    public void MoveTo(Vector3 nVector)
    {
        // ī�޶� ȸ�� ���� �����Ͽ� ���ο� ���Ͱ��� ���
        Vector3 vec = cameraTransform.rotation * nVector;
        moveVec = new Vector3(vec.x, moveVec.y, vec.z);

        // �̵� �ִϸ��̼� ����
        animator.SetFloat("vertical", nVector.z);
        animator.SetFloat("horizontal", nVector.x);
    }

    // Jump RPC ���
    [PunRPC]
    public void Jump()
    {
        // ĳ���Ͱ� �ٴڿ� �پ��ִ� ���
        if (characterController.isGrounded)
        {
            moveVec.y = jumpForce;
            // animator.SetTrigger("isJump");
        }
    }

    public void Turn(Vector2 mousePos)
    {
        // ���콺 ȭ�� ��ġ �� x, y�� ������
        // ���� setting ������ ���� ������ ����
        float mouseX = mousePos.x / 10;
        float mouseY = mousePos.y / 10;

        // ���콺�� x, y�� �÷��̾� �������� �ݴ��̹Ƿ� �ٲپ �־��ش�.
        eulerAngleY += mouseX * rotateSpeedX;
        eulerAngleX -= mouseY * rotateSpeedY;

        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        // �÷��̾��� ���� ����
        transform.rotation = Quaternion.Euler(0, eulerAngleY, 0);

        // �ִϸ��̼� ��ü�� ���� ������ ����
        spine.localRotation = spine.localRotation * Quaternion.Euler(-eulerAngleX / 4.2f, (eulerAngleX+7.5f) * -0.7f, (eulerAngleX+7.5f) * 0.7f );

        // ī�޶� ������Ʈ�� ȸ������
        followTransform.rotation = Quaternion.Euler(eulerAngleX + 14, eulerAngleY, 0);
    }

    // ���콺 ������ Ŭ���� �� ���� üũ
    public void ZoomCheck()
    {
        /* 
         * MoveToTopOfPrioritySubqueue : �ó׸ӽ� ���� ī�޶�鿡�� �켱������ �ֻ����� ���̴� �Լ�
         *                               ��, �� �Լ��� �ó׸ӽ� brain�� ���� ���� ī�޶�� �� �ϳ��� �����ϵ��� �� �� �ִ�.
        */

        // ��, �Ʒ� ����(aboveChar.localRotation.z)�� ���� �ٸ� ī�޶� ���
        if (spine.localRotation.z < -0.02)
        {
            virtualCameras[4].MoveToTopOfPrioritySubqueue();
        }
        else if (spine.localRotation.z > 0.3)
        {
            virtualCameras[5].MoveToTopOfPrioritySubqueue();
        }
        else
        {
            virtualCameras[1].MoveToTopOfPrioritySubqueue();
        }

        // ���ؼ� �� ���� ����
        crosshairImage.texture = crosshair[1];
    }

    public void CameraAngleCheck()
    {
        // ���ؼ� �Ϲ� ���� ����
        crosshairImage.texture = crosshair[0];

        // ��, �Ʒ� ����(aboveChar.localRotation.z)�� ���� �ٸ� ī�޶� ���
        if (spine.localRotation.z > 0.2)
        {
            // �Ʒ���
            virtualCameras[2].MoveToTopOfPrioritySubqueue();
        }
        else if (spine.localRotation.z < -0.02)
        {
            // ����
            virtualCameras[3].MoveToTopOfPrioritySubqueue();
        }
        else
        {
            // �Ϲ� TPS ī�޶�� ����
            virtualCameras[0].MoveToTopOfPrioritySubqueue();
        }
    }

    // 0~360 ������ ������ �������� ��ȯ
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
