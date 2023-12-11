using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Circle
{
    private Vector2 _center;
    private float _radius;
    public Circle(Vector2 center, float radius)
    {
        _center = center;
        _radius = radius;
    }
    public void SetCenter(Vector2 position)
    {
        _center = position;
    }
    public Vector2 GetCenter()
    {
        return _center;
    }
    public bool IsHit(Circle other)
    {
        var x = _center.x - other._center.x;
        var y = _center.y - other._center.y;
        var d = _radius + other._radius;
        return x * x + y * y <= d * d;
    }


}
