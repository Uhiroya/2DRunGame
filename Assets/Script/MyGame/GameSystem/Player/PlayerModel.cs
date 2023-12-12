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
    MyCircleCollider Collider { get; }
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
    PlayerModelSetting _playerModelSetting;

    float _speedRate;
    MyCircleCollider _collider;
    public MyCircleCollider Collider => _collider;
    readonly ReactiveProperty<PlayerCondition> _playerState;
    public IReadOnlyReactiveProperty<PlayerCondition> PlayerState => _playerState;
    public PlayerModel(PlayerModelSetting playerModelSetting, Transform playerTransform)
    {
        _playerModelSetting = playerModelSetting;
        _playerState = new(PlayerCondition.Initialize);
        _collider = new MyCircleCollider(CollisionTag.Player ,playerTransform, _playerModelSetting.PlayerHitRange);
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
        var position = _collider.position;
        var nextPosX = position.x + x * _playerModelSetting.PlayerDefaultSpeed * _speedRate;
        //移動制限
        nextPosX = Mathf.Clamp(nextPosX, InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin);
        position = new Vector2(nextPosX, position.y);
        _collider.position = position;
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
        var resetPos = new Vector2(InGameConst.WindowWidth / 2, _collider.position.y);
        _collider.position = resetPos;
    }
}
