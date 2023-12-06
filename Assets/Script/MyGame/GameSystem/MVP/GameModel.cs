using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Drawing;
using MyScriptableObjectClass;
using System;
using VContainer.Unity;
using System.Data.Common;

public interface IGameModel
{
    IReadOnlyReactiveProperty<GameFlowState> GameState { get; }
    IReadOnlyReactiveProperty<float> GameSpeed { get; }
    IReadOnlyReactiveProperty<float> Score { get; }
    float HighScore { get; }
    void ChangeStateToTitle();
    void ChangeStateToInGame();
    void ChangeStateToWaiting();
    void ChangeStateToResult();
    void ManualUpdate(float deltaTime);
    void AddDistanceScore(float deltaTime);
    void AddItemScore(float score);
    void AddSpeed(float deltaTime);
}
public class GameModel : IGameModel , IDisposable
{
    GameModelSetting _gameModelSetting;
    SaveData _saveData;

    readonly ReactiveProperty<GameFlowState> _gameState;
    readonly ReactiveProperty<float> _gameSpeed ;
    readonly ReactiveProperty<float> _score;
    public float HighScore => _saveData.HighScore;
    public IReadOnlyReactiveProperty<GameFlowState> GameState => _gameState;
    public IReadOnlyReactiveProperty<float> GameSpeed => _gameSpeed;
    public IReadOnlyReactiveProperty<float> Score => _score;
    public GameModel(GameModelSetting gameModelSetting)
    {
        _gameModelSetting = gameModelSetting;
        _gameState = new(GameFlowState.Inisialize);
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
                    case GameFlowState.InGame:
                        _score.Value = 0f;
                        _gameSpeed.Value = 0.5f;
                        break;
                    case GameFlowState.Result:
                        _gameSpeed.Value = 0f;
                        _saveData.SaveScore(Score.Value);
                        SaveDataInterface.SaveJson(_saveData);
                        break;
                    default:
                        break;
                }
            })
        .AddTo(_disposable);
    }
    public void ChangeStateToTitle()
        => _gameState.Value = GameFlowState.Title;
    public void ChangeStateToInGame()
        => _gameState.Value = GameFlowState.InGame;
    public void ChangeStateToWaiting()
        => _gameState.Value = GameFlowState.Waiting;
    public void ChangeStateToResult()
        => _gameState.Value = GameFlowState.Result;
    public void ManualUpdate(float deltaTime)
    {
        AddDistanceScore(deltaTime);
        AddSpeed(deltaTime);
    }
    public void AddDistanceScore(float deltaTime)
    {
        _score.Value += _gameModelSetting.ScoreRatePerSecond * deltaTime;
    }
    public void AddItemScore(float score)
    {
        _score.Value += score;
    }
    public void AddSpeed(float deltaTime)
    {
        _gameSpeed.Value += _gameModelSetting.SpeedUpRate * deltaTime; ;
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

