using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;
using MyScriptableObjectClass;
using System;
using System.Security.Cryptography;

public interface IObstaclePresenter : IDisposable
{
    event Action<float> OnCollisionItem;
    event Action OnCollisionEnemy;
    int ObstacleID { get; }
    MyCircleCollider Collider { get; }
    //View内のコンストラクタで設定できないか考える。
    void CollisionOther(MyCircleCollider other);
    void SetInitializePosition(Vector2 position);
    void UpdateObstacleMove(float deltaTime, float speed);
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
        UnRegisterEvent();
    }
    public ObstaclePresenter(IObstacleModel model , IObstacleView view)
    {
        _model = model;
        _view = view;
        _model.Theta.Subscribe(x => _view.SetTheta(x)).AddTo(_disposable);
        RegisterEvent();
    }
    public event Action<float> OnCollisionItem;
    public event Action OnCollisionEnemy;
    public void SetInitializePosition(Vector2 position)
    {
        _model.SetInitializePosition(position);
    }
    public void UpdateObstacleMove(float deltaTime, float speed)
    {
        _model.Move(deltaTime, speed);
    }

    void RegisterEvent()
    {
        _model.OnCollisionItem += (x) => OnCollisionItem?.Invoke(x);
        _model.OnCollisionEnemy += () => OnCollisionEnemy?.Invoke();
    }
    void UnRegisterEvent()
    {
        _model.OnCollisionItem -= (x) => OnCollisionItem?.Invoke(x);
        _model.OnCollisionEnemy -= () => OnCollisionEnemy?.Invoke();
    }
    public int ObstacleID => _model.ObstacleDataID;
    public MyCircleCollider Collider => _model.Collider;
    public void CollisionOther(MyCircleCollider other)
    {
        _model.CollisionOther(other);
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
