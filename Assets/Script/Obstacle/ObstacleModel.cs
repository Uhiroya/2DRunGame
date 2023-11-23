using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using MyScriptableObjectClass;
public interface IObstacleModel
{
    IReadOnlyReactiveProperty<Vector2> Position { get; }
    public void SetTransform(Transform transform);
    public void Set(float posX, float posY);
    public void Move(float deltaTime, float speed);
}
public class ObstacleModel : IObstacleModel
{
    Transform _transform;
    ObstacleParam _obstacleParam;
    public ObstacleModel(ObstacleParam obstacleParam)
    {
        _obstacleParam = obstacleParam;
    }
    public readonly ReactiveProperty<Vector2> _position = new(Vector2.zero);
    public IReadOnlyReactiveProperty<Vector2> Position => _position;
    float _xMoveRange;
    float _defaultPositionX = 0f;
    float _time;
    public void SetTransform(Transform transform)
    {
        _transform = transform;
    }
    public void Set(float posX, float posY)
    {
        //SetX
        _xMoveRange = _obstacleParam.XMoveArea * InGameConst.WindowWidth / 2;
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
        _position.Value = position;
    }
    public void Move(float deltaTime, float speed)
    {
        //X移動
        _time += deltaTime;
        var theta = (_time * speed * _obstacleParam.XMoveSpeed) % (Mathf.PI * 2);
        var newPos = _defaultPositionX + _xMoveRange * Mathf.Sin(theta);
        //Y移動
        var moveAmount = deltaTime * speed * InGameConst.WindowHeight * _obstacleParam.YMoveSpeed;
        var position = new Vector3(newPos, _transform.position.y - moveAmount);
        _transform.position = position;
        _position.Value = position;
    }
}

