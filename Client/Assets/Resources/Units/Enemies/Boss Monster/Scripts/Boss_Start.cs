using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Start : MonoBehaviour
{
    Animator anim;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("Roar", 3.0f);

        
    }

    // Update is called once per frame
    void Roar()
    {
        anim.SetTrigger("Roar");
    }
}
