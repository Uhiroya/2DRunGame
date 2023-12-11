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
    List<(Circle, IObstaclePresenter)> GetObstacleColliders();
    void OnGameFlowStateChanged(GameFlowState gameFlowState);
    void UpdateObstacleMove(float deltaTime, float speed);
    void SetObstacleInitializePosition(in IObstaclePresenter presenter);
    void HitObstacle(in IObstaclePresenter presenter);
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

    List<IObstaclePresenter> _activePresenterList = new();
    List<IObstaclePresenter> _removeObstacleList = new();
    /// <summary>
    /// 障害物を生成する距離
    /// </summary>
    float _makeObstacleDistance;
    
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
            SetObstacleInitializePosition(in presenter);
            _activePresenterList.Add(presenter);
            _makeObstacleDistance = 0;
        }
        //障害物をスクロールさせる。
        foreach (var presenter in _activePresenterList)
        {
            presenter.UpdateObstacleMove(deltaTime, speed);
            if (presenter.GetCollider().GetCenter().y < -_obstacleGeneratorSetting.ObstacleFrameOutRange)
            {
                _removeObstacleList.Add(presenter);
            }
        }
        foreach (var presenter in _removeObstacleList)
        {
            ReleaseObstacle(presenter);
        }
        _removeObstacleList.Clear();
    }
    public void Reset()
    {
        foreach (var presenter in _activePresenterList)
        {
            _removeObstacleList.Add(presenter);
        }
        foreach (var presenter in _removeObstacleList)
        {
            ReleaseObstacle(presenter);
        }
        _activePresenterList.Clear();
        _removeObstacleList.Clear();
        _makeObstacleDistance = 0f;
    }
    public void SetObstacleInitializePosition(in IObstaclePresenter presenter)
    {
        presenter.SetObstacle(
            UnityEngine.Random.Range(InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin)
            , InGameConst.WindowHeight + _obstacleGeneratorSetting.ObstacleFrameOutRange
            );
    }
    public void HitObstacle(in IObstaclePresenter presenter)
    {
        presenter.SetObstacle(0f, -_obstacleGeneratorSetting.ObstacleFrameOutRange);
    }
    void ReleaseObstacle(IObstaclePresenter presenter)
    {
        _obstacleGenerator.ReleaseObstacle(presenter);
        _activePresenterList.Remove(presenter);
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
        foreach (var presenter in _activePresenterList)
        {
            presenter.Pause();
        }
    }
    public void Resume()
    {
        foreach (var presenter in _activePresenterList)
        {
            presenter.Resume();
        }
    }
    public List<(Circle , IObstaclePresenter)> GetObstacleColliders()
    {
        List<(Circle, IObstaclePresenter)> obstacleColliders = new();
        foreach (var presenter in _activePresenterList)
        {
            obstacleColliders.Add((presenter.GetCollider() , presenter));
        }
        return obstacleColliders;
    }
}

