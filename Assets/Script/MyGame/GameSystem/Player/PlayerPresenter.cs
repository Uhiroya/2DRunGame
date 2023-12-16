using System;
using UniRx;
using VContainer;
using VContainer.Unity;

public interface IPlayerPresenter
{
    IReadOnlyReactiveProperty<PlayerCondition> PlayerState { get; }
    MyCircleCollider Collider { get; }
    void CollisionObstacle(MyCircleCollider obstacle, MyCircleCollider other);
    void OnGameFlowStateChanged(GameFlowState gameFlowState);
    void SetInputX(float x);
    void SetSpeedRate(float speedRate);
}

public class PlayerPresenter : IPlayerPresenter, IPausable, IFixedTickable, IDisposable
{
    private readonly CompositeDisposable _disposable;

    private readonly IPlayerModel _model;
    private readonly IPlayerView _view;
    private float _currentInputX;

    [Inject]
    public PlayerPresenter(IPlayerModel model, IPlayerView view)
    {
        _disposable = new CompositeDisposable();
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
        }
    }

    public void Pause()
    {
        _model.Pause();
    }

    public void Resume()
    {
        _model.Resume();
    }

    public IReadOnlyReactiveProperty<PlayerCondition> PlayerState => _model.PlayerState;

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

    private void Bind()
    {
        //modelとviewのバインド
        _model.PlayerState.Subscribe(x => _view.OnPlayerConditionChanged(x)).AddTo(_disposable);
    }

    private void RegisterEvent()
    {
        _view.OnFinishDeadAnimation += _model.Dead;
    }

    private void UnRegisterEvent()
    {
        _view.OnFinishDeadAnimation -= _model.Dead;
    }

    public void Dying()
    {
        _model.Dying();
    }

    private void InitializePlayer()
    {
        _model.Initialize();
    }

    private void GameStart()
    {
        _model.GameStart();
    }

    private void Reset()
    {
        _model.Reset();
    }
}
