using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Drawing;

public interface IGameModel
{
    IReadOnlyReactiveProperty<GameFlowState> GameState { get; }
    IReadOnlyReactiveProperty<float> GameSpeed { get; }
    IReadOnlyReactiveProperty<float> Score { get; }
    void ChangeStateToTitle();
    void ChangeStateToInGame();
    void ChangeStateToResult();
    void ManualUpdate(float deltaTime);
    void AddScore(float deltaTime);
    void AddSpeed(float deltaTime);
    void GameStart();
    void GameStop();
}
public class GameModel : IGameModel
{
    float _scoreRatePerSecond;
    float _speedUpRate;
    private readonly ReactiveProperty<GameFlowState> _gameState;
    public IReadOnlyReactiveProperty<GameFlowState> GameState => _gameState;
    private readonly ReactiveProperty<float> _gameSpeed ;
    public IReadOnlyReactiveProperty<float> GameSpeed => _gameSpeed;
    private readonly ReactiveProperty<float> _score;
    public IReadOnlyReactiveProperty<float> Score => _score;
    public GameModel(float scoreRatePerSecond, float speedUpRate)
    {
        _scoreRatePerSecond = scoreRatePerSecond;
        _speedUpRate = speedUpRate;
        _gameState = new(GameFlowState.Inisialize);
        _gameSpeed = new(0f);
        _score = new(0f);
    }
    public void ChangeStateToTitle()
        => _gameState.Value = GameFlowState.Title;
    public void ChangeStateToInGame()
        => _gameState.Value = GameFlowState.InGame;
    public void ChangeStateToResult()
        => _gameState.Value = GameFlowState.Result;
    public void ManualUpdate(float deltaTime)
    {
        AddScore(deltaTime);
        AddSpeed(deltaTime);
    }
    public void AddScore(float deltaTime)
    {
        _score.Value += _scoreRatePerSecond * deltaTime;
    }
    public void AddSpeed(float deltaTime)
    {
        _gameSpeed.Value += _speedUpRate * deltaTime; ;
    }
    public void GameStart()
    {
        _gameSpeed.Value = 0.5f;
        _score.Value = 0f;
    }
    public void GameStop()
    {
        _gameSpeed.Value = 0f;
    }

}

