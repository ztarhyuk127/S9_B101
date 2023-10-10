using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MonsterSpawner : MonoBehaviourPun
{
    private float spawnStartTime = 2.0f;
    private float spawnInterval = 0.1f; // 몬스터 소환 간격 (초)
    private float spawnStartTimer = 0.0f;
    private float spawnIntervalTimer = 0.0f;


    private bool isSpawn = false;
    private int spawnCount = 0;

    // 생성할 몬스터 프리펩 
    public Enemy enemyRangePrefab;
    public Enemy enemyMeleePrefab;

    // 스폰 생성될 위치
    public GameObject[] spawnPoints;

    private void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnMonster");
    }

    private void Update()
    {
        if (isSpawn && PhotonNetwork.IsMasterClient)
        {
            // 스타트 타이머 업데이트
            spawnStartTimer += Time.deltaTime;
            // 일정 시간후 몬스터 소환
            if (spawnStartTimer >= spawnStartTime)
            {
                // 스폰 타이머 업데이트
                spawnIntervalTimer += Time.deltaTime;
                if (spawnIntervalTimer >= spawnInterval)
                {
                    Transform childPoint = spawnPoints[spawnCount].GetComponent<Transform>();
                    SpawnEnemy(childPoint);
                    spawnCount++;
                    spawnIntervalTimer = 0;
                }
                if (spawnCount >= spawnPoints.Length)
                {
                    isSpawn = false;
                }
            }
        }
    }

    public void StartSpawnEnemy()
    {
        isSpawn = true;
    }

    private void SpawnEnemy(Transform point)
    {
        // 몬스터 랜덤
        int id = Random.Range(0, 2);
        if (id == 0)
            PhotonNetwork.Instantiate("Units/Enemies/Normal Monster/Prefabs/Normal Range A_FireOn", point.position, point.rotation);
        else
            PhotonNetwork.Instantiate("Units/Enemies/Normal Monster/Prefabs/Normal Melee A_FireOn", point.position, point.rotation);
    }

    public void NewPlayerSynchronize()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach(GameObject enemy in enemies)
        {
            enemy.GetComponent<PhotonTransformView>().transform.position = enemy.transform.position;
        }
    }
}
