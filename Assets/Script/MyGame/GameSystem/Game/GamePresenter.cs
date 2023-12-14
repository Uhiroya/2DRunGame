using UniRx;
using UnityEngine;
using VContainer.Unity;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

public interface IGamePresenter
{
    GameFlowState NowGameState { get; }
}
public class GamePresenter : IGamePresenter, IPauseable,IStartable, ITickable, System.IDisposable
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
        _disposable = new();
        Bind();
        RegisterEvent();
    }
    /// <summary>
    /// 公開プロパティ、メソッド
    /// </summary>
    public GameFlowState NowGameState => _model.GameState.Value;
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
        UnBind();
        UnRegisterEvent();
    }
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
    }
    void UnBind()
    {
        _disposable.Dispose();
    }
    void RegisterEvent()
    {
        _view.OnPressStart += _model.GameStart;
        _view.OnPressRestart += _model.GameStart;
        _view.OnPressReturn += _model.GoTitle;
        _collisionChecker.OnCollisionEnter += CollisionObstacle;
        _obstacleManager.OnCollisionItem += OnCollisionItem;
        _obstacleManager.OnCollisionEnemy += OnCollisionEnemy;
    }
    void UnRegisterEvent()
    {
        _view.OnPressStart -= _model.GameStart;
        _view.OnPressRestart -= _model.GameStart;
        _view.OnPressReturn -= _model.GoTitle;
        _collisionChecker.OnCollisionEnter -= CollisionObstacle;
        _obstacleManager.OnCollisionItem -= OnCollisionItem;
        _obstacleManager.OnCollisionEnemy -= OnCollisionEnemy;
    }
    /// <summary>
    /// 衝突時に呼び出される。
    /// </summary>
    void CollisionObstacle(MyCircleCollider obstacle, MyCircleCollider other)
    {
        _obstacleManager.CollisionObstacle(obstacle, other);
        _playerPresenter.CollisionObstacle(obstacle, other);
    }
    void OnCollisionItem(float score)
    {
        _model.AddItemScore(score);
    }
    void OnCollisionEnemy()
    {
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
