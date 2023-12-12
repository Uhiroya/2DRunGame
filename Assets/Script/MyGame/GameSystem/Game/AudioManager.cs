using MyScriptableObjectClass;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SEType
{
    Start = 0,    
    HitItem = 1,       
    HitEnemy = 2,    
    GameOver = 3,            
}
public enum BGMType
{
    Title = 0,
    InGame = 1,
    Result = 2,
}
public interface IAudioManager
{
    void PlaySE(SEType soundIndex);
    void PlayBGM(BGMType soundIndex);
    void StopBGM();
    void PauseBGM();
    void ResumeBGM();
}
public class AudioManager : IAudioManager
{
    AudioSource _audioSESource;
    AudioSource _audioBGMSource;
    AudioData _audioData;
    AudioClip[] _audioSEClips => _audioData.AudioSEClips;
    AudioClip[] _audioBGMClips => _audioData.AudioBGMClips;
    public AudioManager(MyAudioSetting audioSetting , AudioSource audioSESource ,AudioSource audioBGMSource)
    {
        _audioData = audioSetting.AudioData;
        _audioSESource = audioSESource;
        _audioBGMSource = audioBGMSource;
    }

    public void PlaySE(SEType soundIndex)
    {
        _audioSESource.PlayOneShot(_audioSEClips[(int)soundIndex]);
    }
    public void PlayBGM(BGMType soundIndex)
    {
        _audioBGMSource.clip = _audioBGMClips[(int)soundIndex];
        _audioBGMSource.Play();
    }
    public void StopBGM()
    {
        _audioBGMSource.Stop();
    }
    public void PauseBGM()
    {
        _audioBGMSource.Pause();
    }
    public void ResumeBGM()
    {
        _audioBGMSource.UnPause();

    }
}

