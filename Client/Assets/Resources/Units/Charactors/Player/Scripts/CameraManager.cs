using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public GameObject _camera;

    private void Awake()
    {
        // ������Ʈ ���� ����
        DontDestroyOnLoad(gameObject);
    }
}
