using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject[] spawnPoints;
    private float spawnInterval = 30.0f; // 몬스터 소환 간격 (초)
    private float spawnTimer = 20.0f;

    void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.Instantiate("Units/Enemies/Boss Monster/Prefabs/Boss Monster", spawnPoint.position, spawnPoint.rotation);
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 타이머 업데이트
            spawnTimer += Time.deltaTime;

            // 일정 시간마다 몬스터 소환
            if (spawnTimer >= spawnInterval)
            {
                spawnPoints = GameObject.FindGameObjectsWithTag("SpawnMonster");
                foreach (GameObject sp in spawnPoints)
                {
                    Transform childPoint = sp.GetComponent<Transform>();
                    SpawnEnemy(childPoint);
                }
                spawnTimer = 0.0f; // 타이머 리셋
            }
        }
    }

    void SpawnEnemy(Transform point)
    {
        // 몬스터 랜덤
        int id = Random.Range(0, 2);
        GameObject createdEnemy;
        if (id == 0)
            createdEnemy = PhotonNetwork.Instantiate("Units/Enemies/Normal Monster/Prefabs/Normal Range A_FireOn", point.position, point.rotation);
        else
            createdEnemy = PhotonNetwork.Instantiate("Units/Enemies/Normal Monster/Prefabs/Normal Melee A_FireOn", point.position, point.rotation);

        createdEnemy.GetComponent<Enemy>();
    }
}
