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
public class GamePresenter : IInitializable , ITickable , System.IDisposable, IGamePresenter
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
    GameObject _deathEffect;
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
        //衝突判定
        _obstaclePresenter.ObstaclePosition.
            ObserveReplace()
            .Where(x => Vector2.Distance(x.NewValue, _playerPresenter.PlayerPosition) < x.Key.ObstacleData.HitRange)
            .Subscribe(x => HitObstacle(x.Key));
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
        => _model.ChangeStateToTitle();
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ChangeStateToInGame()
        => _model.ChangeStateToInGame();
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ChangeStateToResult()
        => _model.ChangeStateToResult();


}
