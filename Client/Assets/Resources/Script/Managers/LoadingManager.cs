using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Image _progressBar;
    [SerializeField] private TMP_Text _loadingText;

    static string _nextScene;

    private Timer _timer;
    private PlayerUIManager _puim;
    private GameManager _gm;
    private CameraManager _cm;
    private ChatManager _chatm;

    private AsyncOperation op;

    public static void LoadScene(string sceneName)
    {
        _nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }

    private void Start()
    {
        _gm = FindObjectOfType<GameManager>(true);
        _gm.loadingManager = this;
        SceneManager.sceneLoaded += _gm.LoadSceneEnd;

        _cm = FindObjectOfType<CameraManager>(true);
        _cm._camera.SetActive(true);

        _timer = FindObjectOfType<Timer>(true);
        _timer.ShowTimer(false);
        _timer.PlayTimer(false);
        SceneManager.sceneLoaded += _timer.LoadSceneEnd;

        _puim = FindObjectOfType<PlayerUIManager>(true);
        _puim._stageText.text = _nextScene;
        _puim._childPlayerUI.SetActive(true);
        SceneManager.sceneLoaded += _puim.LoadSceneEnd;

        _chatm = FindObjectOfType<ChatManager>(true);
        _chatm.chatScroll.SetActive(false);
        SceneManager.sceneLoaded += _chatm.LoadSceneEnd;


        PhotonNetwork.IsMessageQueueRunning = false;
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        _progressBar.fillAmount = 0f;

        op = SceneManager.LoadSceneAsync(_nextScene);
        op.allowSceneActivation = false;

        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            if (op.progress < 0.9f)
            {
                _progressBar.fillAmount = Mathf.Lerp(_progressBar.fillAmount, op.progress, timer);
                if (_progressBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                _progressBar.fillAmount = Mathf.Lerp(_progressBar.fillAmount, 1f, timer);

                if (_progressBar.fillAmount == 1.0f)
                {
                    PhotonNetwork.IsMessageQueueRunning = true;
                    _loadingText.text = "다른 대원을 기다리는 중";
                    _gm.PV.RPC("SetLoadReady", RpcTarget.AllBuffered);
                    yield break;
                }
            }
        }
    }

    public void EndLoading()
    {
        Debug.Log("로딩 끝났다!");
        op.allowSceneActivation = true;
    }

    // 전역 로비 전환 메소드
    public static void OnClickExit()
    {
        SceneManager.LoadScene("Lobby");
        PhotonNetwork.LeaveRoom();
        {
            var obj = FindObjectOfType<Timer>();
            if (obj != null) Destroy(obj.gameObject);
        }
        {
            var obj = FindObjectOfType<PlayerUIManager>();
            if (obj != null) Destroy(obj.gameObject);
        }
        {
            var obj = FindObjectOfType<CameraManager>();
            if (obj != null) Destroy(obj.gameObject);
        }
        {
            var obj = FindObjectOfType<GameManager>();
            if (obj != null) Destroy(obj.gameObject);
        }
        {
            var obj = FindObjectOfType<ChatManager>();
            if (obj != null) Destroy(obj.gameObject);
        }
    }
}
