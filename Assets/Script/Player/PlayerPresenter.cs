using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using VContainer;
using VContainer.Unity;


public interface IPlayerPresenter
{
    IReadOnlyReactiveProperty<PlayerCondition> PlayerState { get; }
    public void Move(float x);
    public void SetSpeedRate(float speedRate);
    public void GameStart();
    public void GameOver();

}
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPresenter : IInitializable , IPlayerPresenter
{
    public IReadOnlyReactiveProperty<PlayerCondition> PlayerState => _model.PlayerState;
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
        Debug.Log("Initialize");
        //“ü—Í‚ÌŽó‚¯Žæ‚è

        Bind();
    }

    public void Bind()
    {
        _model.PlayerState.Where(x => x == PlayerCondition.Waiting).Subscribe(x => _view.OnWaiting()).AddTo(_disposable);   
        _model.PlayerState.Where(x => x == PlayerCondition.Alive).Subscribe(x => _view.OnWalk()).AddTo(_disposable);   
        _model.PlayerState.Where(x => x == PlayerCondition.Dead).Subscribe(x => { _view.OnDead(); _model.Reset(); } ).AddTo(_disposable);   
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
