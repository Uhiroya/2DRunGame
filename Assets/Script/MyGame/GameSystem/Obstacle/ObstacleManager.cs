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
    event Action<float> OnCollisionItem;
    event Action OnCollisionEnemy;
    List<MyCircleCollider> GetObstacleColliders();
    void OnGameFlowStateChanged(GameFlowState gameFlowState);
    void UpdateObstacleMove(float deltaTime, float speed);
    void CollisionObstacle(MyCircleCollider obstacle, MyCircleCollider other);
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
        RegisterEvent();
    }
    /// <summary>
    /// VContainerによって破棄される
    /// </summary>
    CompositeDisposable _disposable = new();
    public void Dispose()
    {
        _disposable.Dispose();
        UnRegisterEvent();
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
    /// <summary>
    /// 衝突イベント(Modelに登録する)
    /// </summary>
    public event Action<float> OnCollisionItem;
    public event Action OnCollisionEnemy;

    void RegisterEvent()
    {
        _obstacleGenerator.OnCollisionItem += (x) => OnCollisionItem?.Invoke(x);
        _obstacleGenerator.OnCollisionEnemy += () => OnCollisionEnemy?.Invoke();
    }
    void UnRegisterEvent()
    {
        _obstacleGenerator.OnCollisionItem -= (x) => OnCollisionItem?.Invoke(x);
        _obstacleGenerator.OnCollisionEnemy -= () => OnCollisionEnemy?.Invoke();
    }
    public void CollisionObstacle(MyCircleCollider obstacle, MyCircleCollider other)
    {
        var presenter = _activePresenterSet[obstacle.id];
        presenter.CollisionOther(other);
        ReleaseObstacle(presenter);
    }
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
            GetRandomObstacle();
            _makeObstacleDistance = 0;
        }
        //障害物をスクロールさせる。
        foreach (var presenter in _activePresenterSet.Values)
        {
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
    /// プールからObstacleを取り出す
    /// </summary>
    void GetRandomObstacle()
    {
        var ramdomIndex = UnityEngine.Random.Range(0, _obstacleDataSet.Count());
        _obstacleGenerator.GetObstacle(_obstacleDataSet[ramdomIndex], out IObstaclePresenter presenter);
        presenter.SetInitializePosition(
            new Vector2(
                UnityEngine.Random.Range(InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin)
                , InGameConst.WindowHeight + _obstacleGeneratorSetting.ObstacleSetYPosition
                )
            );
        _activePresenterSet.Add(presenter.Collider.id, presenter);
    }
    /// <summary>
    /// Obstacleをプールに戻す
    /// </summary>
    void ReleaseObstacle(IObstaclePresenter presenter)
    {
        _obstacleGenerator.ReleasePresenter(presenter);
        _activePresenterSet.Remove(presenter.Collider.id);
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

