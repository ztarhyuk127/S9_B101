using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    // ���� ������ �θ� ��ġ
    public GameObject _childPlayerUI;
    public Transform _playerStatusBox;
    public Transform _friendStatusBox;
    public Transform _weaponBox;
    public TMP_Text _objectiveText;
    public TMP_Text _stageText;
    public GameObject _crosshair;

    // ��ü ������ ������
    [SerializeField] private GameObject _weaponBoxUIPrefab;

    // ��, ESC �޴�, Fade �޴�
    [SerializeField] private Image _exitMenu;
    [SerializeField] private Image _infoMenu;
    [SerializeField] private CanvasGroup _fadeCg;
    [SerializeField] private TMP_Text _gameover;
    [SerializeField] private GameObject _gameoverExit;
    public CanvasGroup _hitScreen;
    public CanvasGroup _interactionMenu;
    public float _hitFadeTime;

    // Ʃ�丮�� ��������Ʈ
    [SerializeField] private Sprite[] _tutorials;

    public bool _isInitiated = false;
    private bool _blockUI = false;

    public Player player;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        // ���� â�� ��Ȱ��ȭ (���� ���� �� �̹Ƿ�)
        _exitMenu.gameObject.SetActive(false);
        _infoMenu.gameObject.SetActive(false);
        _childPlayerUI.SetActive(false);
        
    }

    // ���� ���� ���� �� UI ������� ����
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


        // �� ���� ���� UI ����
        PlayerAttack[] playerAttacks = FindObjectsByType<PlayerAttack>(FindObjectsSortMode.None);
        foreach (PlayerAttack attack in playerAttacks)
        {
            if (attack.pv.IsMine)
            {
                Debug.Log("�� ���� ã�Ҵ�!");
                GameObject weaponBox = Instantiate(_weaponBoxUIPrefab, _weaponBox);
                WeaponBoxUI weaponBoxScript = weaponBox.GetComponent<WeaponBoxUI>();
                weaponBoxScript.SetPlayerAttack(attack);
                break;
            }
        }

        // Ʃ�丮�� ��������Ʈ ����
        int type = PlayerPrefs.GetString("CharacterType")[^1] - 'A';
        _infoMenu.sprite = _tutorials[type];

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (_blockUI) return;

        // Tab ������ ĳ���� ����â ǥ��
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _infoMenu.gameObject.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            _infoMenu.gameObject.SetActive(false);
        }

        // ESC ������ �� ���� ���� â ǥ��
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

        // �����ۿ� ������ �ٰ����� �� ��ȣ�ۿ� Ű Ȱ��ȭ
        
        
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

    // �� �ε�� �ݹ�
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
