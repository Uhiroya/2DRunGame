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
    float HighScore { get; }
    void ChangeState(GameFlowState state);
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
    float _currentSpeed;
    public float HighScore => _saveData.HighScore;
    public IReadOnlyReactiveProperty<GameFlowState> GameState => _gameState;
    public IReadOnlyReactiveProperty<float> GameSpeed => _gameSpeed;
    public IReadOnlyReactiveProperty<float> Score => _score;
    public GameModel(GameModelSetting gameModelSetting)
    {
        _gameModelSetting = gameModelSetting;
        _gameState = new(GameFlowState.Initialize);
        _gameSpeed = new(0f);
        _score = new(0f);
        SaveDataInterface.LoadJson(out _saveData);
        _disposable = new();
        Bind();
    }
    void Bind()
    {
        GameState.Subscribe(
            x =>
            {
                switch (x)
                {
                    case GameFlowState.Initialize:
                        _currentSpeed = _gameModelSetting.StartSpeed;
                        break;
                    case GameFlowState.InGame:
                        _gameSpeed.Value = _currentSpeed;
                        break;
                    case GameFlowState.Pause:
                        _currentSpeed = _gameSpeed.Value;
                        break;
                    case GameFlowState.Result:
                        _gameSpeed.Value = 0f;
                        _currentSpeed = _gameModelSetting.StartSpeed;
                        _saveData.SaveScore(Score.Value);
                        SaveDataInterface.SaveJson(_saveData);
                        _score.Value = 0f;
                        break;
                    default:
                        break;
                }
            })
        .AddTo(_disposable);
    }
    public void ChangeState(GameFlowState state)
    {
#if UNITY_EDITOR
        Debug.Log(state);
#endif
        _gameState.Value = state;
    }
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


    /// <summary>
    /// VContainerから呼び出される
    /// </summary>
    CompositeDisposable _disposable;
    public void Dispose()
    {
        _disposable.Dispose();
    }
}

