using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using MyScriptableObjectClass;

public interface IObstacleModel
{
    MyCircleCollider Collider { get; }
    int ObstacleDataID { get; }
    float Score { get; }
    IReadOnlyReactiveProperty<float> Theta { get; }
    void SetInitializePosition(Vector2 position);
    void Move(float deltaTime, float speed);
    void InstantiateDestroyEffect();
}
public class ObstacleModel : IObstacleModel
{
    ObstacleData _obstacleData;
    MyCircleCollider _collider;
    public MyCircleCollider Collider => _collider;
    public int ObstacleDataID => _obstacleData.ObstacleID;
    public float Score => _obstacleData.Score;
    readonly ReactiveProperty<float> _theta;
    public IReadOnlyReactiveProperty<float> Theta => _theta;
    private readonly Transform _transform;
    public ObstacleModel(Transform transform , ObstacleData obstacleData)
    {
        _obstacleData = obstacleData;
        _transform = transform;
        _collider = new(_obstacleData.CollisionType, transform, _obstacleData.HitRange);
        _theta = new();
    } 
    float _xMoveRange;
    float _defaultPositionX = 0f;
    float _time;
    public void SetInitializePosition(Vector2 position)
    {
        var posX = position.x; 
        var posY = position.y;
        //SetX
        _xMoveRange = (_obstacleData.XMoveArea) * (InGameConst.WindowWidth - InGameConst.GroundXMargin * 2) / 2;
        //障害物の移動距離がマップに収まるように制限する。
        if (posX - _xMoveRange < InGameConst.GroundXMargin)
        {
            _defaultPositionX = InGameConst.GroundXMargin + _xMoveRange;
        }
        else if (posX + _xMoveRange > InGameConst.WindowWidth - InGameConst.GroundXMargin)
        {
            _defaultPositionX = InGameConst.WindowWidth - InGameConst.GroundXMargin - _xMoveRange;
        }
        else
        {
            _defaultPositionX = posX;
        }
        //SetX , SetY
        _collider.position = new(posX, posY);
    }
    public void Move(float deltaTime, float speed)
    {
        //X移動
        _time += deltaTime;
        _theta.Value = (_time * speed * _obstacleData.XMoveSpeed) % (Mathf.PI * 2);
        var newPos = _defaultPositionX + _xMoveRange * Mathf.Sin(_theta.Value);
        //Y移動
        var moveAmount = deltaTime * speed * InGameConst.WindowHeight * _obstacleData.YMoveSpeed;
        _collider.position = new Vector2(newPos, _collider.position.y - moveAmount);
    }
    public void InstantiateDestroyEffect()
    {
        if (_obstacleData.DestroyEffect == null) return;
        var obj = Object.Instantiate(_obstacleData.DestroyEffect, _collider.position, Quaternion.identity, _transform.parent);
        if (_obstacleData.DestroyAnimation == null)
        {
            Object.Destroy(obj, 1f);
        }
        else
        {
            Object.Destroy(obj, _obstacleData.DestroyAnimation.length);
        }
    }
}

