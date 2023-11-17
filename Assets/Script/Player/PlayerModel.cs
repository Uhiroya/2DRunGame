using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[System.Serializable]
public class PlayerModel
{
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] Collider2D _col;
    [SerializeField] float _defaultSpeed = 500f;
    private float _speedRate = 1f;
    public readonly ReactiveProperty<float> PositionX;
    private readonly ReactiveProperty<PlayerCondition> _playerState;
    public IReadOnlyReactiveProperty<PlayerCondition> PlayerState => _playerState;
    public PlayerModel()
    {
        PositionX = new(0f);
        _playerState = new(PlayerCondition.Waiting);
        PositionX
            .Skip(1)
            .Subscribe(x  => { ClumpX(); })
            .AddTo(_disposable);
        PlayerState.Where(x => x == PlayerCondition.Alive).Subscribe(x => _col.enabled = true).AddTo(_disposable);
        PlayerState.Where(x => x == PlayerCondition.Dead).Subscribe(x => _col.enabled = false).AddTo(_disposable);
    }
    CompositeDisposable _disposable = new();
    ~PlayerModel()
    {
        _disposable.Dispose();
    }
    public void SetPlayerCondition(PlayerCondition condition)
    {
        _playerState.Value = condition;
    }
    public void Reset()
    {
        //_playerState.Value = PlayerCondition.Waiting;
        _rb.velocity = Vector3.zero;
        _rb.transform.position = new Vector3(Screen.width / 2, 0f, 0f);
        PositionX.Value = Screen.width / 2;
        _playerState.Value = 0f;
    }
    public void SetSpeedRate(float speedRate)
    {
        _speedRate = speedRate;
    }
    public void Move(float x)
    {
        _rb.velocity = new Vector2(x * _defaultSpeed * _speedRate, 0f);
        PositionX.Value = _rb.transform.position.x;
    }
    /// <summary>
    /// à⁄ìÆêßå¿
    /// </summary>
    public void ClumpX()
    {
        var clampX = Mathf.Clamp(_rb.transform.position.x, GamePresenter.MapXMargin, Screen.width - GamePresenter.MapXMargin);
        _rb.transform.position = new Vector2(clampX, _rb.transform.position.y);
    }


}

public enum PlayerCondition
{
    None,
    Waiting,
    Alive,
    Dead,
}
