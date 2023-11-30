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
    ObstaclePublicInfo ObstacleInfo { get; }
    int ModelID { get; }
    int DataIndex { get; }
    void SetTransform(Transform transform);
    void Set(float posX, float posY);
    void Move(float deltaTime, float speed);
}
public class ObstacleModel : IObstacleModel
{
    static int _nextModelID = 0;
    int _modelID = 0;
    int _dataIndex;
    ObstacleData _obstacleData;
    public ObstaclePublicInfo ObstacleInfo => _obstacleData.ObstacleInfo;
    Transform _transform;
    public int ModelID => _modelID;
    public int DataIndex => _dataIndex;
    public ObstacleModel(ObstacleData obstacleData , int dataIndex)
    {
        _obstacleData = obstacleData;
        _dataIndex = dataIndex;
        _modelID = _nextModelID;
        _nextModelID ++;
    }
    public void SetTransform(Transform transform)
    {
        _transform = transform;
    }
    public readonly ReactiveProperty<Vector2> _position = new(Vector2.zero);
    public IReadOnlyReactiveProperty<Vector2> Position => _position;
    float _xMoveRange;
    float _defaultPositionX = 0f;
    float _time;
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
        _position.Value = position;
    }
    public void Move(float deltaTime, float speed)
    {
        //X移動
        _time += deltaTime;
        var theta = (_time * speed * _obstacleData.XMoveSpeed) % (Mathf.PI * 2);
        var newPos = _defaultPositionX + _xMoveRange * Mathf.Sin(theta);
        //Y移動
        var moveAmount = deltaTime * speed * InGameConst.WindowHeight * _obstacleData.YMoveSpeed;
        var position = new Vector3(newPos, _transform.position.y - moveAmount);
        _transform.position = position;
        _position.Value = position;
    }
}

