using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MyScriptableObjectClass;
public interface IObstaclePresenter
{
    ObstacleData ObstacleData { get; }
    IReadOnlyReactiveProperty<Vector2> Position { get; } 
    void SetTransform(Transform transform);
    void SetObstacle(float posX, float posY);
    void UpdateObstacleMove(float deltaTime, float speed);
}
public class ObstaclePresenter : IObstaclePresenter
{
    IObstacleModel _model;
    ObstacleData _obstacleData;
    public ObstaclePresenter(ObstacleData obstacleData , IObstacleModel model)
    {
        _obstacleData = obstacleData;
        _model = model;
    }
    public ObstacleData ObstacleData => _obstacleData;
    public readonly ReactiveProperty<Vector2> _position = new();
    public IReadOnlyReactiveProperty<Vector2> Position => _position;
    public void SetTransform(Transform transform)
    {
        _model.Position.Subscribe(x => _position.Value = x );
        _model.SetTransform(transform);
    }
    public void SetObstacle(float posX, float posY)
    {
        _model.Set(posX, posY);
    }
    public void UpdateObstacleMove(float deltaTime, float speed)
    {
        _model.Move(deltaTime, speed);
    }

}
