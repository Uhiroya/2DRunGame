using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyScriptableObjectClass;
using UnityEngine;
using UnityEngine.UI;

public interface IGameView
{
    event Action OnPressStart;
    event Action OnPressReturn;
    event Action OnPressRestart;
    void OnGameFlowStateChanged(GameFlowState gameFlowState);
    void ManualUpdate(float deltaTime);
    void SetUVSpeed(float speed);
    void SetHighScore(float highScore);
    void SetScore(float currentScore);
    void ShowResultUI();
    UniTaskVoid ShowHighScore();
    void Pause();
    void Resume();
}

public class GameView : IGameView
{
    private readonly IAudioManager _audioManager;
    private readonly IBackGroundController _background;
    private readonly GameViewSetting _gameViewSetting;
    private readonly Text _highScoreText;
    private readonly GameObject _pauseUI;
    private readonly Button _restartButton;
    private readonly float _resultAnimationTime;
    private readonly Text _resultScoreText;
    private readonly GameObject _resultUI;
    private readonly Button _returnButton;
    private readonly Text _scoreText;
    private readonly Button _startButton;
    private readonly float _titleAnimationTime;
    private float _highScore;
    private float _latestScore;

    public GameView(
        GameViewSetting gameViewSetting, IAudioManager audioManager, IBackGroundController background,
        Button startButton, Button returnButton, Button restartButton,
        Text scoreText, Text resultScoreText, Text highScoreText,
        GameObject resultUIGroup, GameObject pauseUI,
        float titleAnimationTime, float resultAnimationTime
    )
    {
        _gameViewSetting = gameViewSetting;
        _audioManager = audioManager;
        _background = background;
        _startButton = startButton;
        _returnButton = returnButton;
        _restartButton = restartButton;
        _scoreText = scoreText;
        _highScoreText = highScoreText;
        _resultScoreText = resultScoreText;
        _resultUI = resultUIGroup;
        _pauseUI = pauseUI;
        _titleAnimationTime = titleAnimationTime;
        _resultAnimationTime = resultAnimationTime;

        RegisterEvent();
    }

    public event Action OnPressStart;
    public event Action OnPressReturn;
    public event Action OnPressRestart;

    public void OnGameFlowStateChanged(GameFlowState gameFlowState)
    {
        switch (gameFlowState)
        {
            case GameFlowState.Title:
                _ = ShowHighScore();
                _audioManager.PlayBGM(GameBGM.Title);
                break;
            case GameFlowState.InGame:
                _audioManager.PlayBGM(GameBGM.InGame);
                break;
            case GameFlowState.Waiting:
                _audioManager.StopBGM();
                break;
            case GameFlowState.Result:
                ShowResultUI();
                _audioManager.PlayBGM(GameBGM.Result);
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
        _resultUI.SetActive(true);
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

    private void RegisterEvent()
    {
        _startButton.onClick.AddListener(OnClickStartButton);
        _returnButton.onClick.AddListener(OnClickReturnButton);
        _restartButton.onClick.AddListener(OnClickRestartButton);
    }

    private void OnClickStartButton()
    {
        _audioManager.PlaySE(GameSE.Click);
        OnPressStart?.Invoke();
    }

    private void OnClickReturnButton()
    {
        _audioManager.PlaySE(GameSE.Click);
        OnPressReturn?.Invoke();
    }

    private void OnClickRestartButton()
    {
        _audioManager.PlaySE(GameSE.Click);
        OnPressRestart?.Invoke();
    }

    private async UniTaskVoid ShowResultScore()
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
}
