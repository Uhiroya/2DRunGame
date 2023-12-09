using System;

public interface ICollisionChecker
{
    event Action<int> OnCollisionEnter;
    void ManualUpdate();
}
public class CollisionChecker : ICollisionChecker
{
    IPlayerPresenter _playerPresenter;
    IObstacleManager _obstacleManager;

    public CollisionChecker(IPlayerPresenter playerPresenter, IObstacleManager obstacleGenerator)
    {
        _playerPresenter = playerPresenter;
        _obstacleManager = obstacleGenerator;
    }
    public void ManualUpdate()
    {
        CheckCollision();
    }
    public event Action<int> OnCollisionEnter;
    /// <summary>
    /// 衝突判定
    /// </summary>
    void CheckCollision()
    {
        var playerCollider = _playerPresenter.GetCollider();
        foreach (var collider in _obstacleManager.GetObstacleColliders())
        {
            if (playerCollider.IsHit(collider))
            {
                OnCollisionEnter?.Invoke(collider.id);
            }
        }
    }

}
