using MyScriptableObjectClass;
using System;
using UnityEngine;

public interface ICollisionChecker
{
    event Action<MyCircleCollider, MyCircleCollider> OnCollisionEnter;
    void ManualUpdate();
}
public class CollisionChecker : ICollisionChecker
{
    private static MyCircleCollider? _outField;
    public MyCircleCollider OutField => _outField ??= new MyCircleCollider(CollisionTag.OutField, null, 0);

    CollisionCheckerSetting _collisionCheckerSetting;
    IPlayerPresenter _playerPresenter;
    IObstacleManager _obstacleManager;

    public CollisionChecker(CollisionCheckerSetting collisionCheckerSetting, IPlayerPresenter playerPresenter, IObstacleManager obstacleGenerator)
    {
        _collisionCheckerSetting = collisionCheckerSetting;
        _playerPresenter = playerPresenter;
        _obstacleManager = obstacleGenerator;
    }
    public void ManualUpdate()
    {
        CheckCollision();
    }
    public event Action<MyCircleCollider, MyCircleCollider> OnCollisionEnter;
    /// <summary>
    /// 衝突判定
    /// </summary>
    void CheckCollision()
    {
        var playerCollider = _playerPresenter.Collider;
        foreach (var collider in _obstacleManager.GetObstacleColliders())
        {
            if (playerCollider.IsHit(collider))
            {
                OnCollisionEnter?.Invoke(collider , playerCollider);
            }
            if (collider.position.y < -_collisionCheckerSetting._YFrameOut)
            {
                OnCollisionEnter?.Invoke(collider, OutField);
            }
        }
    }

}
