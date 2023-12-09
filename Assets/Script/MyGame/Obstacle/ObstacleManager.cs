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
    void HitObstacle(int hitID);
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

    Dictionary<int, IObstaclePresenter> _activePresenterList = new(30);
    Dictionary<int, IObstaclePresenter> _removeObstacleList = new();
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
    /// アップデートタイミングによって障害物の生成及び移動を行うメソッド
    /// </summary>
    /// <param name="deltaTime"></param>
    /// <param name="speed"></param>
    public void UpdateObstacleMove(float deltaTime, float speed)
    {
        _makeObstacleDistance += deltaTime * speed * InGameConst.WindowHeight;
        //一定距離(distance)毎に障害物を生成する
        if (_makeObstacleDistance > _obstacleGeneratorSetting.ObstacleMakePerDistance)
        {
            var ramdomIndex = UnityEngine.Random.Range(0, _obstacleDataSet.Count());
            _obstacleGenerator.GetObstacle(_obstacleDataSet[ramdomIndex], out IObstaclePresenter presenter);
            SetObstacleInitializePosition(presenter);
            _activePresenterList.Add(presenter.Collider.id ,presenter);
            _makeObstacleDistance = 0;
        }
        //障害物をスクロールさせる。
        foreach (var pair in _activePresenterList)
        {
            var presenter = pair.Value;
            presenter.UpdateObstacleMove(deltaTime, speed);
            if (presenter.Collider.position.y < -_obstacleGeneratorSetting.ObstacleFrameOutRange)
            {
                _removeObstacleList.Add(pair.Key , pair.Value);
            }
        }
        foreach (var presenter in _removeObstacleList.Values)
        {
            ReleaseObstacle(presenter);
        }
        _removeObstacleList.Clear();
    }
    public void Reset()
    {
        foreach (var pair in _activePresenterList)
        {
            _removeObstacleList.Add(pair.Key, pair.Value);
        }
        foreach (var presenter in _removeObstacleList.Values)
        {
            ReleaseObstacle(presenter);
        }
        _activePresenterList.Clear();
        _removeObstacleList.Clear();
        _makeObstacleDistance = 0f;
    }
    public void SetObstacleInitializePosition(IObstaclePresenter presenter)
    {
        presenter.SetInitializePosition(new Vector2(
            UnityEngine.Random.Range(InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin)
            , InGameConst.WindowHeight + _obstacleGeneratorSetting.ObstacleFrameOutRange
            ));
    }
    public void HitObstacle(int hitID)
    {
        var obstacle = _activePresenterList[hitID];
        switch (obstacle.Collider.tag)
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
        //画面外に飛ばして消去する
        obstacle.SetPosition( new Vector2(0f, -_obstacleGeneratorSetting.ObstacleFrameOutRange));
    }
    void ReleaseObstacle(IObstaclePresenter presenter)
    {
        _obstacleGenerator.ReleaseObstacle(presenter);
        _activePresenterList.Remove(presenter.Collider.id);
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
        foreach (var presenter in _activePresenterList.Values)
        {
            presenter.Pause();
        }
    }
    public void Resume()
    {
        foreach (var presenter in _activePresenterList.Values)
        {
            presenter.Resume();
        }
    }
    public List<MyCircleCollider> GetObstacleColliders()
    {
        List<MyCircleCollider> obstacleColliders = new(30);
        foreach (var presenter in _activePresenterList.Values)
        {
            obstacleColliders.Add(presenter.Collider);
        }
        return obstacleColliders;
    }
}

