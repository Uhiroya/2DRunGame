using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Playables;
using VContainer;
using VContainer.Unity;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GamePresenter : MonoBehaviour
{
    [SerializeField] Transform _parentTransform;
    [SerializeField] float _scoreRatePerSecond = 30f;
    [SerializeField] float _speedUpRate = 0.01f;
    [SerializeField] GameModel _model;
    [SerializeField] GameView _view;
    [Inject] IPlayerPresenter _playerPresenter;
    [Inject] IObstacleGenerator _obstaclePresenter;
    GameObject _deathEffect;
    CompositeDisposable _disposable = new();
    public GamePresenter(IPlayerPresenter playerPresenter , IObstacleGenerator obstaclePresenter) 
    {
        _playerPresenter = playerPresenter;
        _obstaclePresenter = obstaclePresenter;
        _disposable = new();
    }
    private void Start()
    {
        Initialize();
    }
    /// <summary>初期化を行う</summary>
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
            .AddTo(this);
        _model.Score
            .Subscribe(
                x => 
                    _view.SetScore(x)
                )
            .AddTo(this);
        _model.GameState
            .Subscribe(
                x => 
                {
                    switch (x)
                    {
                        case GameFlowState.InGame:
                            _playerPresenter.GameStart();
                            break;
                        case GameFlowState.Result:
                            if(_deathEffect != null)
                            {
                                Object.Destroy(_deathEffect);
                                _deathEffect = null;
                            }
                            _playerPresenter.Reset();
                            _obstaclePresenter.Reset();
                            _view.ShowResultUI();
                            break;
                        default: 
                            break;
                    } 
                })
            .AddTo(this);
        _obstaclePresenter.ObstaclePosition.
            ObserveReplace()
            .Where(x => Vector2.Distance(x.NewValue, _playerPresenter.PlayerPosition) < x.Key.ObstacleData.HitRange)
            .Subscribe(x => HitObstacle(x.Key));
    }
    public void HitObstacle(IObstaclePresenter obstacle)
    {
        switch (obstacle.ObstacleData.Param.ItemType)
        {
            case ObstacleType.Item:
                AddScore(obstacle.ObstacleData.Score);
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
    void Update()
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
    public void AddScore(float score)
    {
        _model.AddScore(score);
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
