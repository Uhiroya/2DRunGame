using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;
using Cysharp.Threading.Tasks;

public interface IPlayerPresenter
{
    IReadOnlyReactiveProperty<PlayerCondition> PlayerState { get; }
    MyCircleCollider GetCollider();
    void OnGameFlowStateChanged(GameFlowState gameFlowState);
    void SetInputX(float x);
    void SetSpeedRate(float speedRate);
    void Dying();

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
        Bind();
    }
    public void Dispose()
    {
        _disposable.Dispose();
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
        _view.OnFinishDeadAnimation += _model.Dead;
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
    public MyCircleCollider GetCollider()
    {
        return _model.GetCollider();
    }
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
