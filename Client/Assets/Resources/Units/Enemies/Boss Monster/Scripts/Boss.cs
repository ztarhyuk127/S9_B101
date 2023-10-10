using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [SerializeField] Image _bossHealthbar;
    [SerializeField] TMP_Text _bossHealthNumber;

    BossAttack bossAttack;
    Animator animator;

    [Header("boss의 관절 부분 object")]
    public GameObject spine;
    public GameObject head;
    public GameObject jaw;

    public GameObject leftShoulder;
    public GameObject leftUpArm;
    public GameObject leftDownArm;
    public GameObject leftHand;
    public GameObject leftIndexFinger_1;
    public GameObject leftIndexFinger_2;
    public GameObject leftIndexFinger_3;
    public GameObject leftMiddleFinger_1;
    public GameObject leftMiddleFinger_2;
    public GameObject leftMiddleFinger_3;
    public GameObject leftRingFinger_1;
    public GameObject leftRingFinger_2;
    public GameObject leftRingFinger_3;
    public GameObject leftThumbFinger_1;
    public GameObject leftThumbFinger_2;
    public GameObject leftThumbFinger_3;

    public GameObject rightShoulder;
    public GameObject rightUpArm;
    public GameObject rightDownArm;
    public GameObject rightHand;
    public GameObject rightIndexFinger_1;
    public GameObject rightIndexFinger_2;
    public GameObject rightIndexFinger_3;
    public GameObject rightMiddleFinger_1;
    public GameObject rightMiddleFinger_2;
    public GameObject rightMiddleFinger_3;
    public GameObject rightRingFinger_1;
    public GameObject rightRingFinger_2;
    public GameObject rightRingFinger_3;
    public GameObject rightThumbFinger_1;
    public GameObject rightThumbFinger_2;
    public GameObject rightThumbFinger_3;

    [Tooltip("보스 몬스터의 체력")]
    public float health;
    private float maxHealth;


    [Tooltip("현재 공격 패턴이 진행중인지 확인하는 변수")]
    public bool nowThink = false;

    public PhotonView pv;

    public AudioClip[] audioClips;
    private AudioSource audioSource;
    public AudioClip roarClip;

    void Awake()
    {
        bossAttack = GetComponent<BossAttack>();
        animator = GetComponent<Animator>();
        
        pv = GetComponent<PhotonView>();
        audioSource = GetComponent<AudioSource>();

        maxHealth = health;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PatternThink();
        }

        _bossHealthbar.fillAmount = health / maxHealth;
        _bossHealthNumber.text = ((int)health).ToString();

        if (health <= 0)
        {

            FindObjectOfType<PlayerUIManager>()._childPlayerUI.SetActive(false);
            FindObjectOfType<CameraManager>()._camera.SetActive(false);
            Timer timer = FindObjectOfType<Timer>();
            timer.PlayTimer(false);
            timer.ShowTimer(false);

            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                player.SetActive(false);
            }

            SceneManager.LoadScene("Boss_Clear");
        }
    }

    void PatternThink()
    {
        // 패턴이 진행중이 아닐 때 작동
        if (!nowThink)
        {
            nowThink = true;

            pv.RPC("PlayAudio", RpcTarget.AllBuffered);

            // 패턴 번호에 따른 실행
            switch (bossAttack.patternIdx)
            {
                // Roar
                case 0:
                    pv.RPC("RoarRPC", RpcTarget.AllBuffered);
                    break;
                // Targeting Player
                case 1:
                    pv.RPC("TargetingPlayerRPC", RpcTarget.MasterClient);
                    break;
                // downward blow to ground
                case 2:
                    pv.RPC("DownwardBlowRPC", RpcTarget.All);
                    break;
                // Breath
                case 3:
                    pv.RPC("BreathRPC", RpcTarget.All);
                    break;
                // Smash
                case 4:
                    pv.RPC("SmashRPC", RpcTarget.All);
                    break;
                // AreaFireExplosion
                case 5:
                    pv.RPC("AreaFireExplosionRPC", RpcTarget.All);
                    break;
            }
        }
    }

    [PunRPC]
    void PlayAudio()
    {
        if (bossAttack.patternIdx != 0)
        {
            audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
            audioSource.Play();
        }
        else
        {
            audioSource.clip = roarClip;
        }
    }

    [PunRPC]
    void TargetingPlayerRPC()
    {
        StartCoroutine(TargetingPlayer());
    }
    
    IEnumerator TargetingPlayer()
    {
        //Collider[] colliders = Physics.OverlapSphere(spine.transform.position, 45f, LayerMask.GetMask("Player"));

        //animator.SetTrigger("TurnAction");

        //if (colliders.Length > 0)
        //{
        //    GameObject targetPlayer = colliders[Random.Range(0, colliders.Length)].gameObject;
        //    Vector3 targetVector = targetPlayer.transform.position - transform.position - Vector3.up * 52f;

        //    float turnAngle = Vector3.SignedAngle(transform.forward, targetVector, Vector3.up);
        //    animator.SetFloat("Turn", turnAngle / 180);

        //    yield return new WaitForSeconds(4f);

            if (PhotonNetwork.IsMasterClient)
            {
                bossAttack.RandomPattern(bossAttack.patternIdx);
            }
        //}
        yield return new WaitForSeconds(2f);
        nowThink = false;
    }

    [PunRPC]
    public void BossHitByPlayer(float damage)
    {
        health -= damage;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerMelee"))
        {
            PlayerAttack playerAttack = other.GetComponentInParent<PlayerAttack>();
            pv.RPC("BossHitByPlayer", RpcTarget.All, playerAttack.attackMeleeDamage);
        }
    }
}
