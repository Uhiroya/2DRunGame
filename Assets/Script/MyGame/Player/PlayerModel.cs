using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using MyScriptableObjectClass;
public interface IPlayerModel
{
    IReadOnlyReactiveProperty<PlayerCondition> PlayerState { get; }
    float PlayerHitRange { get; }
    float PositionY { get;}
    IReadOnlyReactiveProperty<float> PositionX { get; }
    void SetPlayerCondition(PlayerCondition condition);
    void SetSpeedRate(float speedRate);
    void Move(float x);
    void Reset();
}

public class PlayerModel : IPlayerModel , IDisposable
{
    
    Transform _playerTransform;
    PlayerModelSetting _playerModelSetting;

    float _speedRate;
    float _positionY;
    public float PlayerHitRange => _playerModelSetting.PlayerHitRange;
    public float PositionY => _positionY ;
    readonly ReactiveProperty<float> _positionX;
    public IReadOnlyReactiveProperty<float> PositionX => _positionX;
    readonly ReactiveProperty<PlayerCondition> _playerState;
    public IReadOnlyReactiveProperty<PlayerCondition> PlayerState => _playerState;
    public PlayerModel(PlayerModelSetting playerModelSetting , Transform playerTransform)
    {
        _playerTransform = playerTransform;
        _playerModelSetting = playerModelSetting;
        _positionY = _playerTransform.position.y;
        _positionX = new(0f);
        _positionX
            .Skip(1)
            .Subscribe(x => { ClumpX(); })
            .AddTo(_disposable);
        _playerState = new(PlayerCondition.Initialize);   
    }
    CompositeDisposable _disposable = new();
    public void Dispose()
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
        _playerTransform.position += new Vector3(x * _playerModelSetting.PlayerDefaultSpeed * _speedRate, 0f);
        _positionX.Value = _playerTransform.position.x;
    }
    public void Reset()
    {
        _playerTransform.position = new Vector3(InGameConst.WindowWidth / 2, 0f, 0f);
        _positionX.Value = InGameConst.WindowWidth / 2;
        _playerState.Value = 0f;
    }
    /// <summary>
    /// 移動制限
    /// </summary>
    void ClumpX()
    {
        var position = _playerTransform.position;
        var clampX = Mathf.Clamp(position.x, InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin);
        _playerTransform.position = new Vector2(clampX,position.y);
    }
}
