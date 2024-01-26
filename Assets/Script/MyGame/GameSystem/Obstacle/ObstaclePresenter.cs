using System;
using UniRx;
using UnityEngine;

public interface IObstaclePresenter : IDisposable
{
    int ObstacleID { get; }
    MyCircleCollider Collider { get; }
    event Action<float> OnCollisionItem;

    event Action OnCollisionEnemy;

    //View内のコンストラクタで設定できないか考える。
    void CollisionOther(MyCircleCollider other);
    void SetInitializePosition(Vector2 position);
    void UpdateObstacleMove(float deltaTime, float speed);
    void Pause();
    void Resume();
}

public class ObstaclePresenter : IObstaclePresenter
{
    private readonly CompositeDisposable _disposable = new();
    private readonly IObstacleModel _model;
    private readonly IObstacleView _view;

    public ObstaclePresenter(IObstacleModel model, IObstacleView view)
    {
        _model = model;
        _view = view;
        _model.Theta.Subscribe(x => _view.SetTheta(x)).AddTo(_disposable);
        RegisterEvent();
    }

    public void Dispose()
    {
        _disposable.Dispose();
        UnRegisterEvent();
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

    private void RegisterEvent()
    {
        _model.OnCollisionItem += x => OnCollisionItem?.Invoke(x);
        _model.OnCollisionEnemy += () => OnCollisionEnemy?.Invoke();
    }
    
    private void UnRegisterEvent()
    {
        _model.OnCollisionItem -= InvokeOnCollisionItem;
        _model.OnCollisionEnemy -= InvokeOnCollisionEnemy;
    }

    private void InvokeOnCollisionItem(float score)
    {
        OnCollisionItem?.Invoke(score);
    }

    private void InvokeOnCollisionEnemy()
    {
        OnCollisionEnemy?.Invoke();
    }
    
}
