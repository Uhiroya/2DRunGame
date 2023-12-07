using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;
using Cysharp.Threading.Tasks;

public interface IPlayerPresenter
{
    IReadOnlyReactiveProperty<PlayerCondition> PlayerState { get; }
    Vector2 PlayerPosition { get; }
    float PlayerHitRange { get; }
    void OnGameFlowStateChanged(GameFlowState gameFlowState);
    void SetInputX(float x);
    void SetSpeedRate(float speedRate);
    void Dying();

}
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPresenter : IPlayerPresenter, IPauseable, IFixedTickable, System.IDisposable
{
    public IReadOnlyReactiveProperty<PlayerCondition> PlayerState => _model.PlayerState;
    public float PlayerHitRange => _model.PlayerHitRange;
    Vector2 _playerPosition;
    public Vector2 PlayerPosition => _playerPosition;
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
        //プレイヤー移動の監視
        _model.PositionX.Subscribe(x => _playerPosition = new Vector2(x, _model.PositionY)).AddTo(_disposable);
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
                GameStart();
                break;
            case GameFlowState.Result:
                Reset();
                break;
            default:
                break;
        }
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
