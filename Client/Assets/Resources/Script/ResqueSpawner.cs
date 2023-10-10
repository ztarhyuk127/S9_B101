using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ResqueSpawner : MonoBehaviour
{
    // 스폰 생성될 위치
    public GameObject[] spawnPoints;
    public bool canSpawn = false;
    private int spawnCount = 0;
    private float startTime = 0;
    private float waitTime = 5f;
    private float spawnTime = 0;
    private float spawnInterval = 0.1f;

    void Awake()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("ResqueSpawner");
    }

    void Update()
    {
        if (canSpawn && PhotonNetwork.IsMasterClient)
        {
            if (startTime > waitTime)
            {
                if (spawnTime > spawnInterval)
                {
                    int randomId = Random.Range(0, 2);
                    if (randomId == 0) PhotonNetwork.Instantiate("Units/Ida Faber/Survival Girl/Prefabs/SK_SurvGirl_Combination09 Variant", spawnPoints[spawnCount].transform.position, spawnPoints[spawnCount].transform.rotation);
                    else PhotonNetwork.Instantiate("Units/Ida Faber/Survival Man/Prefabs/SK_SurvMan_Comb06 Variant", spawnPoints[spawnCount].transform.position, spawnPoints[spawnCount].transform.rotation);
                    spawnCount++;
                    if (spawnCount == spawnPoints.Length)
                    {
                        canSpawn = false;
                    }
                    spawnTime = 0;
                }
                spawnTime += Time.deltaTime;
            }
            else
            {
                startTime += Time.deltaTime;
            }
        }
    }
}
