using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text _timer;
    [SerializeField] private PhotonView _PV;

    public float _currentTime = 0.0f;
    private bool _isTimeFlow = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _timer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (_isTimeFlow) 
        {
            _currentTime += Time.deltaTime;
            UpdateTimer();
        }
    }

    public void ShowTimer(bool show)
    {
        _timer.gameObject.SetActive(show);
    }

    public void PlayTimer(bool run)
    {
        _isTimeFlow = run;
    }

    public void UpdateTimer()
    {
        int sec = (int)(_currentTime * 100);
        int milisec = sec % 100;
        sec /= 100;
        int min = sec / 60;
        sec %= 60;
        _timer.text = $"{min}:{sec}:{milisec}";
    }

    public void SyncTimer()
    {
        _PV.RPC("SyncTimerRPC", RpcTarget.AllBuffered, _currentTime);
    }

    [PunRPC]
    public void SyncTimerRPC(float time)
    {
        Debug.Log("Call SyncTimerRPC");
        _currentTime = time;
        ShowTimer(true);
        PlayTimer(true);
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _PV.RequestOwnership();
        }
    }

    public void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        SyncTimer();
        SceneManager.sceneLoaded -= LoadSceneEnd;
    }

    public int GetIntegerTime()
    {
        return (int)(_currentTime * 100);
    }
}
