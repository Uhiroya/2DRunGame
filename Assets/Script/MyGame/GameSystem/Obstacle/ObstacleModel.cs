using System;
using MyScriptableObjectClass;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

public interface IObstacleModel
{
    MyCircleCollider Collider { get; }
    int ObstacleDataID { get; }
    IReadOnlyReactiveProperty<float> Theta { get; }
    event Action<float> OnCollisionItem;
    event Action OnCollisionEnemy;
    void CollisionOther(MyCircleCollider other);
    void SetInitializePosition(Vector2 position);
    void Move(float deltaTime, float speed);
}

public class ObstacleModel : IObstacleModel
{
    private readonly ObstacleData _obstacleData;
    private readonly ReactiveProperty<float> _theta;
    private readonly Transform _transform;
    private MyCircleCollider _collider;
    private float _defaultPositionX;
    private float _time;
    private float _xMoveRange;

    public ObstacleModel(Transform transform, ObstacleData obstacleData)
    {
        _obstacleData = obstacleData;
        _transform = transform;
        _collider = new MyCircleCollider(_obstacleData.CollisionType, transform, _obstacleData.HitRange);
        _theta = new ReactiveProperty<float>();
    }

    public event Action<float> OnCollisionItem;
    public event Action OnCollisionEnemy;
    public MyCircleCollider Collider => _collider;
    public int ObstacleDataID => _obstacleData.ObstacleID;
    public IReadOnlyReactiveProperty<float> Theta => _theta;

    public void CollisionOther(MyCircleCollider other)
    {
        if (other.Tag.Equals(CollisionTag.Player))
            switch (_obstacleData.CollisionType)
            {
                case CollisionTag.Item:
                    OnCollisionItem?.Invoke(_obstacleData.Score);
                    break;
                case CollisionTag.Enemy:
                    OnCollisionEnemy?.Invoke();
                    InstantiateDestroyEffect();
                    break;
            }
    }

    public void SetInitializePosition(Vector2 position)
    {
        var posX = position.x;
        var posY = position.y;
        //SetX
        _xMoveRange = _obstacleData.XMoveArea * (InGameConst.WindowWidth - InGameConst.GroundXMargin * 2) / 2;
        //障害物の移動距離がマップに収まるように制限する。
        if (posX - _xMoveRange < InGameConst.GroundXMargin)
            _defaultPositionX = InGameConst.GroundXMargin + _xMoveRange;
        else if (posX + _xMoveRange > InGameConst.WindowWidth - InGameConst.GroundXMargin)
            _defaultPositionX = InGameConst.WindowWidth - InGameConst.GroundXMargin - _xMoveRange;
        else
            _defaultPositionX = posX;
        //SetX , SetY
        _collider.Position = new Vector2(posX, posY);
    }

    public void Move(float deltaTime, float speed)
    {
        //X移動
        _time += deltaTime;
        _theta.Value = _time * speed * _obstacleData.XMoveSpeed % (Mathf.PI * 2);
        var newPos = _defaultPositionX + _xMoveRange * Mathf.Sin(_theta.Value);
        //Y移動
        var moveAmount = deltaTime * speed * InGameConst.WindowHeight * _obstacleData.YMoveSpeed;
        _collider.Position = new Vector2(newPos, _collider.Position.y - moveAmount);
    }

    private void InstantiateDestroyEffect()
    {
        if (_obstacleData.DestroyEffect == null) return;
        var obj = Object.Instantiate(_obstacleData.DestroyEffect, _collider.Position, Quaternion.identity,
            _transform.parent);
        if (_obstacleData.DestroyAnimation == null)
            Object.Destroy(obj, 1f);
        else
            Object.Destroy(obj, _obstacleData.DestroyAnimation.length);
    }
}
