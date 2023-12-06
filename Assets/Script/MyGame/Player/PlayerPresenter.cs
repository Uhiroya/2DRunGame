using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;
using Cysharp.Threading.Tasks;

public interface IPlayerPresenter
{
    IReadOnlyReactiveProperty<PlayerCondition> PlayerState { get; }
    Vector2 PlayerPosition { get;}
    float PlayerHitRange { get;}
    void SetInputX(float x);
    void SetSpeedRate(float speedRate);
    void Reset();
    void InitializePlayer();
    void GameStart();
    void HitObject();

}
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPresenter : IPlayerPresenter ,IPauseable , IFixedTickable, System.IDisposable
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
    public PlayerPresenter(IPlayerModel model , IPlayerView view)
    {
        _disposable = new();
        _model =  model;
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

    public void Bind()
    {
        //プレイヤー移動の監視
        _model.PositionX.Subscribe(x => _playerPosition = new Vector2 (x, _model.PositionY)).AddTo(_disposable);
        _model.PlayerState
            .Subscribe(
            async x =>
            {
                switch(x)
                {
                    case PlayerCondition.Initialize:
                        _view.OnInitialize();
                        break;
                    case PlayerCondition.Pause:
                        _view.OnWaiting();
                        break;
                    case PlayerCondition.Alive:
                        _view.OnWalk();
                        break;
                    case PlayerCondition.OnDead:
                        await _view.OnDead();
                        _model.SetPlayerCondition(PlayerCondition.Dead);
                        break;
                    default:
                        break;
                }
            }).AddTo(_disposable);
    }
    public void Reset()
    {
        _model.Reset();
    }
    public void SetInputX(float x)
    {
        _currentInputX = x;
    }
    public void SetSpeedRate(float speedRate)
    {
        _model.SetSpeedRate(speedRate);
    }
    public void InitializePlayer()
    {
        _model.SetPlayerCondition(PlayerCondition.Initialize);
    }
    public void GameStart()
    {
        _model.SetPlayerCondition(PlayerCondition.Alive);
    }
    public void HitObject()
    {
        _model.SetPlayerCondition(PlayerCondition.OnDead);
    }

    public void Pause()
    {
        _model.SetPlayerCondition(PlayerCondition.Pause);
    }

    public void Resume()
    {
        _model.SetPlayerCondition(PlayerCondition.Alive);
    }
}
