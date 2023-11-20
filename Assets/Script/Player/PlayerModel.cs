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
        PositionX
            .Skip(1)
            .Subscribe(x => { ClumpX(); })
            .AddTo(_disposable);
        _playerState = new(PlayerCondition.Waiting);
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

    public void SetSpeedRate(float speedRate)
    {
        _speedRate = speedRate;
    }
    public void Move(float x)
    {
        _rb.velocity = new Vector2(x * _defaultSpeed * _speedRate, 0f);
        PositionX.Value = _rb.transform.position.x;
    }
    public void Reset()
    {
        _rb.velocity = Vector3.zero;
        _rb.transform.position = new Vector3(InGameConst.WindowWidth / 2, 0f, 0f);
        PositionX.Value = InGameConst.WindowWidth / 2;
        _playerState.Value = 0f;
    }
    /// <summary>
    /// à⁄ìÆêßå¿
    /// </summary>
    private void ClumpX()
    {
        var clampX = Mathf.Clamp(_rb.transform.position.x, InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin);
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
