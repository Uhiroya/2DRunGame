using UnityEngine;
using UniRx;
using VContainer;
using VContainer.Unity;
public interface IPlayerPresenter
{
    IReadOnlyReactiveProperty<PlayerCondition> PlayerState { get; }
    Vector2 PlayerPosition { get;}
    void Move(float x);
    void SetSpeedRate(float speedRate);
    void Reset();
    void GameStart();
    void GameOver();

}
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPresenter : IInitializable , IPlayerPresenter
{
    public IReadOnlyReactiveProperty<PlayerCondition> PlayerState => _model.PlayerState;
    Vector2 _playerPosition;
    public Vector2 PlayerPosition => _playerPosition;
    IPlayerModel _model;
    IPlayerView _view;

    CompositeDisposable _disposable;
    [Inject]
    public PlayerPresenter(IPlayerModel model , IPlayerView view)
    {
        _disposable = new();
        _model =  model;
        _view = view;
    }
    ~PlayerPresenter()
    {
        _disposable.Dispose();
    }
    
    public void Initialize()
    {
        Bind();
    }

    public void Bind()
    {
        _model.PositionX.Subscribe(x => _playerPosition = new Vector2 (x, _model.PositionY));
        _model.PlayerState.Where(x => x == PlayerCondition.Waiting).Subscribe(x => _view.OnWaiting()).AddTo(_disposable);   
        _model.PlayerState.Where(x => x == PlayerCondition.Alive).Subscribe(x => _view.OnWalk()).AddTo(_disposable);   
        _model.PlayerState.Where(x => x == PlayerCondition.Dead).Subscribe(x => { _view.OnDead();} ).AddTo(_disposable);   
    }
    public void Reset()
    {
        _model.Reset();
    }
    public void Move(float x)
    {
        _model.Move(x);
    }
    public void SetSpeedRate(float speedRate)
    {
        _model.SetSpeedRate(speedRate);
    }
    public void GameStart()
    {
        _model.SetPlayerCondition(PlayerCondition.Alive);
    }
    public void GameOver()
    {
        _model.SetPlayerCondition(PlayerCondition.Dead);
    }


}
