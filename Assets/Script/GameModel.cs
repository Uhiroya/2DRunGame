using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public interface IGameModel
{
    IReadOnlyReactiveProperty<GameFlowState> GameState { get; }
    IReadOnlyReactiveProperty<float> GameSpeed { get; }
    IReadOnlyReactiveProperty<float> Score { get; }
    void SetGameState(GameFlowState state);
    void AddScore(float point);
    void AddSpeed(float point);
    void GameStart();
    void GameStop();
}
public class GameModel : IGameModel
{
    private readonly ReactiveProperty<GameFlowState> _gameState;
    public IReadOnlyReactiveProperty<GameFlowState> GameState => _gameState;
    private readonly ReactiveProperty<float> _gameSpeed ;
    public IReadOnlyReactiveProperty<float> GameSpeed => _gameSpeed;
    private readonly ReactiveProperty<float> _score;
    public IReadOnlyReactiveProperty<float> Score => _score;
    public GameModel()
    {
        _gameState = new(GameFlowState.Inisialize);
        _gameSpeed = new(0f);
        _score = new(0f);
    }
    public void SetGameState(GameFlowState state)
    {
        _gameState.Value = state;
    }
    public void AddScore(float point)
    {
        _score.Value += point;
    }
    public void AddSpeed(float point)
    {
        _gameSpeed.Value += point;
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

