using MyScriptableObjectClass;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameSE
{
    Click = 0,    
    HitItem = 1,       
    HitEnemy = 2,    
    GameOver = 3,            
}
public enum GameBGM
{
    Title = 0,
    InGame = 1,
    Result = 2,
}
public interface IAudioManager
{
    void PlaySE(GameSE soundIndex);
    void PlayBGM(GameBGM soundIndex);
    void StopBGM();
    void PauseBGM();
    void ResumeBGM();
}
public class AudioManager : IAudioManager
{
    AudioSource _audioSESource;
    AudioSource _audioBGMSource;
    AudioData _audioData;
    AudioClip[] AudioSEClips => _audioData.AudioSEClips;
    AudioClip[] AudioBGMClips => _audioData.AudioBGMClips;
    public AudioManager(MyAudioSetting audioSetting , AudioSource audioSESource ,AudioSource audioBGMSource)
    {
        _audioData = audioSetting.AudioData;
        _audioSESource = audioSESource;
        _audioBGMSource = audioBGMSource;
    }

    public void PlaySE(GameSE soundIndex)
    {
        _audioSESource.PlayOneShot(AudioSEClips[(int)soundIndex]);
    }
    public void PlayBGM(GameBGM soundIndex)
    {
        _audioBGMSource.clip = AudioBGMClips[(int)soundIndex];
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

