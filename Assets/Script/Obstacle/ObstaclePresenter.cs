using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;
using MyScriptableObjectClass;
public interface IObstaclePresenter
{
    int ModelID { get; }
    int ObstacleID { get; }
    ObstaclePublicInfo ObstacleInfo { get; }
    IReadOnlyReactiveProperty<Vector2> Position { get; } 
    void SetTransform(Transform transform);
    void SetObstacle(float posX, float posY);
    void UpdateObstacleMove(float deltaTime, float speed);
}
public class ObstaclePresenter : IObstaclePresenter
{
    IObstacleModel _model;
    IObstacleView _view;
    public ObstaclePublicInfo ObstacleInfo => _model.ObstacleInfo;
    public ObstaclePresenter(IObstacleModel model , IObstacleView view)
    {
        _model = model;
        _view = view;
    }

    public int ModelID => _model.ModelID;
    public int ObstacleID => _model.ObstacleID;
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
