using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Drawing;
using MyScriptableObjectClass;
using System;
public interface IGameModel
{
    IReadOnlyReactiveProperty<GameFlowState> GameState { get; }
    IReadOnlyReactiveProperty<float> GameSpeed { get; }
    IReadOnlyReactiveProperty<float> Score { get; }
    IReadOnlyReactiveProperty<float> HighScore { get; }
    void OnPlayerConditionChanged(PlayerCondition playerCondition);
    void GoTitle();
    void GameStart();
    void Pause();
    void Resume();
    void ManualUpdate(float deltaTime);
    void AddItemScore(float score);
}
public class GameModel : IGameModel , IDisposable
{
    GameModelSetting _gameModelSetting;
    SaveData _saveData;

    readonly ReactiveProperty<GameFlowState> _gameState;
    readonly ReactiveProperty<float> _gameSpeed ;
    readonly ReactiveProperty<float> _score;
    readonly ReactiveProperty<float> _highScore;
    float _currentSpeed;
    public IReadOnlyReactiveProperty<float> HighScore => _highScore;
    public IReadOnlyReactiveProperty<GameFlowState> GameState => _gameState;
    public IReadOnlyReactiveProperty<float> GameSpeed => _gameSpeed;
    public IReadOnlyReactiveProperty<float> Score => _score;
    public GameModel(GameModelSetting gameModelSetting)
    {
        SaveDataInterface.LoadJson(out _saveData);
        _gameModelSetting = gameModelSetting;
        _gameState = new(GameFlowState.Initialize);
        _highScore = new(_saveData.HighScore);
        _gameSpeed = new(0f);
        _score = new(0f);
        _disposable = new();
        Bind();
    }
    void Bind()
    {
        GameState.Subscribe(x => OnGameStateChanged(x)).AddTo(_disposable);
    }
    void OnGameStateChanged(GameFlowState gameFlowState)
    {
        switch (gameFlowState)
        {
            case GameFlowState.Initialize:
                _currentSpeed = _gameModelSetting.StartSpeed;
                break;
            case GameFlowState.GameInitialize:
                _currentSpeed = _gameModelSetting.StartSpeed;
                _score.Value = 0f;
                ChangeState(GameFlowState.GameInitializeEnd);
                break;
            //PlyerGameStartのタイミングより早くInGameに遷移させてしまう可能性があるため、フェーズを分離。
            case GameFlowState.GameInitializeEnd:
                ChangeState(GameFlowState.InGame);
                break;
            case GameFlowState.InGame:
                _gameSpeed.Value = _currentSpeed;
                break;
            case GameFlowState.Pause:
                _currentSpeed = _gameSpeed.Value;
                break;
            case GameFlowState.Result:
                _gameSpeed.Value = 0f;
                _saveData.SaveScore(Score.Value);
                SaveDataInterface.SaveJson(_saveData);
                _highScore.Value = _saveData.HighScore;
                break;
            default:
                break;
        }
    }
    public void OnPlayerConditionChanged(PlayerCondition playerCondition)
    {
        switch (playerCondition)
        {
            case PlayerCondition.Dying:
                ChangeState(GameFlowState.Waiting);
                break;
            case PlayerCondition.Dead:
                ChangeState(GameFlowState.Result);
                break;
            default:
                break;
        }
    }
    public void GoTitle() => ChangeState(GameFlowState.Title);
    public void GameStart() => ChangeState(GameFlowState.GameInitialize);
    public void Pause() => ChangeState(GameFlowState.Pause);
    public void Resume() => ChangeState(GameFlowState.InGame);

    public void ManualUpdate(float deltaTime)
    {
        AddScoreByDistance(deltaTime);
        AddSpeedByDistance(deltaTime);
    }
    public void AddItemScore(float score)
    {
        _score.Value += score;
    }
    void AddScoreByDistance(float deltaTime)
    {
        _score.Value += _gameModelSetting.ScoreRatePerSecond * deltaTime;
    }
    void AddSpeedByDistance(float deltaTime)
    {
        _gameSpeed.Value += _gameModelSetting.SpeedUpRate * deltaTime;
    }

    void ChangeState(GameFlowState state)
    {
#if UNITY_EDITOR
        Debug.Log(state);
#endif
        _gameState.Value = state;
    }
    /// <summary>
    /// VContainerから呼び出される
    /// </summary>
    CompositeDisposable _disposable;
    public void Dispose()
    {
        _disposable.Dispose();
    }


}

