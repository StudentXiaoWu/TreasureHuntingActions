using UnityEngine;

public class AudioManager : MonoBehaviour 
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get { return _instance; }
    }

    public bool isMute = false;

    public AudioClip button;
    public AudioClip dig;
    public AudioClip end;
    public AudioClip hoe;
    public AudioClip hurt;
    public AudioClip die;
    public AudioClip move;
    public AudioClip door;
    public AudioClip pass;
    public AudioClip enemy;
    public AudioClip tnt;
    public AudioClip map;
    public AudioClip pick;
    public AudioClip flag;
    public AudioClip why;
    public AudioClip winbg;

    public AudioSource audioSource;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        isMute = DataManager.Instance.isMute;
    }

    public void SwitchMuteState()
    {
        isMute = !isMute;
        if (isMute)
        {
            StopBGM();
        }
        else
        {
            PlayBGM();
        }
        DataManager.Instance.SetMuteState(isMute);
    }

    public void PlayClip(AudioClip clip)
    {
        if (isMute == false)
        {
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
        }
    }

    public void PlayBGM()
    {
        if (isMute == false)
        {
            audioSource.Play();
        }
    }

    public void StopBGM()
    {
        audioSource.Stop();
    }
}
