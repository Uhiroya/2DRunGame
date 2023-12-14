using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;
using Cysharp.Threading.Tasks;

public interface IPlayerPresenter
{
    IReadOnlyReactiveProperty<PlayerCondition> PlayerState { get; }
    MyCircleCollider Collider { get; }
    void CollisionObstacle(MyCircleCollider obstacle, MyCircleCollider other);
    void OnGameFlowStateChanged(GameFlowState gameFlowState);
    void SetInputX(float x);
    void SetSpeedRate(float speedRate);

}
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPresenter : IPlayerPresenter, IPauseable, IFixedTickable, System.IDisposable
{
    public IReadOnlyReactiveProperty<PlayerCondition> PlayerState => _model.PlayerState;

    IPlayerModel _model;
    IPlayerView _view;

    CompositeDisposable _disposable;
    float _currentInputX;
    [Inject]
    public PlayerPresenter(IPlayerModel model, IPlayerView view)
    {
        _disposable = new();
        _model = model;
        _view = view;
        RegisterEvent();
        Bind();
    }
    public void Dispose()
    {
        _disposable.Dispose();
        UnRegisterEvent();
    }
    public void FixedTick()
    {
        switch (_model.PlayerState.Value)
        {
            case PlayerCondition.Alive:
                _model.Move(_currentInputX);
                break;
            default:
                break;
        }
    }
    void Bind()
    {
        //modelとviewのバインド
        _model.PlayerState.Subscribe(x => _view.OnPlayerConditionChanged(x)).AddTo(_disposable);
        
    }
    void RegisterEvent()
    {
        _view.OnFinishDeadAnimation += _model.Dead;
    }
    void UnRegisterEvent()
    {
        _view.OnFinishDeadAnimation -= _model.Dead;
    }
    public void CollisionObstacle(MyCircleCollider obstacle, MyCircleCollider other)
    {
        _model.CollisionObstacle(obstacle, other);
    }
    public void OnGameFlowStateChanged(GameFlowState gameFlowState)
    {
        switch (gameFlowState)
        {
            case GameFlowState.Title:
                InitializePlayer();
                break;
            case GameFlowState.GameInitialize:
                Reset();
                GameStart();
                break;
            default:
                break;
        }
    }
    public MyCircleCollider Collider => _model.Collider;
    public void SetInputX(float x)
    {
        _currentInputX = x;
    }
    public void SetSpeedRate(float speedRate)
    {
        _model.SetSpeedRate(speedRate);
    }

    public void Dying()
    {
        _model.Dying();
    }

    public void Pause()
    {
        _model.Pause();
    }

    public void Resume()
    {
        _model.Resume();
    }

    void InitializePlayer()
    {
        _model.Initialize();
    }
    void GameStart()
    {
        _model.GameStart();
    }
    void Reset()
    {
        _model.Reset();
    }
}
