using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct MyCircleCollider
{
    private static int _id = 0;
    public readonly CollisionTag tag;
    private readonly Transform _transform;
    private readonly float _radius;
    public readonly int id;
    public Vector2 position 
    { 
        get => _transform.position; 
        set => _transform.position = value;
    }
    public MyCircleCollider(CollisionTag tag ,Transform transform ,float radius)
    {
        id = _id;
        this.tag = tag;
        _transform = transform;
        _radius = radius;
        _id++;
    }
    public bool IsHit(in MyCircleCollider other)
    {
        var center = _transform.position;
        var otherCenter = other._transform.position;
        var x = center.x - otherCenter.x;
        var y = center.y - otherCenter.y;
        var d = _radius + other._radius;
        return x * x + y * y <= d * d;
    }


}
