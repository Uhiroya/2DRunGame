using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Cysharp.Threading.Tasks;

public interface IGamePresenter
{
    public void GoTitle();
    public void GameStart();
}
public class GamePresenter : IInitializable ,IStartable ,ITickable , System.IDisposable, IGamePresenter
{
    /// <summary>
    /// VContainerで注入される
    /// </summary>
    IGameModel _model;
    IGameView _view;
    IPlayerPresenter _playerPresenter;
    IObstacleManager _obstacleManager;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public GamePresenter(IGameModel model , IGameView view 
        ,IPlayerPresenter playerPresenter, IObstacleManager obstacleGenerator)
    {
        _model = model ;
        _view = view ;
        _playerPresenter = playerPresenter;
        _obstacleManager = obstacleGenerator;
    }
    /// <summary>
    /// その他メンバ変数
    /// </summary>
    CompositeDisposable _disposable;
    GameObject _hitEffect;
    /// <summary>
    ///VContainerからPlayerLoop.Initializationの前に呼ばれる
    /// </summary>
    public void Initialize()
    {
        _disposable = new();
        Bind();
    }
    public void Start()
    {
        _model.ChangeStateToTitle();
    }
    /// <summary>
    /// VContainerから呼び出される
    /// </summary>
    public void Dispose()
    {
        _disposable.Dispose();
    }
    /// <summary>
    /// バインド
    /// </summary>
    private void Bind()
    {
        //modelと
        _model.GameSpeed
            .Subscribe(
                x =>
                {
                    _view.SetUVSpeed(x);
                    _playerPresenter.SetSpeedRate(x);
                })
            .AddTo(_disposable);

        _model.Score
            .Subscribe(
                x =>
                    _view.SetScore(x)
                )
            .AddTo(_disposable);
        //ゲームフローの状態による命令
        _model.GameState
            .Subscribe(
                x =>
                {
                    switch (x)
                    {
                        case GameFlowState.Title:
                            _view.ShowHighScore(_model.HighScore);
                            break;
                        case GameFlowState.InGame:
                            _playerPresenter.GameStart();
                            break;
                        case GameFlowState.Result:
                            if (_hitEffect != null)
                            {
                                Object.Destroy(_hitEffect);
                                _hitEffect = null;
                            }
                            _playerPresenter.Reset();
                            _obstacleManager.Reset();
                            _view.ShowResultUI();
                            break;
                        default:
                            break;
                    }
                })
            .AddTo(_disposable);
        //衝突判定
        _obstacleManager.ObstaclePosition.
            ObserveReplace()
            .Where(x =>
            {
                return Vector2.Distance(x.NewValue, _playerPresenter.PlayerPosition) 
                        < x.Key.ObstacleInfo.HitRange + _playerPresenter.PlayerHitRange;
            })
            .Subscribe(x => HitObstacle(x.Key));
        _playerPresenter.PlayerDeath += OnPlayerDeath;
    }
    /// <summary>
    ///VContainerからUpdateのタイミングで呼ばれる
    /// </summary>
    public void Tick()
    {
        switch (_playerPresenter.PlayerState.Value)
        {
            case PlayerCondition.Alive:
                _view.ManualUpdate(Time.deltaTime);
                _model.ManualUpdate(Time.deltaTime);
                _obstacleManager.UpdateObstacleMove(Time.deltaTime, _model.GameSpeed.Value);
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
        
        switch (obstacle.ObstacleInfo.ItemType)
        {
            case ItemType.Item:
                _model.AddItemScore(obstacle.ObstacleInfo.Score);
                break;
            case ItemType.Enemy:
                _playerPresenter.HitObject();
                obstacle.InstantiateDestroyEffect();
                break;
            default:
                break;
        }
        _obstacleManager.HitObstacle(in obstacle);
    }

    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void GoTitle()
    {
        _model.ChangeStateToTitle();
    }
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void GameStart()
    {
        _model.ChangeStateToInGame();
    }
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void OnPlayerDeath()
    {
        _model.ChangeStateToResult();
        _view.ShowResultScore(_model.Score.Value);
    }
}
