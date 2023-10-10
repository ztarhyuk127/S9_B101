using UnityEngine;

public class BgmManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgm;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public bool IsPlaying()
    {
        return bgm.isPlaying;
    }

    public void Play()
    {
        if (!IsPlaying())
        {
            bgm.Play();
        }
    }

    public void Stop()
    {
        bgm.Stop();
    }



}
