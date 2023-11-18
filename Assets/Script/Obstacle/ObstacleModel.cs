using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
public class ObstacleModel
{
    Transform _transform;
    ObstacleType _itemType;
    float _xMoveRangeRate;
    float _xMoveSpeed;
    float _yMoveSpeed;
    float _score;

    public ObstacleModel(ObstacleType itemType , float xMoveRangeRate
        , float xMoveSpeed , float yMoveSpeed , float score)
    {
        _itemType = itemType;
        _xMoveRangeRate = xMoveRangeRate;
        _xMoveSpeed = xMoveSpeed;
        _yMoveSpeed = yMoveSpeed;
        _score = score;
    }
    public ObstacleType ItemType => _itemType;
    public float Score => _score;
    public readonly ReactiveProperty<float> PositionX = new(0f);
    public readonly ReactiveProperty<float> PositionY = new(0f);
    float _xMoveRange;
    float _defaultPositionX = 0f;
    float _time;
    public void Create(Transform transform)
    {
        _transform = transform;
    }
    public void Set(float posX , float posY)
    {
        SetX(posX);
        SetY(posY);
    }
    public void Move(float deltaTime , float speed)
    {
        MoveX(deltaTime, speed);
        MoveY(deltaTime, speed);
    }
    public void SetX(float posX)
    {
        _xMoveRange = _xMoveRangeRate * InGameConst.WindowWidth / 2;
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
        _transform.position = new Vector3(posX, _transform.position.y, _transform.position.z);
        PositionX.Value = posX;  
    }
    public void SetY(float posY)
    {
        _transform.position = new Vector3(_transform.position.x, posY, _transform.position.z);
        PositionY.Value = _transform.position.y;
    }
    public void MoveX(float deltaTime, float speed)
    {
        _time += deltaTime;
        var theta = (_time * speed * _xMoveSpeed) % (Mathf.PI * 2);
        var newPos = _defaultPositionX + _xMoveRange * Mathf.Sin(theta);
        _transform.position = new Vector3(newPos, _transform.position.y, _transform.position.z);
        PositionX.Value = newPos;
    }
    public void MoveY(float deltaTime, float speed)
    {
        var moveAmount = deltaTime * speed * InGameConst.WindowHeight;
        _transform.position -= new Vector3(0f , (moveAmount * _yMoveSpeed), 0f);
        PositionY.Value = _transform.position.y;
    }
}
[System.Serializable]
public enum ObstacleType
{
    None,
    Enemy,
    Item,
}