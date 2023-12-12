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
    int ObstacleID { get; }
    float Score { get; }
    MyCircleCollider Collider { get; }
    //View内のコンストラクタで設定できないか考える。
    void SetAnimator(Animator animator);
    void SetInitializePosition(Vector2 position);
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
    public int ObstacleID => _model.ObstacleDataID;
    public float Score => _model.Score;
    public MyCircleCollider Collider => _model.Collider;
    public void SetAnimator(Animator animator)
    {
        _view.SetAnimator(animator);
    }
    public void SetInitializePosition(Vector2 position)
    {
        _model.SetInitializePosition(position);
    }
    public void UpdateObstacleMove(float deltaTime, float speed)
    {
        _model.Move(deltaTime, speed);
    }
    public void InstantiateDestroyEffect()
    {
       _model.InstantiateDestroyEffect();
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
