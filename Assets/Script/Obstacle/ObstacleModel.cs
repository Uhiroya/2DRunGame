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
    [SerializeField] float _xMoveSpeed;
    [SerializeField , Range(0f , 1f)] float _xMoveRangeRate;
    [SerializeField] float _yMoveSpeed;
    [SerializeField] float _score;
    public ObstacleType ItemType => _itemType;
    public float Score => _score;
    public readonly ReactiveProperty<float> PositionX = new(0f);
    public readonly ReactiveProperty<float> PositionY = new(0f);
    float _xMoveRange;
    float _defaultPositionX = 0f;
    float _time;
    public void SetX(float posX)
    {
        _xMoveRange = _xMoveRangeRate * Screen.width / 2;
        //障害物の移動距離がマップに収まるように制限する。
        if (posX - _xMoveRange < GamePresenter.MapXMargin)
        {
            _defaultPositionX = GamePresenter.MapXMargin + _xMoveRange;
        }
        else if (posX + _xMoveRange > Screen.width - GamePresenter.MapXMargin)
        {
            _defaultPositionX = Screen.width - GamePresenter.MapXMargin - _xMoveRange;
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
        var moveAmount = deltaTime * speed * Screen.height;
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