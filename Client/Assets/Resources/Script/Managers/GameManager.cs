using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    private CurrentVersion currentVersion = CurrentVersion.Presentation;

    public GameObject myFireFight;
    public AudioClip[] bgmSounds;
    public AudioSource audioBGM;

    public PhotonView PV;

    [HideInInspector]
    public LoadingManager loadingManager;

    private bool _isInitiated = false;
    private int loadedPlayer = 0;
    
    public int waitNextStage = 0;

    public enum StageState
    {
        Stage1_1,
        Stage1_2,
        stage1_3,
        stage2_1,
        stage2_2,
        stage2_3,
        stage3_1,
        stage3_2,
        stage3_3,
        stage4_1,
        stage4_2,
        stage4_3,
        Boss_Stage,
        Boss_Start
    }

    public enum CurrentVersion
    {
        Demo,
        Presentation,
        Release
    }

    private StageState currentGameState;
    private int stageNum = 0;

    public int ResquedPersonCount = 0;
    public int ResquedPersonCondition = 0;

    PlayerUIManager playerUIManager;

    private void Awake()
    {
        // 오브젝트 삭제 금지
        DontDestroyOnLoad(gameObject);
        playerUIManager = FindObjectOfType<PlayerUIManager>();
    }

    [PunRPC]
    public void SetLoadReady()
    {
        loadedPlayer = loadedPlayer + 1;

        if (loadedPlayer == PhotonNetwork.CurrentRoom.PlayerCount && PhotonNetwork.IsMasterClient)
        {
            PV.RPC("FinishLoading", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void FinishLoading()
    {
        loadingManager.EndLoading();
    }

    [PunRPC]
    public void ReadyNextStage()
    {
        waitNextStage = waitNextStage + 1;
        Debug.Log(waitNextStage);
        if (waitNextStage == PhotonNetwork.CurrentRoom.PlayerCount && PhotonNetwork.IsMasterClient) 
        {
            int nextStageRandom = Random.Range(0, 3);
            PV.RPC("LoadNextStage", RpcTarget.AllBuffered, nextStageRandom);
        }
    }

    public void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        loadedPlayer = 0;

        if (!_isInitiated)
        {
            _isInitiated=true;
            InitializePlayerCharacter();
        }

        SpawnPlayer();

        if (currentGameState != StageState.Boss_Stage)
        {
            SpawnEnemies();
            SpawnPeople();
        }

        SceneManager.sceneLoaded -= LoadSceneEnd;
    }

    [PunRPC]
    public void LoadNextStage(int idx)
    {
        audioBGM.Pause();
        switch (currentVersion)
        {
            case CurrentVersion.Demo:
                // 다음 씬으로
                currentGameState = currentGameState + 1;
                switch (currentGameState)
                {
                    case StageState.Stage1_1:
                        audioBGM.clip = bgmSounds[0];
                        break;
                    case StageState.Stage1_2:
                        audioBGM.clip = bgmSounds[1];
                        break;
                    case StageState.stage1_3:
                        audioBGM.clip = bgmSounds[0];
                        break;
                    default:
                        currentGameState = StageState.Stage1_1;
                        break;
                }
            break; 
            case CurrentVersion.Presentation:
                // 다음 씬으로
                if (ResquedPersonCount < ResquedPersonCondition)
                {
                    audioBGM.Play();
                    return;
                }
                stageNum = stageNum + 1;
                switch (stageNum)
                {
                    case 1:
                        ResquedPersonCondition = 3;
                        playerUIManager.SetObjectiveText($"구조 인원을 모두 구출한 후\r\n소방차를 찾아 이동하십시오.\r\n\r\n구조된 인원 {ResquedPersonCount} / {ResquedPersonCondition} 명");
                        currentGameState = StageState.stage2_2;
                        audioBGM.clip = bgmSounds[0];
                        break;
                    case 2:
                        playerUIManager.SetObjectiveText("몰려오는 모든 적을 처치하고 \r\n소방차를 찾아 이동하십시오.");
                        ResquedPersonCount = 0;
                        ResquedPersonCondition = 0;
                        currentGameState = StageState.stage4_1;
                        audioBGM.clip = bgmSounds[1];
                        break;
                    case 3:
                        playerUIManager.SetObjectiveText("화마들의 왕을 처치하십시오.");
                        currentGameState = StageState.Boss_Stage;
                        audioBGM.clip = bgmSounds[2];
                        break;
                }
            break; 
            case CurrentVersion.Release:
                stageNum = stageNum + 1;

                if (stageNum == 1)
                {
                    audioBGM.clip = bgmSounds[0];
                }
                else if (stageNum == 2)
                {
                    audioBGM.clip = bgmSounds[1];
                }
                else if (stageNum == 3)
                {
                    audioBGM.clip = bgmSounds[0];
                }
                else if (stageNum == 4)
                {
                    audioBGM.clip = bgmSounds[1];
                }
                else
                {
                    audioBGM.clip = bgmSounds[2];
                }

                switch (stageNum)
                {
                    case 1:
                        switch (idx)
                        {
                            case 0:
                                currentGameState = StageState.Stage1_1;
                                break;
                            case 1:
                                currentGameState = StageState.Stage1_2;
                                break;
                            case 2:
                                currentGameState = StageState.stage1_3;
                                break;
                        }
                        break;

                    case 2:
                        switch (idx)
                        {
                            case 0:
                                currentGameState = StageState.stage2_1;
                                break;
                            case 1:
                                currentGameState = StageState.stage2_2;
                                break;
                            case 2:
                                currentGameState = StageState.stage2_3;
                                break;
                        }
                        break;

                    case 3:
                        switch (idx)
                        {
                            case 0:
                                currentGameState = StageState.stage3_1;
                                break;
                            case 1:
                                currentGameState = StageState.stage3_2;
                                break;
                            case 2:
                                currentGameState = StageState.stage3_3;
                                break;
                        }
                        break;

                    case 4:
                        switch (idx)
                        {
                            case 0:
                                currentGameState = StageState.stage4_1;
                                break;
                            case 1:
                                currentGameState = StageState.stage4_2;
                                break;
                            case 2:
                                currentGameState = StageState.stage4_3;
                                break;
                        }
                        break;

                    case 5:
                        currentGameState = StageState.Boss_Stage;
                    break;
                }
            break;
        }
        
        audioBGM.Play();
        waitNextStage = 0;
        LoadingManager.LoadScene(currentGameState.ToString());
    }

    void InitializePlayerCharacter()
    {
        // 캐릭터 생성
        Transform[] points = GameObject.Find("SpawnGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        myFireFight = PhotonNetwork.Instantiate($"Units/Charactors/Player/Prefabs/{PlayerPrefs.GetString("CharacterType")}", points[idx].position, points[idx].rotation, 0);
    }

    void SpawnPlayer()
    {
        Debug.Log("플래이어 스폰");

        Transform[] points = GameObject.Find("SpawnGroup").GetComponentsInChildren<Transform>();

        int idx = Random.Range(1, points.Length);
        CharacterController characterController = myFireFight.GetComponent<CharacterController>();
        characterController.enabled = false;
        myFireFight.GetComponent<PhotonTransformView>().transform.position = points[idx].position;
        characterController.enabled = true;

    }

    void SpawnEnemies()
    {
        MonsterSpawner monsterSpawner = FindObjectOfType<MonsterSpawner>();

        monsterSpawner.StartSpawnEnemy();
    }

    void SpawnPeople()
    {
        ResqueSpawner resqueSpawner = FindObjectOfType<ResqueSpawner>();
        if (resqueSpawner != null)
        {
            resqueSpawner.canSpawn = true;
        }
    }

    public void SetResquedPersonText()
    {
        playerUIManager.SetObjectiveText($"구조 인원을 모두 구출한 후\r\n소방차를 찾아 이동하십시오.\r\n\r\n구조된 인원 {ResquedPersonCount} / {ResquedPersonCondition} 명");
    }
}