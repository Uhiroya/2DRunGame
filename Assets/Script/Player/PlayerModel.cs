using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
public interface IPlayerModel
{
    IReadOnlyReactiveProperty<PlayerCondition> PlayerState { get; }
    float PositionY { get;}
    IReadOnlyReactiveProperty<float> PositionX { get; }
    void SetPlayerCondition(PlayerCondition condition);
    void SetSpeedRate(float speedRate);
    void Move(float x);
    void Reset();
}

public class PlayerModel : IPlayerModel
{
    Transform _transform;
    float _defaultSpeed = 500f;
    private float _speedRate = 1f;
    private float _positionY;
    public float PositionY => _positionY ;
    public readonly ReactiveProperty<float> _positionX;
    public IReadOnlyReactiveProperty<float> PositionX => _positionX;
    private readonly ReactiveProperty<PlayerCondition> _playerState;
    public IReadOnlyReactiveProperty<PlayerCondition> PlayerState => _playerState;
    public PlayerModel(Transform transform  ,float defaultSpeed)
    {
        _transform = transform;
        _positionY = _transform.transform.position.y;
        _defaultSpeed = defaultSpeed;
        _positionX = new(0f);
        _positionX
            .Skip(1)
            .Subscribe(x => { ClumpX(); })
            .AddTo(_disposable);
        _playerState = new(PlayerCondition.Waiting);    }
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
        _transform.position += new Vector3(x * _defaultSpeed * _speedRate, 0f);
        _positionX.Value = _transform.transform.position.x;
    }
    public void Reset()
    {
        _transform.position = new Vector3(InGameConst.WindowWidth / 2, 0f, 0f);
        _positionX.Value = InGameConst.WindowWidth / 2;
        _playerState.Value = 0f;
    }
    /// <summary>
    /// 移動制限
    /// </summary>
    void ClumpX()
    {
        var clampX = Mathf.Clamp(_transform.transform.position.x, InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin);
        _transform.transform.position = new Vector2(clampX, _transform.transform.position.y);
    }
}
