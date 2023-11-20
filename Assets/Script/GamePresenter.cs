using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
public interface IGamePresenter
{
    public void ShowResultScore();
    public void ChangeStateToTitle();
    public void ChangeStateToInGame();
    public void ChangeStateToResult();
}
public class GamePresenter : IInitializable , ITickable , IGamePresenter
{
    Transform _parentTransform;
    float _scoreRatePerSecond;
    float _speedUpRate;
    IGameModel _model;
    IGameView _view;
    IPlayerPresenter _playerPresenter;
    IObstacleGenerator _obstaclePresenter;
    
    CompositeDisposable _disposable;
    public GamePresenter(Transform parentTransform , float scoreRatePerSecond , float speedUpRate
        ,IGameModel model , IGameView view ,IPlayerPresenter playerPresenter, IObstacleGenerator obstaclePresenter)
    {
        _parentTransform  = parentTransform;
        _scoreRatePerSecond = scoreRatePerSecond;
        _speedUpRate = speedUpRate;
        _model = model ;
        _view = view ;
        _playerPresenter = playerPresenter;
        _obstaclePresenter = obstaclePresenter;
        _disposable = new();
    }
    GameObject _deathEffect;
    public void Initialize()
    {
        Bind();
        _model.SetGameState(GameFlowState.Inisialize);
    }
    /// <summary>
    /// バインド
    /// </summary>
    private void Bind()
    {
        _model.GameSpeed
            .Subscribe(
                x =>
                {
                    _view.SetUVSpeed(x);
                    _playerPresenter.SetSpeedRate(x);
                }
                )
            .AddTo(_disposable);

        _model.Score
            .Subscribe(
                x =>
                    _view.SetScore(x)
                )
            .AddTo(_disposable);

        _model.GameState
            .Subscribe(
                x =>
                {
                    switch (x)
                    {
                        case GameFlowState.InGame:
                            _model.GameStart();
                            _playerPresenter.GameStart();
                            break;
                        case GameFlowState.Result:
                            if (_deathEffect != null)
                            {
                                Object.Destroy(_deathEffect);
                                _deathEffect = null;
                            }
                            _playerPresenter.Reset();
                            _obstaclePresenter.Reset();
                            _model.GameStop();
                            _view.ShowResultUI();
                            break;
                        default:
                            break;
                    }
                })
            .AddTo(_disposable);

        _obstaclePresenter.ObstaclePosition.
            ObserveReplace()
            .Where(x => Vector2.Distance(x.NewValue, _playerPresenter.PlayerPosition) < x.Key.ObstacleData.HitRange)
            .Subscribe(x => HitObstacle(x.Key));
    }
    public void Tick()
    {
        switch (_playerPresenter.PlayerState.Value)
        {
            case PlayerCondition.Alive:
                _view.ManualUpdate(Time.deltaTime);
                _model.AddScore(_scoreRatePerSecond * Time.deltaTime);
                _model.AddSpeed(_speedUpRate * Time.deltaTime);
                _obstaclePresenter.UpdateObstacleMove(Time.deltaTime, _model.GameSpeed.Value);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 衝突判定
    /// </summary>
    /// <param name="obstacle"></param>
    public void HitObstacle(IObstaclePresenter obstacle)
    {
        switch (obstacle.ObstacleData.Param.ItemType)
        {
            case ObstacleType.Item:
                _model.AddScore(obstacle.ObstacleData.Score);
                break;
            case ObstacleType.Enemy:
                _playerPresenter.GameOver();
                _deathEffect = Object.Instantiate(obstacle.ObstacleData.DestroyEffect
                    , _playerPresenter.PlayerPosition, Quaternion.identity, _parentTransform);
                break;
            default:
                break;
        }
        _obstaclePresenter.Release(obstacle);
    }

    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ShowResultScore()
        => _view.ShowResultScore(_model.Score.Value);
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ChangeStateToTitle()
        => _model.SetGameState(GameFlowState.Title);
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ChangeStateToInGame()
        => _model.SetGameState(GameFlowState.InGame);
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ChangeStateToResult()
        => _model.SetGameState(GameFlowState.Result);
}
