using System;
using MyScriptableObjectClass;
using UniRx;
using UnityEngine;

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

public class GameModel : IGameModel, IDisposable
{
    /// <summary>
    ///     VContainerから呼び出される
    /// </summary>
    private readonly CompositeDisposable _disposable;

    private readonly GameModelSetting _gameModelSetting;
    private readonly ReactiveProperty<float> _gameSpeed;

    private readonly ReactiveProperty<GameFlowState> _gameState;
    private readonly ReactiveProperty<float> _highScore;
    private readonly SaveData _saveData;
    private readonly ReactiveProperty<float> _score;
    private float _currentSpeed;

    public GameModel(GameModelSetting gameModelSetting)
    {
        SaveDataInterface.LoadJson(out _saveData);
        _gameModelSetting = gameModelSetting;
        _gameState = new ReactiveProperty<GameFlowState>(GameFlowState.Initialize);
        _highScore = new ReactiveProperty<float>(_saveData.HighScore);
        _gameSpeed = new ReactiveProperty<float>(0f);
        _score = new ReactiveProperty<float>(0f);
        _disposable = new CompositeDisposable();
        Bind();
    }

    public void Dispose()
    {
        _disposable.Dispose();
    }

    public IReadOnlyReactiveProperty<float> HighScore => _highScore;
    public IReadOnlyReactiveProperty<GameFlowState> GameState => _gameState;
    public IReadOnlyReactiveProperty<float> GameSpeed => _gameSpeed;
    public IReadOnlyReactiveProperty<float> Score => _score;

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
        }
    }

    public void GoTitle()
    {
        ChangeState(GameFlowState.Title);
    }

    public void GameStart()
    {
        ChangeState(GameFlowState.GameInitialize);
    }

    public void Pause()
    {
        ChangeState(GameFlowState.Pause);
    }

    public void Resume()
    {
        ChangeState(GameFlowState.InGame);
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

    private void Bind()
    {
        GameState.Subscribe(OnGameStateChanged).AddTo(_disposable);
    }

    private void OnGameStateChanged(GameFlowState gameFlowState)
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
            //PlayerGameStartのタイミングより早くInGameに遷移させてしまう可能性があるため、フェーズを分離。
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
        }
    }

    private void AddScoreByDistance(float deltaTime)
    {
        _score.Value += _gameModelSetting.ScoreRatePerSecond * deltaTime;
    }

    private void AddSpeedByDistance(float deltaTime)
    {
        _gameSpeed.Value += _gameModelSetting.SpeedUpRate * deltaTime;
    }

    private void ChangeState(GameFlowState state)
    {
#if UNITY_EDITOR
        Debug.Log(state);
#endif
        _gameState.Value = state;
    }
}
