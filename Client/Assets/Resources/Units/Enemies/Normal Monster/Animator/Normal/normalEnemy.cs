using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class normalEnemy : MonoBehaviour
{
    Animator anim;
    private bool walk = false;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            anim.SetTrigger("Attack1");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            anim.SetTrigger("Attack2");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger("Attack3");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetTrigger("Attack4");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            anim.SetTrigger("Attack5");
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            anim.SetTrigger("Attack6");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if(walk)
            {
                walk = false;
                anim.SetBool("Walk", walk);
            }
            else
            {
                walk = true;
                anim.SetBool("Walk", walk);
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetTrigger("TurnLeft");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            anim.SetTrigger("TurnRight");
        }
    }
}
