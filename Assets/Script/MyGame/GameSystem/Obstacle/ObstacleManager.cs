using System;
using System.Collections.Generic;
using System.Linq;
using MyScriptableObjectClass;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

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

public class ObstacleManager : IObstacleManager, IDisposable, IPausable
{
    /// <summary>
    ///     Collider.id と presenter の 辞書型
    /// </summary>
    private readonly Dictionary<int, IObstaclePresenter> _activePresenterSet = new(30);

    /// <summary>
    ///     VContainerによって破棄される
    /// </summary>
    private readonly CompositeDisposable _disposable = new();

    private readonly IObstacleGenerator _obstacleGenerator;
    private readonly ObstacleGeneratorSetting _obstacleGeneratorSetting;
    private readonly Dictionary<int, IObstaclePresenter> _removeObstacleSet = new();

    /// <summary>
    ///     障害物を生成する距離
    /// </summary>
    private float _makeObstacleDistance;

    public ObstacleManager(ObstacleGeneratorSetting obstacleGeneratorSetting, IObstacleGenerator obstacleGenerator)
    {
        _obstacleGenerator = obstacleGenerator;
        _obstacleGeneratorSetting = obstacleGeneratorSetting;
        RegisterEvent();
    }

    private List<ObstacleData> ObstacleDataSet => _obstacleGeneratorSetting.ObstacleDataSet;

    public void Dispose()
    {
        _disposable.Dispose();
        UnRegisterEvent();
    }

    /// <summary>
    ///     衝突イベント(Modelに登録する)
    /// </summary>
    public event Action<float> OnCollisionItem;

    public event Action OnCollisionEnemy;

    public void CollisionObstacle(MyCircleCollider obstacle, MyCircleCollider other)
    {
        var presenter = _activePresenterSet[obstacle.ID];
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
        }
    }

    /// <summary>
    ///     障害物の生成及び移動を行うメソッド
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
        foreach (var presenter in _activePresenterSet.Values) presenter.UpdateObstacleMove(deltaTime, speed);
    }

    /// <summary>
    ///     ゲーム終了時の初期化
    /// </summary>
    public void Reset()
    {
        foreach (var pair in _activePresenterSet) _removeObstacleSet.Add(pair.Key, pair.Value);
        foreach (var presenter in _removeObstacleSet.Values) ReleaseObstacle(presenter);
        _activePresenterSet.Clear();
        _removeObstacleSet.Clear();
        _makeObstacleDistance = 0f;
    }

    public List<MyCircleCollider> GetObstacleColliders()
    {
        List<MyCircleCollider> obstacleColliders = new(30);
        foreach (var presenter in _activePresenterSet.Values) obstacleColliders.Add(presenter.Collider);
        return obstacleColliders;
    }

    /// <summary>
    ///     ObstaclePresenterとObstacleViewにはIPausableを実装せずにここから呼び出している。
    /// </summary>
    public void Pause()
    {
        foreach (var presenter in _activePresenterSet.Values) presenter.Pause();
    }

    public void Resume()
    {
        foreach (var presenter in _activePresenterSet.Values) presenter.Resume();
    }

    private void RegisterEvent()
    {
        _obstacleGenerator.OnCollisionItem += x => OnCollisionItem?.Invoke(x);
        _obstacleGenerator.OnCollisionEnemy += () => OnCollisionEnemy?.Invoke();
    }

    private void UnRegisterEvent()
    {
        _obstacleGenerator.OnCollisionItem -= x => OnCollisionItem?.Invoke(x);
        _obstacleGenerator.OnCollisionEnemy -= () => OnCollisionEnemy?.Invoke();
    }

    /// <summary>
    ///     プールからObstacleを取り出す
    /// </summary>
    private void GetRandomObstacle()
    {
        var randomIndex = Random.Range(0, ObstacleDataSet.Count());
        _obstacleGenerator.GetObstacle(ObstacleDataSet[randomIndex], out var presenter);
        presenter.SetInitializePosition(
            new Vector2(
                Random.Range(InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin)
                , InGameConst.WindowHeight + _obstacleGeneratorSetting.ObstacleSetYPosition
            )
        );
        _activePresenterSet.Add(presenter.Collider.ID, presenter);
    }

    /// <summary>
    ///     Obstacleをプールに戻す
    /// </summary>
    private void ReleaseObstacle(IObstaclePresenter presenter)
    {
        _obstacleGenerator.ReleasePresenter(presenter);
        _activePresenterSet.Remove(presenter.Collider.ID);
    }
}
