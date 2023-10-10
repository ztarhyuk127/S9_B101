using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    // 생성 프리펩 부모 위치
    public GameObject _childPlayerUI;
    public Transform _playerStatusBox;
    public Transform _friendStatusBox;
    public Transform _weaponBox;
    public TMP_Text _objectiveText;
    public TMP_Text _stageText;
    public GameObject _crosshair;

    // 객체 생성용 프리펩
    [SerializeField] private GameObject _weaponBoxUIPrefab;

    // 탭, ESC 메뉴, Fade 메뉴
    [SerializeField] private Image _exitMenu;
    [SerializeField] private Image _infoMenu;
    [SerializeField] private CanvasGroup _fadeCg;
    [SerializeField] private TMP_Text _gameover;
    [SerializeField] private GameObject _gameoverExit;
    public CanvasGroup _hitScreen;
    public CanvasGroup _interactionMenu;
    public float _hitFadeTime;

    // 튜토리얼 스프라이트
    [SerializeField] private Sprite[] _tutorials;

    public bool _isInitiated = false;
    private bool _blockUI = false;

    public Player player;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        // 각종 창들 비활성화 (게임 시작 전 이므로)
        _exitMenu.gameObject.SetActive(false);
        _infoMenu.gameObject.SetActive(false);
        _childPlayerUI.SetActive(false);
        
    }

    // 최초 게임 시작 시 UI 구성요소 빌드
    public void Init()
    {
        GameObject playerStatusUI = PhotonNetwork.Instantiate("Prefabs/UI/PlayerStatusUI", new Vector3(0, 0, 0), Quaternion.identity);

        PlayerStatusUI[] playerUIs = FindObjectsByType<PlayerStatusUI>(FindObjectsSortMode.None);
        foreach (PlayerStatusUI playerUI in playerUIs)
        {
            if (playerUI._PV.IsMine)
            {
                playerUI.SetPlayerUIManager(this);
                playerUI.transform.SetParent(_playerStatusBox, false);
            }
            else
            {
                playerUI.SetPlayerUIManager(this);
                playerUI.transform.SetParent(_friendStatusBox, false);
            }
        }


        // 내 무기 상태 UI 생성
        PlayerAttack[] playerAttacks = FindObjectsByType<PlayerAttack>(FindObjectsSortMode.None);
        foreach (PlayerAttack attack in playerAttacks)
        {
            if (attack.pv.IsMine)
            {
                Debug.Log("내 어택 찾았다!");
                GameObject weaponBox = Instantiate(_weaponBoxUIPrefab, _weaponBox);
                WeaponBoxUI weaponBoxScript = weaponBox.GetComponent<WeaponBoxUI>();
                weaponBoxScript.SetPlayerAttack(attack);
                break;
            }
        }

        // 튜토리얼 스프라이트 설정
        int type = PlayerPrefs.GetString("CharacterType")[^1] - 'A';
        _infoMenu.sprite = _tutorials[type];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (_blockUI) return;

        // Tab 누를시 캐릭터 정보창 표시
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _infoMenu.gameObject.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            _infoMenu.gameObject.SetActive(false);
        }

        // ESC 눌렀을 시 게임 종료 창 표시
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_exitMenu.gameObject.activeSelf)
            {
                _exitMenu.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                _exitMenu.gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        if (_hitScreen.alpha > 0)
        {
            _hitFadeTime += Time.deltaTime * 0.5f;
            _hitScreen.alpha = Mathf.Lerp(0.1f, 0f, _hitFadeTime);
        }

        // 아이템에 가까이 다가갔을 때 상호작용 키 활성화
        
        
    }

    public void AlertPlayerDeath()
    {
        _blockUI = true;
        _gameover.gameObject.SetActive(true);
        _gameover.text = "GAME OVER";
        StartCoroutine(Fade(true));
    }

    public void OnClickCancel()
    {
        _exitMenu.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnClickExit()
    {
        LoadingManager.OnClickExit();
    }

    public void SetObjectiveText(string text)
    {
        _objectiveText.text = text;
    }

    // 씬 로드시 콜백
    public void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        StartCoroutine(Fade(false));
        _childPlayerUI.SetActive(true);
        if (!_isInitiated)
        {
            _isInitiated = true;
            Init();
        }
        SceneManager.sceneLoaded -= LoadSceneEnd;
    }

    public void HitEffects()
    {
        _hitFadeTime = 0.0f;
        _hitScreen.alpha = 0.1f;
    }

    public void OnInteraction()
    {
        _interactionMenu.alpha = 1.0f;
    }
    public void OffInteraction()
    {
        _interactionMenu.alpha = 0f;
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        _fadeCg.alpha = isFadeIn ? 0 : 1;
        _fadeCg.gameObject.SetActive(true);
        float timer = 0f;

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.deltaTime*0.2f;
            _fadeCg.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
        }

        if (!isFadeIn)
        {
            _fadeCg.gameObject.SetActive(false);
        }

        if (isFadeIn)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _gameoverExit.SetActive(true);
        }
    }
}
