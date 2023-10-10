using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ResuePerson : MonoBehaviour
{
    [HideInInspector]
    public int personIdx = 0;
    [HideInInspector]
    public bool canResqued = true;

    Animator animator;
    GameObject GameManagerObject;
    public PhotonView pv;
    [SerializeField]
    private GameObject findEffect;

    void Awake()
    {
        animator = GetComponent<Animator>();
        GameManagerObject = FindObjectOfType<GameManager>().gameObject;
        pv = GetComponent<PhotonView>();
    }

    [PunRPC]
    public void Resqued()
    {
        GameManager gameManager = GameManagerObject.GetComponent<GameManager>();
        animator.SetBool("Save", true);
        gameManager.ResquedPersonCount++;
        if (gameManager.ResquedPersonCount > gameManager.ResquedPersonCondition)
            gameManager.ResquedPersonCount = gameManager.ResquedPersonCondition;
        gameManager.SetResquedPersonText();
        canResqued = false;
        Instantiate(findEffect, transform.position, transform.rotation);
        Invoke(nameof(DestroyPerson), 5f);
    }

    void DestroyPerson()
    {
        Destroy(gameObject);
    }
}
