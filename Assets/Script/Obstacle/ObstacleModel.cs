using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
[System.Serializable]
public class ObstacleModel
{
    [SerializeField] Transform _transform;
    [SerializeField] ObstacleType _itemType;
    [SerializeField, Range(0f, 1f)] float _xMoveRangeRate;
    [SerializeField] float _xMoveSpeed;
    [SerializeField] float _yMoveSpeed;
    [SerializeField] float _score;
    public ObstacleType ItemType => _itemType;
    public float Score => _score;
    public readonly ReactiveProperty<float> PositionX = new(0f);
    public readonly ReactiveProperty<float> PositionY = new(0f);
    float _xMoveRange;
    float _defaultPositionX = 0f;
    float _time;
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
        //��Q���̈ړ��������}�b�v�Ɏ��܂�悤�ɐ�������B
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
    Enemy,
    Item,
}