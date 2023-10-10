using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MonsterSpawner : MonoBehaviourPun
{
    private float spawnStartTime = 2.0f;
    private float spawnInterval = 0.1f; // ���� ��ȯ ���� (��)
    private float spawnStartTimer = 0.0f;
    private float spawnIntervalTimer = 0.0f;


    private bool isSpawn = false;
    private int spawnCount = 0;

    // ������ ���� ������ 
    public Enemy enemyRangePrefab;
    public Enemy enemyMeleePrefab;

    // ���� ������ ��ġ
    public GameObject[] spawnPoints;

    private void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnMonster");
    }

    private void Update()
    {
        if (isSpawn && PhotonNetwork.IsMasterClient)
        {
            // ��ŸƮ Ÿ�̸� ������Ʈ
            spawnStartTimer += Time.deltaTime;
            // ���� �ð��� ���� ��ȯ
            if (spawnStartTimer >= spawnStartTime)
            {
                // ���� Ÿ�̸� ������Ʈ
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
        // ���� ����
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
