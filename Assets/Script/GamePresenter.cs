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
public class GamePresenter : IInitializable , IStartable ,ITickable , System.IDisposable, IGamePresenter
{
    /// <summary>
    /// VContainerで注入される
    /// </summary>
    Transform _parentTransform;
    IGameModel _model;
    IGameView _view;
    IPlayerPresenter _playerPresenter;
    IObstacleGenerator _obstaclePresenter;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public GamePresenter(Transform parentTransform 
        ,IGameModel model , IGameView view ,IPlayerPresenter playerPresenter, IObstacleGenerator obstaclePresenter)
    {
        _parentTransform  = parentTransform;
        _model = model ;
        _view = view ;
        _playerPresenter = playerPresenter;
        _obstaclePresenter = obstaclePresenter;
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
    /// <summary>
    /// インスタンスを生成する人にDisposeしてもらう.。
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
                }
                )
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
                        case GameFlowState.InGame:
                            _model.GameStart();
                            _playerPresenter.GameStart();
                            break;
                        case GameFlowState.Result:
                            if (_hitEffect != null)
                            {
                                Object.Destroy(_hitEffect);
                                _hitEffect = null;
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
        //衝突判定
        _obstaclePresenter.ObstaclePosition.
            ObserveReplace()
            .Where(x =>
            {
                return Vector2.Distance(x.NewValue, _playerPresenter.PlayerPosition) 
                        < x.Key.ObstacleData.HitRange + _playerPresenter.PlayerHitRange;
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
        _obstaclePresenter.Release(obstacle);
        switch (obstacle.ObstacleData.Param.ItemType)
        {
            case ObstacleType.Item:
                _model.AddScore(obstacle.ObstacleData.Score);
                break;
            case ObstacleType.Enemy:
                _playerPresenter.HitObject();
                _hitEffect = Object.Instantiate(obstacle.ObstacleData.DestroyEffect
                    , _playerPresenter.PlayerPosition, Quaternion.identity, _parentTransform);
                break;
            default:
                break;
        }
        
    }

    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void GoTitle()
    {
        _model.ChangeStateToTitle();
        _view.TitleStart();
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

    public async void Start()
    {
        await _view.TitleStart();
    }
}
