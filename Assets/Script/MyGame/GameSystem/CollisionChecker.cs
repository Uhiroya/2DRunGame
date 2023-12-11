using System;

public interface ICollisionChecker
{
    event Action<IObstaclePresenter> OnCollisionEnter;
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
    public event Action<IObstaclePresenter> OnCollisionEnter;
    /// <summary>
    /// 衝突判定
    /// </summary>
    void CheckCollision()
    {
        foreach (var collider in _obstacleManager.GetObstacleColliders())
        {
            if (_playerPresenter.GetCollider().IsHit(collider.Item1))
            {
                OnCollisionEnter?.Invoke(collider.Item2);
            }
        }
    }

}
