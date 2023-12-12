using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using MyScriptableObjectClass;
public interface IGameView
{
    void ManualUpdate(float deltaTime);
    void SetUVSpeed(float speed);
    void SetHighScore(float highScore);
    void SetScore(float currentScore);
    void ShowResultUI();
    void OnGameFlowStateChanged(GameFlowState gameFlowState);
    UniTaskVoid ShowHighScore();
    void Pause();
    void Resume();
    void ButtonSound();
    void HitItemSound();
    void HitEnemySound();

}
public class GameView : IGameView
{
    IAudioManager _audioManager;
    float _titleAnimationTime;
    float _resultAnimationTime;
    GameViewSetting _gameViewSetting;
    IBackGroundController _background;
    Text _scoreText;
    GameObject _resultUIGroup;
    Text _highScoreText;
    Text _resultScoreText;
    GameObject _pauseUI;
    public GameView(IAudioManager audioManager,GameViewSetting gameViewSetting ,float titleAnimationTime, float resultAnimationTime 
        ,IBackGroundController background , Text scoreText , GameObject resultUIGroup
        ,Text resultScoreText , Text highScoreText ,GameObject pauseUI)
    {
        _audioManager = audioManager;
        _gameViewSetting = gameViewSetting;
        _titleAnimationTime = titleAnimationTime;
        _resultAnimationTime = resultAnimationTime;
        _background = background;
        _scoreText = scoreText; 
        _resultUIGroup = resultUIGroup;
        _resultScoreText = resultScoreText;
        _highScoreText = highScoreText;
        _pauseUI = pauseUI;
    }
    float _highScore;
    float _latestScore;
    public void OnGameFlowStateChanged(GameFlowState gameFlowState)
    {
        switch (gameFlowState)
        {
            case GameFlowState.Title:
                _ = ShowHighScore();
                _audioManager.PlayBGM(BGMType.Title);
                break;
            case GameFlowState.InGame:
                _audioManager.PlayBGM(BGMType.InGame);
                break;
            case GameFlowState.Waiting:
                _audioManager.StopBGM();
                break;
            case GameFlowState.Result:
                ShowResultUI();
                _audioManager.PlayBGM(BGMType.Result);
                break;
        }
    }
    public void ManualUpdate(float deltaTime)
    {
        _background.ManualUpdate(deltaTime);
    }
    public void SetUVSpeed(float speed)
    {
        _background.SetUVSpeed(speed);
    }
    public void SetHighScore(float highScore)
    {
        _highScore = highScore;
    }
    public void SetScore(float currentScore)
    {
        _latestScore = currentScore;
        _scoreText.text = _latestScore.ToString("00000000");
    }
    public void ShowResultUI()
    {
        _resultUIGroup.SetActive(true);
        _ = ShowResultScore();
    }
    public async UniTaskVoid ShowHighScore()
    {
        await UniTask.Delay((int)(_titleAnimationTime * 1000));
        //カウントアップ処理
        _ = DOVirtual.Float(
            0f,
            _highScore,
            _gameViewSetting.ScoreCountUpTime,
            value => _highScoreText.text = value.ToString("00000000")
        );
    }
    async UniTaskVoid ShowResultScore()
    {
        await UniTask.Delay((int)(_resultAnimationTime * 1000));
        //カウントアップ処理
        _ = DOVirtual.Float(
            0f,
            _latestScore,
            _gameViewSetting.ScoreCountUpTime,
            value => _resultScoreText.text = value.ToString("00000000")
        );
    }
    public void Pause()
    {
        _audioManager.PauseBGM();
        _pauseUI.SetActive(true);
    }
    public void Resume()
    {
        _audioManager.ResumeBGM();
        _pauseUI.SetActive(false);
    }
    public void ButtonSound()
    {
        _audioManager.PlaySE(SEType.Start);
        
    }

    public void HitItemSound()
    {
        _audioManager.PlaySE(SEType.HitItem);
    }

    public void HitEnemySound()
    {
        _audioManager.PlaySE(SEType.HitEnemy);
    }
}
