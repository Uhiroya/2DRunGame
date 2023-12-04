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
    void SetAnimator(Animator animator);
    void SetObstacle(float posX, float posY);
    void UpdateObstacleMove(float deltaTime, float speed);
    void InstantiateDestroyEffect();
}
public class ObstaclePresenter : IObstaclePresenter
{
    IObstacleModel _model;
    IObstacleView _view;
    public ObstaclePresenter(IObstacleModel model , IObstacleView view)
    {
        _model = model;
        _view = view;
    }
    public int ModelID => _model.ModelID;
    public int ObstacleID => _model.ObstacleID;
    public ObstaclePublicInfo ObstacleInfo => _model.ObstacleInfo;
    readonly ReactiveProperty<Vector2> _position = new();
    public IReadOnlyReactiveProperty<Vector2> Position => _position;
    public void SetTransform(Transform transform)
    {
        _model.Position.Subscribe(pos =>
            {
                Debug.Log(pos.x - _position.Value.x);
                _view.SetXMovement(pos.x - _position.Value.x);
                _position.Value = pos;
            });
        _model.SetTransform(transform);
    }
    public void SetAnimator(Animator animator)
    {
        _view.SetAnimator(animator);
    }
    public void SetObstacle(float posX, float posY)
    {
        _model.Set(posX, posY);
    }
    public void UpdateObstacleMove(float deltaTime, float speed)
    {
        _model.Move(deltaTime, speed);
    }
    public void InstantiateDestroyEffect()
    {
       _model.InstantiateDestroyEffect();
    }
}
