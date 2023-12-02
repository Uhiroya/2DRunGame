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
    IReadOnlyReactiveDictionary<IObstaclePresenter, Vector2> ObstaclePosition { get; }
    void UpdateObstacleMove(float deltaTime, float speed);
    void ReleaseObstacle(IObstaclePresenter obstaclePresenter);
    public void ObstacleSetInitializePosition(in IObstaclePresenter presenter);
    void Reset();
}
public class ObstacleManager : IObstacleManager ,  System.IDisposable
{
    IObstacleGenerator _obstacleGenerator;
    ObstacleGeneratorSetting _obstacleGeneratorSetting;
    public ObstacleManager(ObstacleGeneratorSetting obstacleGeneratorSetting , IObstacleGenerator obstacleGenerator)
    {
        _obstacleGenerator = obstacleGenerator;
        _obstacleGeneratorSetting = obstacleGeneratorSetting;
    }
    List<ObstacleData> _obstacleDataSet => _obstacleGeneratorSetting.ObstacleDataSet;
    Dictionary<IObstaclePresenter,IDisposable> _presenterDisposablePair = new();
    //障害物全てのポジションの更新を受け取り、公開する。
    public readonly ReactiveDictionary<IObstaclePresenter, Vector2> _obstaclePosition = new();
    public IReadOnlyReactiveDictionary<IObstaclePresenter, Vector2> ObstaclePosition => _obstaclePosition;

    List<IObstaclePresenter> _activePresenterList = new();

    List<IObstaclePresenter> _removeObstacleList = new();
    /// <summary>
    /// 障害物を生成する距離
    /// </summary>
    float _makeObstacleDistance;
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
            var disposable = presenter.Position.Subscribe(x => _obstaclePosition[presenter] = x);
            _presenterDisposablePair.Add(presenter, disposable);
            ObstacleSetInitializePosition(in presenter);
            _activePresenterList.Add(presenter);
            _makeObstacleDistance = 0;
        }
        //障害物をスクロールさせる。
        foreach (var presenter in _activePresenterList)
        {
            presenter.UpdateObstacleMove(deltaTime, speed);
            if (presenter.Position.Value.y < -_obstacleGeneratorSetting.ObstacleFrameOutRange)
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
    public void ObstacleSetInitializePosition(in IObstaclePresenter presenter)
    {
        presenter.SetObstacle(
            UnityEngine.Random.Range(InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin)
            , InGameConst.WindowHeight + _obstacleGeneratorSetting.ObstacleFrameOutRange
            );
    }
    public void ReleaseObstacle(IObstaclePresenter presenter)
    {
        _obstacleGenerator.ReleaseObstacle(presenter);
        _presenterDisposablePair[presenter].Dispose();
        _presenterDisposablePair.Remove(presenter);
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
}

