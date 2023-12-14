using System;
using MyScriptableObjectClass;

public interface ICollisionChecker
{
    event Action<MyCircleCollider, MyCircleCollider> OnCollisionEnter;
    void ManualUpdate();
}

public class CollisionChecker : ICollisionChecker
{
    private static MyCircleCollider? _outField;

    private readonly CollisionCheckerSetting _collisionCheckerSetting;
    private readonly IObstacleManager _obstacleManager;
    private readonly IPlayerPresenter _playerPresenter;

    public CollisionChecker(CollisionCheckerSetting collisionCheckerSetting, IPlayerPresenter playerPresenter,
        IObstacleManager obstacleGenerator)
    {
        _collisionCheckerSetting = collisionCheckerSetting;
        _playerPresenter = playerPresenter;
        _obstacleManager = obstacleGenerator;
    }

    private MyCircleCollider OutField => _outField ??= new MyCircleCollider(CollisionTag.OutField, null, 0);

    public void ManualUpdate()
    {
        CheckCollision();
    }

    public event Action<MyCircleCollider, MyCircleCollider> OnCollisionEnter;

    /// <summary>
    ///     衝突判定
    /// </summary>
    private void CheckCollision()
    {
        var playerCollider = _playerPresenter.Collider;
        foreach (var collider in _obstacleManager.GetObstacleColliders())
        {
            if (playerCollider.IsHit(collider)) OnCollisionEnter?.Invoke(collider, playerCollider);
            if (collider.Position.y < -_collisionCheckerSetting.YFrameOut)
                OnCollisionEnter?.Invoke(collider, OutField);
        }
    }
}
