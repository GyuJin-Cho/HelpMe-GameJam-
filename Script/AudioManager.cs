using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class AudioPlayer : Singleton<AudioPlayer>
{
    private static AudioPlayer instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 이벤트 리스너 등록
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 이벤트 리스너 제거
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.buildIndex == 0)
        {
            PlayBGM(0);
        }
        else
        {
            PlayBGM(1);
        }
    }

    public List<AudioClip> sfxClips = new List< AudioClip>();
    public List<AudioClip> bgmClips = new List<AudioClip>();
    public AudioSource sfxPlayer, bgmPlayer;
    
    public void PlayClip(int idx)
    {
        sfxPlayer.clip = sfxClips[idx];
        sfxPlayer.Play();
    }
    
    public void PlayBGM(int idx)
    {
        bgmPlayer.Stop();
        bgmPlayer.clip = bgmClips[idx];
        bgmPlayer.Play();
    }
}
