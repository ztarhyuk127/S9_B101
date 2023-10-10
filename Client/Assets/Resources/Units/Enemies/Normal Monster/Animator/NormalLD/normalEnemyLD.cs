using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class normalEnemyLD : MonoBehaviour
{
    Animator anim;
    private bool walk = false;
    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            anim.SetTrigger("Attack1");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            anim.SetTrigger("Attack2");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetTrigger("TurnLeft");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            anim.SetTrigger("TurnRight");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if(walk)
            {
                walk = false;
                anim.SetBool("Walking", walk);
            }
            else
            {
                walk = true;
                anim.SetBool("Walking", walk);
            }
        }
    }
}
