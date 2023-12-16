using System;
using UnityEngine;

[Serializable]
public struct MyCircleCollider
{
    private static int _id;
    private readonly float _radius;
    private readonly Transform _transform;
    public readonly int ID;
    public readonly CollisionTag Tag;

    public MyCircleCollider(CollisionTag tag, Transform transform, float radius)
    {
        ID = _id;
        this.Tag = tag;
        _transform = transform;
        _radius = radius;
        _id++;
    }

    public Vector2 Position
    {
        get => _transform.position;
        set => _transform.position = value;
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
