using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using System.Linq;
using VContainer;
using VContainer.Unity;
using MyScriptableObjectClass;
using System;

public interface IObstacleManager
{
    event Action<float> OnCollisionItemEvent;
    event Action OnCollisionEnemyEvent;
    List<MyCircleCollider> GetObstacleColliders();
    void OnGameFlowStateChanged(GameFlowState gameFlowState);
    void UpdateObstacleMove(float deltaTime, float speed);
    void CollisionObstacle(MyCircleCollider collider, CollisionTag collisionTag);
    void Reset();
}
public class ObstacleManager : IObstacleManager, IDisposable, IPauseable
{
    IObstacleGenerator _obstacleGenerator;
    ObstacleGeneratorSetting _obstacleGeneratorSetting;
    public ObstacleManager(ObstacleGeneratorSetting obstacleGeneratorSetting, IObstacleGenerator obstacleGenerator)
    {
        _obstacleGenerator = obstacleGenerator;
        _obstacleGeneratorSetting = obstacleGeneratorSetting;
    }
    List<ObstacleData> _obstacleDataSet => _obstacleGeneratorSetting.ObstacleDataSet;

    /// <summary>
    /// Collider.id と presenter の 辞書型
    /// </summary>
    Dictionary<int, IObstaclePresenter> _activePresenterSet = new(30);
    Dictionary<int, IObstaclePresenter> _removeObstacleSet = new();
    /// <summary>
    /// 障害物を生成する距離
    /// </summary>
    float _makeObstacleDistance;
    
    public event Action<float> OnCollisionItemEvent;
    public event Action OnCollisionEnemyEvent;
    public void OnGameFlowStateChanged(GameFlowState gameFlowState)
    {
        switch (gameFlowState)
        {
            case GameFlowState.Result:
                Reset();
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 障害物の生成及び移動を行うメソッド
    /// </summary>
    public void UpdateObstacleMove(float deltaTime, float speed)
    {
        _makeObstacleDistance += deltaTime * speed * InGameConst.WindowHeight;
        //一定距離(distance)毎に障害物を生成する
        if (_makeObstacleDistance > _obstacleGeneratorSetting.ObstacleMakePerDistance)
        {
            var ramdomIndex = UnityEngine.Random.Range(0, _obstacleDataSet.Count());
            _obstacleGenerator.GetObstacle(_obstacleDataSet[ramdomIndex], out IObstaclePresenter presenter);
            presenter.SetInitializePosition(
                new Vector2(
                    UnityEngine.Random.Range(InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin)
                    , InGameConst.WindowHeight + _obstacleGeneratorSetting.ObstacleSetYPosition
                    )
                );
            _activePresenterSet.Add(presenter.Collider.id ,presenter);
            _makeObstacleDistance = 0;
        }
        //障害物をスクロールさせる。
        foreach (var pair in _activePresenterSet)
        {
            var presenter = pair.Value;
            presenter.UpdateObstacleMove(deltaTime, speed);
        }
    }

    /// <summary>
    /// ゲーム終了時の初期化
    /// </summary>
    public void Reset()
    {
        foreach (var pair in _activePresenterSet)
        {
            _removeObstacleSet.Add(pair.Key, pair.Value);
        }
        foreach (var presenter in _removeObstacleSet.Values)
        {
            ReleaseObstacle(presenter);
        }
        _activePresenterSet.Clear();
        _removeObstacleSet.Clear();
        _makeObstacleDistance = 0f;
    }
    /// <summary>
    /// 衝突時及び場外の判定
    /// </summary>
    public void CollisionObstacle(MyCircleCollider collider , CollisionTag collisionTag)
    {
        var obstacle = _activePresenterSet[collider.id];
        switch (collisionTag)
        {
            case CollisionTag.OutField:
                break;
            case CollisionTag.Player:
                switch (collider.tag)
                {
                    case CollisionTag.Item:
                        OnCollisionItemEvent?.Invoke(obstacle.Score);
                        break;
                    case CollisionTag.Enemy:
                        OnCollisionEnemyEvent.Invoke();
                        obstacle.InstantiateDestroyEffect();
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
        ReleaseObstacle(obstacle);
    }
    /// <summary>
    /// 障害物をプールに戻す
    /// </summary>
    void ReleaseObstacle(IObstaclePresenter presenter)
    {
        _obstacleGenerator.ReleaseObstacle(presenter);
        _activePresenterSet.Remove(presenter.Collider.id);
    }
    /// <summary>
    /// VContainerによって破棄される
    /// </summary>
    CompositeDisposable _disposable = new();
    public void Dispose()
    {
        _disposable.Dispose();
    }
    /// <summary>
    /// ObstaclePresenterとObstacleViewにはIPausableを実装せずにここから呼び出している。
    /// </summary>
    public void Pause()
    {
        foreach (var presenter in _activePresenterSet.Values)
        {
            presenter.Pause();
        }
    }
    public void Resume()
    {
        foreach (var presenter in _activePresenterSet.Values)
        {
            presenter.Resume();
        }
    }
    public List<MyCircleCollider> GetObstacleColliders()
    {
        List<MyCircleCollider> obstacleColliders = new(30);
        foreach (var presenter in _activePresenterSet.Values)
        {
            obstacleColliders.Add(presenter.Collider);
        }
        return obstacleColliders;
    }
}

