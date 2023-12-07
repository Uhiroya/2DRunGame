using UniRx;
using UnityEngine;
using VContainer.Unity;
using Cysharp.Threading.Tasks;

public interface IGamePresenter
{
    GameFlowState NowGameState { get; }
    public void GoTitle();
    public void GameStart();
}
public class GamePresenter : IGamePresenter, IPauseable, IInitializable, IStartable, ITickable, System.IDisposable
{
    /// <summary>
    /// VContainerで注入される
    /// </summary>
    IGameModel _model;
    IGameView _view;
    IPlayerPresenter _playerPresenter;
    IObstacleManager _obstacleManager;
    /// <summary>
    /// メンバ変数
    /// </summary>
    CompositeDisposable _disposable;
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
    /// 公開プロパティ、メソッド
    /// </summary>
    public GameFlowState NowGameState => _model.GameState.Value;
    /// <summary>ボタンから呼び出される。</summary>
    public void GoTitle()
    {
        _model.GoTitle();
    }
    /// <summary>ボタンから呼び出される。</summary>
    public void GameStart()
    {
        _model.GameStart();
    }
    /// <summary>
    ///VContainerから呼び出される
    /// </summary>
    public void Initialize()
    {
        _disposable = new();
        Bind();
    }
    public void Start()
    {
        _model.GoTitle();
    }
    public void Tick()
    {
        switch (_model.GameState.Value)
        {
            case GameFlowState.InGame:
                _view.ManualUpdate(Time.deltaTime);
                _model.ManualUpdate(Time.deltaTime);
                _obstacleManager.UpdateObstacleMove(Time.deltaTime, _model.GameSpeed.Value);
                break;
            default:
                break;
        }
    }
    public void Dispose()
    {
        _disposable.Dispose();
    }
    /// <summary>
    /// バインド
    /// </summary>
    void Bind()
    {
        //modelとviewのバインド
        _model.HighScore
            .Subscribe(
                x =>
                {
                    _view.SetHighScore(x);
                })
            .AddTo(_disposable);
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
                {
                    _view.SetScore(x);
                })
            .AddTo(_disposable);

        //ゲームフローの状態による命令
        _model.GameState
            .Subscribe(
                x =>
                {
                    _view.OnGameFlowStateChanged(x);
                    _playerPresenter.OnGameFlowStateChanged(x);
                    _obstacleManager.OnGameFlowStateChanged(x);
                })
            .AddTo(_disposable);

        //プレイヤーの状態を監視して現在のゲームの状態を変更する
        _playerPresenter.PlayerState
            .Subscribe(
            x =>
            {
                _model.OnPlayerConditionChanged(x);
            })
            .AddTo(_disposable);

        //衝突判定
        _obstacleManager.ObstaclePosition.
            ObserveReplace()
            .Where(x =>
            {
                //Modelで判定するべき？
                return Vector2.Distance(x.NewValue, _playerPresenter.PlayerPosition)
                        < x.Key.ObstacleInfo.HitRange + _playerPresenter.PlayerHitRange;
            })
            .Subscribe(x => HitObstacle(x.Key))
            .AddTo(_disposable);
    }
    /// <summary>
    /// 衝突時の処理。
    /// </summary>
    void HitObstacle(IObstaclePresenter obstacle)
    {
        switch (obstacle.ObstacleInfo.ItemType)
        {
            case ItemType.Item:
                _model.AddItemScore(obstacle.ObstacleInfo.Score);
                break;
            case ItemType.Enemy:
                _playerPresenter.Dying();
                obstacle.InstantiateDestroyEffect();
                break;
            default:
                break;
        }
        _obstacleManager.HitObstacle(in obstacle);
    }
    public void Pause()
    {
        _model.Pause();
        _view.Pause();
    }

    public void Resume()
    {
        _model.Resume();
        _view.Resume();
    }
}
