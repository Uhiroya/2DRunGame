using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using MyScriptableObjectClass;
using UnityEngine.UIElements;

public interface IPlayerModel
{
    IReadOnlyReactiveProperty<PlayerCondition> PlayerState { get; }
    Circle GetCollider();
    void Initialize();
    void GameStart();
    void Dying();
    void Dead();
    void Pause();
    void Resume();
    void SetSpeedRate(float speedRate);
    void Move(float x);
    void Reset();
}

public class PlayerModel : IPlayerModel, IDisposable
{
    Transform _playerTransform;
    PlayerModelSetting _playerModelSetting;

    float _speedRate;
    Vector2 _position;
    Circle _collider;
    readonly ReactiveProperty<PlayerCondition> _playerState;
    public IReadOnlyReactiveProperty<PlayerCondition> PlayerState => _playerState;
    public PlayerModel(PlayerModelSetting playerModelSetting, Transform playerTransform)
    {
        _playerTransform = playerTransform;
        _playerModelSetting = playerModelSetting;
        _position = _playerTransform.position;
        _playerState = new(PlayerCondition.Initialize);
        _collider = new Circle(_position , _playerModelSetting.PlayerHitRange);
    }
    CompositeDisposable _disposable = new();
    public void Dispose()
    {
        _disposable.Dispose();
    }

    void SetPlayerCondition(PlayerCondition condition)
    {
        _playerState.Value = condition;
    }
    public void SetSpeedRate(float speedRate)
    {
        _speedRate = speedRate;
    }
    public void Move(float x)
    {
        var nextPosX = _position.x + x * _playerModelSetting.PlayerDefaultSpeed * _speedRate;
        //移動制限
        nextPosX = Mathf.Clamp(nextPosX, InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin);
        _position = new Vector2(nextPosX, _position.y);
        _collider.SetCenter(_position);
        _playerTransform.position = _position;
    }
    public Circle GetCollider()
    {
        return _collider;
    }

    public void Initialize()
    {
        SetPlayerCondition(PlayerCondition.Initialize);
        Reset();
    }

    public void GameStart()
    {
        SetPlayerCondition(PlayerCondition.Alive);
    }

    public void Dying()
    {
        SetPlayerCondition(PlayerCondition.Dying);
    }
    public void Dead()
    {
        SetPlayerCondition(PlayerCondition.Dead);
    }

    public void Pause()
    {
        SetPlayerCondition(PlayerCondition.Pause);
    }

    public void Resume()
    {
        SetPlayerCondition(PlayerCondition.Alive);
    }
    public void Reset()
    {
        _playerTransform.position = new Vector3(InGameConst.WindowWidth / 2, 0f, 0f);
        _position = new Vector2(InGameConst.WindowWidth / 2, _position.y);
    }
}
