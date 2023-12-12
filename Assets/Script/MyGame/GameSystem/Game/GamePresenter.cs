using UniRx;
using UnityEngine;
using VContainer.Unity;
using Cysharp.Threading.Tasks;

public interface IGamePresenter
{
    GameFlowState NowGameState { get; }
    public void PressReturnButton();
    public void PressStartButton();
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
    ICollisionChecker _collisionChecker;

    /// <summary>
    /// メンバ変数
    /// </summary>
    CompositeDisposable _disposable;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public GamePresenter(IGameModel model , IGameView view 
        ,IPlayerPresenter playerPresenter, IObstacleManager obstacleGenerator 
        , ICollisionChecker collisionChecker)
    {
        _model = model ;
        _view = view ;
        _playerPresenter = playerPresenter;
        _obstacleManager = obstacleGenerator;
        _collisionChecker = collisionChecker;
    }
    /// <summary>
    /// 公開プロパティ、メソッド
    /// </summary>
    public GameFlowState NowGameState => _model.GameState.Value;

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
                _collisionChecker.ManualUpdate();
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
        BindOvserve();
        BindEvent();
    }
    void BindOvserve()
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
    }
    void BindEvent()
    {
        _collisionChecker.OnCollisionEnter += CollisionObstacle;
        _obstacleManager.OnCollisionItemEvent += _model.AddItemScore;
        _obstacleManager.OnCollisionItemEvent += (x) => _view.PlayHitItemSound();
        _obstacleManager.OnCollisionEnemyEvent += _playerPresenter.Dying;
        _obstacleManager.OnCollisionEnemyEvent += _view.PlayHitEnemySound;
    }
    void UnBindEvent()
    {
        _collisionChecker.OnCollisionEnter -= CollisionObstacle;
        _obstacleManager.OnCollisionItemEvent -= _model.AddItemScore;
        _obstacleManager.OnCollisionItemEvent -= (x) => _view.PlayHitItemSound();
        _obstacleManager.OnCollisionEnemyEvent -= _playerPresenter.Dying;
        _obstacleManager.OnCollisionEnemyEvent -= _view.PlayHitEnemySound;
    }
    /// <summary>
    /// 衝突時に呼び出される。
    /// </summary>
    void CollisionObstacle(MyCircleCollider collider, CollisionTag collisionTag)
    {
        _obstacleManager.CollisionObstacle(collider , collisionTag);
    }
    /// <summary>ボタンから呼び出される。</summary>
    public void PressReturnButton()
    {
        _model.GoTitle();
        _view.PlayButtonSound();
    }
    /// <summary>ボタンから呼び出される。</summary>
    public void PressStartButton()
    {
        _model.GameStart();
        _view.PlayButtonSound();
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
