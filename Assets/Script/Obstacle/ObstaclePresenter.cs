using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public interface IObstaclePresenter
{

}
public class ObstaclePresenter 
{
    ObstacleModel _model;
    ObstacleData _obstacleData;

    public ObstaclePresenter(ObstacleData obstacleData , ObstacleModel model)
    {
        _obstacleData = obstacleData;
        _model = model;
    }
    public ObstacleData ObstacleData => _obstacleData;
    public readonly ReactiveProperty<Vector2> Position = new();
    public void SetTransform(Transform transform)
    {
        _model.Position.Subscribe(x => Position.Value = x );
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
