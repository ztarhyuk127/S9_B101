using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    private void Awake()
    {
        // ������Ʈ ���� ����
        DontDestroyOnLoad(gameObject);
    }
}
