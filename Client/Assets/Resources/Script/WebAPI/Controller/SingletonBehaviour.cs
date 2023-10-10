using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour // 상속 받는 클래스는 컴포넌트로 제약
{
    // 싱글톤 객체
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)  // 객체가 만들어지기 전에 호출을 받은 경우
            {
                _instance = FindObjectOfType<T>(); // 객체를 찾아 instance에 할당하고
                DontDestroyOnLoad(_instance); // 파괴되지 않도록 등록 (이 경우 awake에서는 아무것도 수행하지 않게됨)
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null) // instance가 초기화 되지 않았을 경우
        {
            _instance = GetComponent<T>();   // instance를 할당하고
            DontDestroyOnLoad(gameObject); // 객체가 destory 되지 않도록 등록
        }
        else if (_instance != this) // 지금 만들어진 객체가 기존의 instance와 다른 객체이면
        {
            Destroy(gameObject); // 먼저 만들어진 싱글톤이 있으므로 지금 객체는 파괴
        }
    }
}