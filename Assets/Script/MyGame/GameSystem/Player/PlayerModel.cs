using System;
using MyScriptableObjectClass;
using UniRx;
using UnityEngine;

public interface IPlayerModel
{
    IReadOnlyReactiveProperty<PlayerCondition> PlayerState { get; }
    MyCircleCollider Collider { get; }
    void CollisionObstacle(MyCircleCollider obstacle, MyCircleCollider other);
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
    private readonly CompositeDisposable _disposable = new();
    private readonly PlayerModelSetting _playerModelSetting;
    private readonly ReactiveProperty<PlayerCondition> _playerState;
    private MyCircleCollider _collider;

    private float _speedRate;

    public PlayerModel(PlayerModelSetting playerModelSetting, Transform playerTransform)
    {
        _playerModelSetting = playerModelSetting;
        _playerState = new ReactiveProperty<PlayerCondition>(PlayerCondition.Initialize);
        _collider = new MyCircleCollider(CollisionTag.Player, playerTransform, _playerModelSetting.PlayerHitRange);
    }

    public void Dispose()
    {
        _disposable.Dispose();
    }

    public MyCircleCollider Collider => _collider;
    public IReadOnlyReactiveProperty<PlayerCondition> PlayerState => _playerState;

    public void SetSpeedRate(float speedRate)
    {
        _speedRate = speedRate;
    }

    public void Move(float x)
    {
        var position = _collider.Position;
        var nextPosX = position.x + x * _playerModelSetting.PlayerDefaultSpeed * _speedRate;
        //移動制限
        nextPosX = Mathf.Clamp(nextPosX, InGameConst.GroundXMargin,
            InGameConst.WindowWidth - InGameConst.GroundXMargin);
        position = new Vector2(nextPosX, position.y);
        _collider.Position = position;
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

    /// <summary>
    ///     衝突時及び場外の判定
    /// </summary>
    public void CollisionObstacle(MyCircleCollider obstacle, MyCircleCollider other)
    {
        if (other.Tag.Equals(CollisionTag.Player))
            switch (obstacle.Tag)
            {
                case CollisionTag.Item:
                    SetPlayerCondition(PlayerCondition.GetItem);
                    SetPlayerCondition(PlayerCondition.Alive);
                    break;
                case CollisionTag.Enemy:
                    Dying();
                    break;
            }
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
        var resetPos = new Vector2(InGameConst.WindowWidth / 2, _collider.Position.y);
        _collider.Position = resetPos;
    }

    private void SetPlayerCondition(PlayerCondition condition)
    {
        _playerState.Value = condition;
    }
}
