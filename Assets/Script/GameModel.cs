using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Drawing;
using MyScriptableObjectClass;
public interface IGameModel
{
    IReadOnlyReactiveProperty<GameFlowState> GameState { get; }
    IReadOnlyReactiveProperty<float> GameSpeed { get; }
    IReadOnlyReactiveProperty<float> Score { get; }
    float HighScore { get; }
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
    GameModelSetting _gameModelSetting;
    private readonly ReactiveProperty<GameFlowState> _gameState;
    public IReadOnlyReactiveProperty<GameFlowState> GameState => _gameState;
    private readonly ReactiveProperty<float> _gameSpeed ;
    public IReadOnlyReactiveProperty<float> GameSpeed => _gameSpeed;
    private readonly ReactiveProperty<float> _score;
    public IReadOnlyReactiveProperty<float> Score => _score;
    private SaveData _saveData;
    public float HighScore => _saveData.HighScore;
    public GameModel(GameModelSetting gameModelSetting)
    {
        _gameModelSetting = gameModelSetting;
        _gameState = new(GameFlowState.Inisialize);
        _gameSpeed = new(0f);
        _score = new(0f);
        SaveDataInterface.JsonLoad(out _saveData);
    }
    public void ChangeStateToTitle()
        => _gameState.Value = GameFlowState.Title;
    public void ChangeStateToInGame()
        => _gameState.Value = GameFlowState.InGame;
    public void ChangeStateToResult()
    {
        _gameState.Value = GameFlowState.Result;
        if(_saveData.HighScore < Score.Value)
        {
            _saveData.HighScore = Score.Value;
            SaveDataInterface.JsonSave(_saveData);
        }
    }
    public void ManualUpdate(float deltaTime)
    {
        AddScore(deltaTime);
        AddSpeed(deltaTime);
    }
    public void AddScore(float deltaTime)
    {
        _score.Value += _gameModelSetting.ScoreRatePerSecond * deltaTime;
    }
    public void AddSpeed(float deltaTime)
    {
        _gameSpeed.Value += _gameModelSetting.SpeedUpRate * deltaTime; ;
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

