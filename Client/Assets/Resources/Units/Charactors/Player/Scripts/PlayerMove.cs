using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviourPun
{
    [Tooltip("플레이어 움직임 벡터")]
    private Vector3 moveVec;

    [Tooltip("플레이어 움직임 속도")]
    public float moveSpeed = 5f;

    [Tooltip("플레이어 점프 강도")]
    public float jumpForce = 2f;

    [Tooltip("플레이어 추락 강도(기본 중력값)")]
    public float gravity = -9.81f;

    [Tooltip("캐릭터 이동 관련 컴포넌트")]
    CharacterController characterController;

    [Tooltip("가상 카메라를 담는 리스트 변수")]
    [SerializeField]
    private CinemachineVirtualCamera[] virtualCameras;
    // 가상 카메라의 총 갯수
    private int virtualCamerasCount = 6;

    // 모든 가상 카메라를 담는 리스트 변수
    private CinemachineVirtualCamera[] totalVirtualCameras;

    [Tooltip("카메라가 따라다닐 transform 지정")]
    public Transform followTransform;

    // 회전 속도 변수
    private float rotateSpeedX = 2;
    private float rotateSpeedY = 0.8f;

    // 상하 회전 각도 최대치
    private float limitMinX = -60;
    private float limitMaxX = 50;

    // X, Y의 최종 회전 값
    private float eulerAngleX;
    private float eulerAngleY;

    [Tooltip("카메라 회전 값으로 캐릭터 회전을 적용")]
    [SerializeField]
    Transform cameraTransform;

    [Tooltip("조준선 텍스쳐 리스트")]
    public Texture[] crosshair;

    [Tooltip("조준선 이미지 변수")]
    private RawImage crosshairImage;

    [Tooltip("플레이어 애니메이터 설정")]
    private Animator animator;
    [Tooltip("캐릭터 상하 조절용 오브젝트")]
    private Transform spine;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();

        // 애니메이션에서 상체값 가져오기
        animator = GetComponent<Animator>();
        spine = animator.GetBoneTransform(HumanBodyBones.Spine);
    }

    public void initCamera()
    {
        // Crosshair 이미지 등록
        crosshairImage = GameObject.Find("Crosshair").GetComponentInChildren<RawImage>();

        // 플레이어를 추척하는 가상 카메라들을 찾고 전체 가상카메라 리스트에 담기
        totalVirtualCameras = GameObject.FindObjectsOfType<CinemachineVirtualCamera>();
        // 가상 카메라들을 담는 리스트 
        virtualCameras = new CinemachineVirtualCamera[virtualCamerasCount];

        // 카메라 위치 정보를 찾아 등록
        cameraTransform = GameObject.FindObjectOfType<Camera>().transform;

        // 각각의 가상 카메라별로 화면 번호 지정
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

    // 컴퓨터 프레임 단위마다 실행되는 함수
    void Update()
    {
        // 캐릭터가 공중에 있는 경우 중력값을 넣어줌
        if (!characterController.isGrounded)
        {
            moveVec.y += gravity * Time.deltaTime;
            animator.SetBool("isJump", true);
        }
        else
        {
            animator.SetBool("isJump", false);
        }

        // 캐릭터 컨트롤러로 움직임.
        characterController.Move(moveVec * moveSpeed * Time.deltaTime);
    }

    public void MoveTo(Vector3 nVector)
    {
        // 카메라 회전 각을 포함하여 새로운 벡터값을 계산
        Vector3 vec = cameraTransform.rotation * nVector;
        moveVec = new Vector3(vec.x, moveVec.y, vec.z);

        // 이동 애니메이션 구현
        animator.SetFloat("vertical", nVector.z);
        animator.SetFloat("horizontal", nVector.x);
    }

    // Jump RPC 등록
    [PunRPC]
    public void Jump()
    {
        // 캐릭터가 바닥에 붙어있는 경우
        if (characterController.isGrounded)
        {
            moveVec.y = jumpForce;
            // animator.SetTrigger("isJump");
        }
    }

    public void Turn(Vector2 mousePos)
    {
        // 마우스 화면 위치 값 x, y로 나누기
        // 이후 setting 값으로 감도 조절할 예정
        float mouseX = mousePos.x / 10;
        float mouseY = mousePos.y / 10;

        // 마우스의 x, y가 플레이어 움직임의 반대이므로 바꾸어서 넣어준다.
        eulerAngleY += mouseX * rotateSpeedX;
        eulerAngleX -= mouseY * rotateSpeedY;

        eulerAngleX = ClampAngle(eulerAngleX, limitMinX, limitMaxX);

        // 플레이어의 방향 조정
        transform.rotation = Quaternion.Euler(0, eulerAngleY, 0);

        // 애니메이션 상체의 상하 움직임 조정
        spine.localRotation = spine.localRotation * Quaternion.Euler(-eulerAngleX / 4.2f, (eulerAngleX+7.5f) * -0.7f, (eulerAngleX+7.5f) * 0.7f );

        // 카메라 오브젝트의 회전조정
        followTransform.rotation = Quaternion.Euler(eulerAngleX + 14, eulerAngleY, 0);
    }

    // 마우스 오른쪽 클릭시 줌 상태 체크
    public void ZoomCheck()
    {
        /* 
         * MoveToTopOfPrioritySubqueue : 시네머신 가상 카메라들에서 우선순위를 최상으로 높이는 함수
         *                               즉, 이 함수로 시네머신 brain이 여러 가상 카메라들 중 하나를 선택하도록 할 수 있다.
        */

        // 위, 아래 각도(aboveChar.localRotation.z)에 따라 다른 카메라 사용
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

        // 조준선 줌 모드로 변경
        crosshairImage.texture = crosshair[1];
    }

    public void CameraAngleCheck()
    {
        // 조준선 일반 모드로 변경
        crosshairImage.texture = crosshair[0];

        // 위, 아래 각도(aboveChar.localRotation.z)에 따라 다른 카메라 사용
        if (spine.localRotation.z > 0.2)
        {
            // 아래쪽
            virtualCameras[2].MoveToTopOfPrioritySubqueue();
        }
        else if (spine.localRotation.z < -0.02)
        {
            // 위쪽
            virtualCameras[3].MoveToTopOfPrioritySubqueue();
        }
        else
        {
            // 일반 TPS 카메라로 설정
            virtualCameras[0].MoveToTopOfPrioritySubqueue();
        }
    }

    // 0~360 범위의 각만을 기준으로 변환
    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }
}
