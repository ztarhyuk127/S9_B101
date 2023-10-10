using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour // ��� �޴� Ŭ������ ������Ʈ�� ����
{
    // �̱��� ��ü
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)  // ��ü�� ��������� ���� ȣ���� ���� ���
            {
                _instance = FindObjectOfType<T>(); // ��ü�� ã�� instance�� �Ҵ��ϰ�
                DontDestroyOnLoad(_instance); // �ı����� �ʵ��� ��� (�� ��� awake������ �ƹ��͵� �������� �ʰԵ�)
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null) // instance�� �ʱ�ȭ ���� �ʾ��� ���
        {
            _instance = GetComponent<T>();   // instance�� �Ҵ��ϰ�
            DontDestroyOnLoad(gameObject); // ��ü�� destory ���� �ʵ��� ���
        }
        else if (_instance != this) // ���� ������� ��ü�� ������ instance�� �ٸ� ��ü�̸�
        {
            Destroy(gameObject); // ���� ������� �̱����� �����Ƿ� ���� ��ü�� �ı�
        }
    }
}