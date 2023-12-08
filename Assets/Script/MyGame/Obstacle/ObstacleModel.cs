using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using MyScriptableObjectClass;
public interface IObstacleModel
{
    Circle GetCollider();
    ObstaclePublicInfo ObstacleInfo { get; }
    int ModelID { get; }
    int ObstacleID { get; }
    IReadOnlyReactiveProperty<float> Theta { get; }
    void SetTransform(Transform transform);
    void Set(float posX, float posY);
    void Move(float deltaTime, float speed);
    void InstantiateDestroyEffect();
}
public class ObstacleModel : IObstacleModel
{
    static int _nextModelID = 0;
    int _modelID = 0;
    ObstacleData _obstacleData;
    public ObstaclePublicInfo ObstacleInfo => _obstacleData.ObstacleInfo;
    Transform _transform;
    public int ModelID => _modelID;
    public int ObstacleID => _obstacleData.ObstacleID;

    readonly ReactiveProperty<float> _theta;
    public IReadOnlyReactiveProperty<float> Theta => _theta;
    public ObstacleModel(ObstacleData obstacleData)
    {
        _obstacleData = obstacleData;
        _modelID = _nextModelID;
        _nextModelID++;
        _theta = new();
    }
    public void SetTransform(Transform transform)
    {
        _transform = transform;
    }
    Circle _collider;
    float _xMoveRange;
    float _defaultPositionX = 0f;
    float _time;
    public Circle GetCollider()
    {
        return _collider;
    }
    public void Set(float posX, float posY)
    {
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
        var position = new Vector3(posX, posY);
        _transform.position = position;
        _collider = new(position , ObstacleInfo.HitRange);
    }
    public void Move(float deltaTime, float speed)
    {
        //X移動
        _time += deltaTime;
        _theta.Value = (_time * speed * _obstacleData.XMoveSpeed) % (Mathf.PI * 2);
        var newPos = _defaultPositionX + _xMoveRange * Mathf.Sin(_theta.Value);
        //Y移動
        var moveAmount = deltaTime * speed * InGameConst.WindowHeight * _obstacleData.YMoveSpeed;
        var position = new Vector3(newPos, _transform.position.y - moveAmount);
        _transform.position = position;
        _collider.SetCenter( position);
    }
    public void InstantiateDestroyEffect()
    {
        if (_obstacleData.DestroyEffect == null) return;
        var obj = Object.Instantiate(_obstacleData.DestroyEffect, _transform.position, Quaternion.identity, _transform.parent);
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

