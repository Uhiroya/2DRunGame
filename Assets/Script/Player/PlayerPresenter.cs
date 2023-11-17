using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPresenter : MonoBehaviour
{
    [SerializeField] PlayerModel _model;
    [SerializeField] PlayerView _view;
    public IReadOnlyReactiveProperty<PlayerCondition> PlayerState  => _model.PlayerState;
    CompositeDisposable _disposable;
    void OnEnable()
    {
        Inisialize();
    }
    public void Inisialize()
    {
        _disposable = new();
        this.UpdateAsObservable()
             .Where(x => _model.PlayerState.Value == PlayerCondition.Alive)
             .Select(x => Input.GetAxis("Horizontal"))
             .Subscribe(x => _model.Move(x))
             .AddTo(_disposable);
    }
    public void Start()
    {
        Bind();
    }
    public void Bind()
    {
        _model.PlayerState.Where(x => x == PlayerCondition.Waiting).Subscribe(x => _view.OnWaiting());   
        _model.PlayerState.Where(x => x == PlayerCondition.Alive).Subscribe(x => _view.OnWalk());   
        _model.PlayerState.Where(x => x == PlayerCondition.Dead).Subscribe(x => { _view.OnDead(); _model.Reset(); } );   
    }
    void OnDisable()
    {
        _disposable.Dispose();
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
