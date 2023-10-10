using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public GameObject _camera;

    private void Awake()
    {
        // 오브젝트 삭제 금지
        DontDestroyOnLoad(gameObject);
    }
}
