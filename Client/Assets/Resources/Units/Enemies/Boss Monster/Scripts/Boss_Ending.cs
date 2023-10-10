using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Ending : MonoBehaviour
{
    Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("Defeat", 1.0f);
        
    }

    // Update is called once per frame
    void Defeat()
    {
        anim.SetTrigger("Defeat");
        
    }
}
