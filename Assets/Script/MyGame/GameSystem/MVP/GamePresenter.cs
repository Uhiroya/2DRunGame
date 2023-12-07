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
        _model.ChangeState(GameFlowState.Title);
    }
    /// <summary>ボタンから呼び出される。</summary>
    public void GameStart()
    {
        _model.ChangeState(GameFlowState.GameStart);
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
        _model.ChangeState(GameFlowState.Title);
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
                    switch (x)
                    {
                        case GameFlowState.Title:
                            _view.ShowHighScore(_model.HighScore);
                            _playerPresenter.InitializePlayer();
                            break;
                        case GameFlowState.GameStart:
                            _playerPresenter.GameStart();
                            _model.ChangeState(GameFlowState.InGame);
                            break;
                        case GameFlowState.Result:
                            _playerPresenter.Reset();
                            _obstacleManager.Reset();
                            _view.ShowResultUI();
                            break;
                        default:
                            break;
                    }
                })
            .AddTo(_disposable);

        //プレイヤーの状態を監視して現在のゲームの状態を変更する
        _playerPresenter.PlayerState
            .Subscribe(
            x =>
            {
                switch(x)
                {
                    case PlayerCondition.OnDead:
                        _model.ChangeState(GameFlowState.Waiting);
                        break;
                    case PlayerCondition.Dead:                       
                        _view.ShowResultScore(_model.Score.Value);
                        _model.ChangeState(GameFlowState.Result);
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
                _playerPresenter.HitObject();
                obstacle.InstantiateDestroyEffect();
                break;
            default:
                break;
        }
        _obstacleManager.HitObstacle(in obstacle);
    }
    public void Pause()
    {
        _model.ChangeState(GameFlowState.Pause);
        _view.Pause();
    }

    public void Resume()
    {
        _model.ChangeState(GameFlowState.InGame);
        _view.Resume();
    }
}
