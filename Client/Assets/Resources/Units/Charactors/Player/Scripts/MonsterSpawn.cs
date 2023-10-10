using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MonsterSpawn : MonoBehaviour
{
    private Scene scene;

    private string sceneName;
    private int sceneNum;

    private GameObject[] enemyObjs;


    private Dictionary<string, int> sceneNumber = new Dictionary<string, int>()
    {
        {"Stage1_1", 0},
        {"Stage1_2", 1},
        {"stage1_3", 2},
        {"stage2_1", 3},
        {"stage2_2", 4},
        {"stage2_3", 5},
        {"stage3_1", 6},
        {"stage3_2", 7},
        {"stage3_3", 8},
        {"Way Scene_1", 9},
        {"Way Scene_2", 10},
        {"Way Scene_3", 11},
    };
    // Start is called before the first frame update
    void Start()
    {
        scene = SceneManager.GetActiveScene();

        sceneName = scene.name;

        sceneNum = sceneNumber[sceneName];
    }

    // Update is called once per frame
    void Update()
    {
        spawnMonster(sceneNum);
    }

    void spawnMonster(int sceneNum)
    {
        Vector3 myPos = transform.position;

        if (sceneNum == 0)
        {
            // enemy 1
            if (myPos.x <= -8.0f && myPos.x >= -9.0f && myPos.z >= 13.0f && myPos.z <= 20.0f)
            {
                enemyObjs = GameObject.FindGameObjectsWithTag("EnemyGroup1");

                foreach (GameObject enemyObj in enemyObjs)
                {
                    enemyObj.SetActive(true);
                }
            }
            else if(myPos.x <= -10.0f && myPos.x >= -14.0f && myPos.z >= 22.0f && myPos.z <= 28.0f)
            {
                enemyObjs = GameObject.FindGameObjectsWithTag("EnemyGroup2");

                foreach (GameObject enemyObj in enemyObjs)
                {
                    enemyObj.SetActive(true);
                }
            }
        }
        else if (sceneNum == 1)
        {

        } 
        else if (sceneNum == 2)
        {

        }
        else if (sceneNum == 3)
        {

        }
        else if (sceneNum == 4)
        {

        }
        else if (sceneNum == 5)
        {

        }
        else if (sceneNum == 6)
        {

        }
        else if (sceneNum == 7)
        {

        }
        else if (sceneNum == 8)
        {

        }
        else if (sceneNum == 9)
        {

        }
        else if (sceneNum == 10)
        {

        }
        else if (sceneNum == 11)
        {

        }

    }
}
