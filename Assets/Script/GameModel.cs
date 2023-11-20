using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
[System.Serializable]
public class GameModel
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
        GameState.Where(x => x == GameFlowState.Title || x == GameFlowState.InGame)
            .Subscribe( x => Reset()).AddTo(_disposable);
        GameState.Where(x => x == GameFlowState.Result)
            .Subscribe(x => GameStop()).AddTo(_disposable);
    }
    CompositeDisposable _disposable = new();
    ~GameModel()
    {
        _disposable.Dispose();
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
    public void Reset()
    {
        _gameSpeed.Value = 0.5f;
        _score.Value = 0f;
    }
    public void GameStop()
    {
        _gameSpeed.Value = 0f;
    }
}
[System.Serializable]
public enum GameFlowState
{
    None,
    Inisialize,
    Title,
    InGame,
    Result,
}

