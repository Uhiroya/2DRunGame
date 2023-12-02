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
    event System.Action PlayerDeath;
    void SetInputX(float x);
    
    void SetSpeedRate(float speedRate);
    void Reset();
    void GameStart();
    void HitObject();

}
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPresenter : IInitializable , IFixedTickable , IPlayerPresenter , System.IDisposable
{
    public IReadOnlyReactiveProperty<PlayerCondition> PlayerState => _model.PlayerState;
    public event System.Action PlayerDeath;
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
    }
    public void Dispose()
    {
        _disposable.Dispose();
    }
    public void Initialize()
    {
        Bind();
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
        _model.PlayerState.Where(x => x == PlayerCondition.Waiting)
             .Subscribe(x => _view.OnWaiting())
             .AddTo(_disposable);
        _model.PlayerState.Where(x => x == PlayerCondition.Alive)
            .Subscribe(x => _view.OnWalk())
            .AddTo(_disposable);   

        _model.PlayerState.Where(x => x == PlayerCondition.OnDead)
            .Subscribe(async x => 
            {
                await _view.OnDead();
                GameOver();
            })
            .AddTo(_disposable);   
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
    public void GameStart()
    {
        _model.SetPlayerCondition(PlayerCondition.Alive);
    }
    public void HitObject()
    {
        _model.SetPlayerCondition(PlayerCondition.OnDead);
    }
    public void GameOver()
    {
        PlayerDeath?.Invoke();
    }


}
