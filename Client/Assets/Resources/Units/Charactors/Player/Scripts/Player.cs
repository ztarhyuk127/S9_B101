using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ItemName;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class Player : MonoBehaviourPun, IPunObservable
{
    [Tooltip("플레이어 움직임 관련 스크립트")]
    [SerializeField]
    private PlayerMove playerMove;
    [Tooltip("방향키를 눌렀는지 확인하는 변수")]
    Vector3 moveVec;
    [Tooltip("spacebar를 눌렀는지 확인하는 변수")]
    bool inputJump;
    [Tooltip("플레이어가 줌 상태인지 확인하는 변수")]
    private bool inputZoom;
    [Tooltip("화면의 마우스의 위치를 받는 변수")]
    private Vector2 mousePos = Vector2.zero;

    [Tooltip("플레이어 공격 관련 스크립트")]
    [SerializeField]
    public PlayerAttack playerAttack;
    [Tooltip("플레이어가 공격 버튼을 눌렀는지 확인하는 변수")]
    private bool inputFire;
    [Tooltip("플레이어 공격 후 현재 시간. maxFireTime 보다 커지면 공격 가능")]
    private float curFireTime = 0f;
    [Tooltip("플레이어 공격속도")]
    [SerializeField]
    private float maxFireTime = 0f;
    [Tooltip("플레이어 게이지형 차지 시간 변수")]
    private float curChargeTime = 0f;

    [Tooltip("애니메이터 기능 변수")]
    Animator animator;

    // Input System 관련 액션 변수들
    [Tooltip("플레이어 New 인풋 시스템 변수")]
    PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction fireAction;
    private InputAction zoomAction;
    private InputAction jumpAction;
    private InputAction reloadAction;
    private InputAction interactionAction;
    private InputAction lookControllerAction;

    // Photon 관련 변수들
    [Tooltip("Photon View 변수")]
    public PhotonView pv;

    // 원거리 캐릭터가 장전중임을 나타내는 변수
    public bool reloading = false;

    [Tooltip("플레이어의 체력 설정")]
    public float health = 100f;

    [Tooltip("플레이어의 최대 체력 설정")]
    public float maxHealth = 100f;

    [Tooltip("플레이어가 맞았는지 확인하는 변수")]
    public bool isHit = false;

    // 플레이어가 죽었는지 확인하는 변수
    public bool isDeath = false;

    [Tooltip("플레이어가 구조자/아이템에 진입했을 때의 게임오브젝트를 저장하기 위한 변수")]
    public GameObject nearObject;

    public bool isShop = false;

    bool isEsc = false; 

    [Tooltip("플레이어 아이템 관련 배열")]
    public GameObject[] Items;

    [Tooltip("플레이어가 아이템을 습득했는지 여부를 알려주는 배열")]
    public bool[] hasItems;

    // UI에 표시하기 위한 아이템 배열
    public int[] countItemUIAmount = { 0, 0, 0 };

    public float attackpower = 0f;

    [Header("음원")]
    [Tooltip("플레이어 발소리")]
    public AudioSource[] footAudioSource;
    [Tooltip("플레이어 숨소리")]
    public AudioSource breathAudioSource;
    [Tooltip("플레이어 장전소리")]
    public AudioSource[] reloadAudioSource;
    [Tooltip("플레이어 공격소리")]
    public AudioSource attackAudioSource;
    [Tooltip("플레이어 피격소리")]
    public AudioSource hitAudioSource;

    public PlayerStatusUI _playerStatusUI;

    // 이 함수에서 컴포넌트가 생성될 때 불러올 것들을 작성합니다.
    void Awake()
    {
        // 포톤 뷰 등록
        pv = GetComponent<PhotonView>();

        // 애니메이터 불러오기
        animator = GetComponent<Animator>();

        // New Input System을 사용하기 위한 설정
        playerInput = GetComponent<PlayerInput>();
        moveAction   = playerInput.actions["Move"];
        lookAction   = playerInput.actions["Look"];
        fireAction   = playerInput.actions["Fire"];
        zoomAction   = playerInput.actions["Zoom"];
        jumpAction   = playerInput.actions["Jump"];
        reloadAction = playerInput.actions["Reload"];
        interactionAction = playerInput.actions["Interaction"];
        lookControllerAction = playerInput.actions["LookController"];

        // 씬이 로드될 때마다 실행되는 initCamera. 최초로 캐릭터 생성시에도 실행.
        if (pv.IsMine)
        {
            playerMove.initCamera();
        }

        // 오브젝트 삭제 금지
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        if (pv.IsMine)
        {
            playerMove.initCamera();
        }
    }
    void Update()
    {
        if (transform.position.y <= -3f)
        {
            GameObject spawnGroup = GameObject.Find("SpawnGroup");
            if (spawnGroup != null)
            {
                Transform[] points = spawnGroup.GetComponentsInChildren<Transform>();
                int idx = Random.Range(1, points.Length);
                CharacterController characterController = GetComponent<CharacterController>();
                characterController.enabled = false;
                transform.position = points[idx].position;
                characterController.enabled = true;
            }
        }

        // 멀티플레이에서 내 캐릭터일때만 동작하게 함.
        if (pv.IsMine)
        {
            // 만약 죽은 상태라면 아래의 모든 함수 실행 차단.
            if (isDeath) 
            {
                mousePos = Vector3.zero;
                playerMove.MoveTo(Vector3.zero);
                return;
            }

            GetInput();

            // 캐릭터 움직임 함수 호출
            playerMove.MoveTo(inputZoom ? moveVec / 4 : moveVec);

            if (moveVec.x != 0 || moveVec.z != 0)
            {
                for (int i = 0; i < footAudioSource.Length; i++)
                {
                    if (!footAudioSource[i].isPlaying)
                    {
                        footAudioSource[i].Play();
                    }
                }
            }

            // 점프 버튼을 누르면 점프
            if (inputJump)
            {
                //playerMove.Jump();
                // Jump RPC 등록
                pv.RPC("Jump", RpcTarget.All);
            }

            // 줌 버튼을 누르면 줌, 아니면 일반 TPS 카메라 설정
            // 이때 근접공격 캐릭터는 줌을 할 수 없게 함.
            if (inputZoom && playerAttack.attackType != PlayerAttack.AttackType.Melee)
            {
                playerMove.ZoomCheck();
            }
            else
            {
                playerMove.CameraAngleCheck();
            }

            // 공격 시간 내에 공격 버튼을 누른 경우(계속 누르고 있는 경우 포함)
            if (playerAttack.attackType == PlayerAttack.AttackType.Range && inputFire && curFireTime > maxFireTime)
            {
                if (playerAttack.curBulletValue > 0 && !reloading)
                {
                    playerAttack.OnAttack(inputZoom);
                    //pv.RPC("OnAttack", RpcTarget.All, inputZoom);
                    curFireTime = 0f;
                    attackAudioSource.Play();
                }
            }
            else if (playerAttack.attackType == PlayerAttack.AttackType.Melee && inputFire && curFireTime > maxFireTime)
            {
                animator.SetTrigger("Attack1");
                curFireTime = 0f;
                StartCoroutine(playerAttack.OnAttackMelee(attackAudioSource));
            }
            else if (playerAttack.attackType == PlayerAttack.AttackType.Gauge)
            {
                if (inputFire && curFireTime > maxFireTime && playerAttack.curGauge > 0)
                {
                    playerAttack.OnAttack(inputZoom);
                    curFireTime = 0f;
                    if (!attackAudioSource.isPlaying)
                    {
                        attackAudioSource.Play();
                    }
                }
                else
                {
                    if (curChargeTime > playerAttack.gaugeChargeSpeed)
                    {
                        playerAttack.curGauge += (playerAttack.curGauge < playerAttack.maxGauge ? 1 : 0);
                        curChargeTime = 0f;
                    }
                }
                curChargeTime += Time.deltaTime;
            }

            curFireTime += Time.deltaTime;
        }
    }

    // Update이후 실행되는 함수
    void LateUpdate()
    {
        if (pv.IsMine)
        {
            // 캐릭터 회전 함수 호출
            playerMove.Turn(mousePos);
        }
    }

    void GetInput()
    {
        // Regacy Input System
        // GetAxisRaw       : 예전 InputSystem을 사용하여 특정 키를 눌렀음을 불러오는 메서드
        // GetButtonDown    : 버튼이 눌린 순간을 감지하는 함수
        // GetMouseButton   : 특정 마우스 버튼이 눌렸는지 확인하는 함수
        // ------------------------------------------------------------ //
        // inputHorizontal = Input.GetAxisRaw("Horizontal");
        // inputVertical = Input.GetAxisRaw("Vertical");
        // inputJump = Input.GetButtonDown("Jump");
        // inputFire = Input.GetMouseButton(0);
        // inputZoom = Input.GetMouseButton(1);
        // float inputMouseX = Input.GetAxisRaw("Mouse X");
        // float inputMouseY = Input.GetAxisRaw("Mouse Y");
        // mousePos = new Vector2(inputMouseX, inputMouseY);
        // ------------------------------------------------------------ //

        // New Input System
        // ReadValue        : input 시스템에 입력받도록 설정된 값을 읽어오기

        // 받은 입력값으로 상하좌우 벡터 생성
        Vector2 moveV2 = moveAction.ReadValue<Vector2>();

        // 자연스러운 움직임을 유발하도록 Lerp 함수로 frame 당 0.03만큼 움직이도록 구현
        moveVec = Vector3.Lerp(moveVec, new Vector3(moveV2.x, 0, moveV2.y), 0.03f);
        if (moveV2.x == 0)
        {
            moveVec.x = 0;
        }
        if (moveV2.y == 0)
        { 
            moveVec.z = 0;
        }

        // 점프키를 눌렀는지 여부 판별
        inputJump = jumpAction.triggered;

        // 공격키를 눌렀는지 여부 판별
        if (fireAction.WasPressedThisFrame()) inputFire = true;
        if (fireAction.WasReleasedThisFrame())
        { 
            inputFire = false;
            if (playerAttack.attackType == PlayerAttack.AttackType.Gauge)
            {
                Invoke(nameof(StopAttackSound), 0.1f);
            }
            else
            {
                Invoke(nameof(StopAttackSound), 1f);
            }
        }

        // 줌키를 눌렀는지 여부 판별
        if (zoomAction.WasPressedThisFrame()) inputZoom = true;
        if (zoomAction.WasReleasedThisFrame() ) inputZoom = false;

        // 마우스 값 설정
        mousePos = lookAction.ReadValue<Vector2>() + lookControllerAction.ReadValue<Vector2>() * 5.0f;
        

        // R키를 눌렀는지 여부 판별
        if (reloadAction.WasPressedThisFrame() && playerAttack.attackType == PlayerAttack.AttackType.Range)
        {
            // 장전중임을 변수로 표시
            reloading = true;

            // 장전 애니메이션 재생
            animator.SetTrigger("reLoading");

            // 일정시간이 지난 뒤, 장전 상태 해제
            StartCoroutine(CompleteReload());
        }

        if (interactionAction.WasPressedThisFrame() && nearObject != null)
        {
            if (nearObject.tag == "SupplyItem")
            {
                Item item = nearObject.GetComponent<Item>();
                
                switch (item.type)
                {
                    case Item.Type.Healpack:
                        health += item.value;
                        if (health > maxHealth)
                        {
                            health = maxHealth;
                        }
                        break;
                    case Item.Type.MeleeUp:
                            attackpower += item.value;
                            countItemUIAmount[0]++;
                            break;
                    case Item.Type.HealthUp:
                        maxHealth += item.value;
                        health += item.value;
                        break;
                    case Item.Type.SpeedUp:
                            playerMove.moveSpeed += item.value;
                            countItemUIAmount[1]++;
                            break;
                    case Item.Type.SpecialItem:
                        if (playerAttack.attackType == PlayerAttack.AttackType.Range)
                        {
                            playerAttack.maxBulletValue += item.value * 10;
                            playerAttack.curBulletValue = playerAttack.maxBulletValue;
                            
                        }
                        else if (playerAttack.attackType == PlayerAttack.AttackType.Melee)
                        {
                            playerAttack.attackMeleeDamage += item.value * 0.5f;
                            maxHealth += 5;
                            health += 5;
                        }
                        else if (playerAttack.attackType == PlayerAttack.AttackType.Gauge)
                        {
                            attackpower += 1;
                        }
                        countItemUIAmount[2]++;
                        break;
                }
                pv.RPC("DestroyItemRPC", RpcTarget.MasterClient, item.GetComponent<PhotonView>().ViewID);
            }
            else if (nearObject.tag == "Shop")
            {
                isShop = true;
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isEsc = !isEsc;
        }
    }

    [PunRPC]
    void DestroyItemRPC(int viewID)
    {
        GameObject itemObject = PhotonView.Find(viewID).gameObject;
        PhotonNetwork.Destroy(itemObject);
        _playerStatusUI._playerUIManager.OffInteraction();
    }

    // 장전 완료 함수
    public IEnumerator CompleteReload()
    {
        reloadAudioSource[0].Play();
        yield return new WaitForSeconds(1.7f);
        reloadAudioSource[1].Play();

        yield return new WaitForSeconds(2f);
        // 현재 탄약 갯수를 최대값으로 만든다.
        playerAttack.curBulletValue = playerAttack.maxBulletValue;
        reloading = false;
    }

    // 플레이어가 죽음상태가 되면 실행되는 코루틴 함수
    public IEnumerator PlayerDeath()
    {
        yield return new WaitForSeconds(5f);

        if (pv.IsMine)
        {
            _playerStatusUI._playerUIManager.AlertPlayerDeath();
        }
        Debug.Log("으악 죽었당");
    }

    public IEnumerator PlayerHit()
    {
        Debug.Log("아파!");
        if (!hitAudioSource.isPlaying)
        {
            hitAudioSource.Play();
        }

        if (pv.IsMine)
        {
            _playerStatusUI._playerUIManager.HitEffects();
        }

        yield return new WaitForSeconds(0.5f);
        isHit = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ResuePerson"))
        {
            ResuePerson resuePerson = other.GetComponent<ResuePerson>();
            if (resuePerson.canResqued)
            {
                resuePerson.pv.RPC("Resqued", RpcTarget.All);
            }
        }
    }

    // 아이템 or 소방차(상점) 근처에 갔을 때 실행되는 함수
    void OnTriggerStay(Collider other)
    {
        // 아이템과 가까워졌을 때 상호작용키 보이게 만들어 활성화
        if (other.tag == "SupplyItem" && pv.IsMine)
        {
            nearObject = other.gameObject;
            _playerStatusUI._playerUIManager.OnInteraction();
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 아이템으로부터 멀어졌을 때 상호작용키 투명하게 만들어 비활성화
        if (other.tag == "SupplyItem" && pv.IsMine) 
        {
            nearObject = null;
            _playerStatusUI._playerUIManager.OffInteraction();
        }
    }

    void StopAttackSound()
    {
        attackAudioSource.Stop();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 데이터를 보내는 부분
            stream.SendNext(health);
            stream.SendNext(maxHealth);
            stream.SendNext(attackpower);
        }
        else
        {
            // 데이터를 받는 부분
            health = (float)stream.ReceiveNext();
            maxHealth = (float)stream.ReceiveNext();
            attackpower = (float)stream.ReceiveNext();
        }
    }
}
