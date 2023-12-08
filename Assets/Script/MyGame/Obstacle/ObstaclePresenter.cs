using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;
using MyScriptableObjectClass;
using System;

public interface IObstaclePresenter : IDisposable
{
    int ModelID { get; }
    int ObstacleID { get; }
    ObstaclePublicInfo ObstacleInfo { get; }
    Circle GetCollider();
    void SetTransform(Transform transform);
    void SetAnimator(Animator animator);
    void SetObstacle(float posX, float posY);
    void UpdateObstacleMove(float deltaTime, float speed);
    void InstantiateDestroyEffect();
    void Pause();
    void Resume();
}
public class ObstaclePresenter : IObstaclePresenter 
{
    IObstacleModel _model;
    IObstacleView _view;

    CompositeDisposable _disposable = new();
    public void Dispose()
    {
        _disposable.Dispose();
    }
    public ObstaclePresenter(IObstacleModel model , IObstacleView view)
    {
        _model = model;
        _view = view;
        _model.Theta.Subscribe(x => _view.SetTheta(x)).AddTo(_disposable);
    }
    public int ModelID => _model.ModelID;
    public int ObstacleID => _model.ObstacleID;
    public ObstaclePublicInfo ObstacleInfo => _model.ObstacleInfo;
    public void SetTransform(Transform transform)
    {
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
    public Circle GetCollider()
    {
        return _model.GetCollider();
    }
    public void Pause()
    {
        _view.Pause();
    }

    public void Resume()
    {
        _view.Resume();
    }
}
