using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using System.Linq;
using VContainer;
using VContainer.Unity;
using MyScriptableObjectClass;
public interface IObstacleGenerator
{
    public void ReleaseObstacle(IObstaclePresenter presenter);
    GameObject GetObstacle(ObstacleData obstacleData, out IObstaclePresenter presenter);
}
public class ObstacleGenerator : IObstacleGenerator, System.IDisposable
{
    Transform _parentTransform;

    [Inject] readonly System.Func<ObstacleData, IObstaclePresenter> _obstaclePresenterFactory;
    public ObstacleGenerator(Transform parentTransform)
    {
        _parentTransform = parentTransform;
    }
    CompositeDisposable _disposable = new();
    public void Dispose()
    {
        _disposable.Dispose();
    }
    /// <summary>
    /// Obstacleの種類毎のオブジェクトプール
    /// </summary>
    ///<param name = "name" > ObstacleID </ param >
    Dictionary<int, ObjectPool<GameObject>> _objectPool = new();
    Dictionary<GameObject, IObstaclePresenter> _objectToPresenterReference = new();
    Dictionary<IObstaclePresenter , GameObject> _presenterToObjectReference = new();
    /// <summary>
    /// オブジェクトプールの初期化
    /// </summary>
    /// <param name="obstacleData"></param>
    /// <returns></returns>
    ObjectPool<GameObject> InisializeObjectPool(ObstacleData obstacleData)
    {
        var target = obstacleData;
        var obstaclePool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                return Object.Instantiate(target.Obstacle, _parentTransform);
            },// プールが空のときに新しいインスタンスを生成する処理
            actionOnGet: target =>
            {
                target.SetActive(true);
            },// プールから取り出されたときの処理 
            actionOnRelease: target =>
            {
                target.SetActive(false);
            }
            ,// プールに戻したときの処理
            actionOnDestroy: target =>
            {
                ReleaseObstacle(_objectToPresenterReference[target]);
                Object.Destroy(target);
            } // プールがmaxSizeを超えたときの処理
        );
        //一つ目を作っておかないと初回生成が重い
        obstaclePool.Get(out var obj);
        obstaclePool.Release(obj);
        return obstaclePool;
    }
    public void ReleaseObstacle(IObstaclePresenter presenter)
    {
        _objectPool[presenter.ObstacleID]
            .Release(_presenterToObjectReference[presenter]);
    }
    public GameObject GetObstacle(ObstacleData obstacleData , out IObstaclePresenter presenter)
    {
        GameObject obj;
        if (_objectPool.ContainsKey(obstacleData.ObstacleID))
        {
            _objectPool[obstacleData.ObstacleID].Get(out obj);
            if (_objectToPresenterReference.ContainsKey(obj))
            {
                presenter = _objectToPresenterReference[obj];      
            }
            else
            {
                MakePresenter(obj, obstacleData, out presenter);
            }
        }
        else
        {
            _objectPool.Add(obstacleData.ObstacleID, InisializeObjectPool(obstacleData));
            _objectPool[obstacleData.ObstacleID].Get(out obj);
            MakePresenter(obj, obstacleData, out presenter);
        }
        return obj;
    }
    void MakePresenter(GameObject obj, ObstacleData obstacleData, out IObstaclePresenter presenter)
    {
        presenter = _obstaclePresenterFactory.Invoke(obstacleData);
        presenter.SetTransform(obj.transform);
        _objectToPresenterReference.Add(obj, presenter);
        _presenterToObjectReference.Add(presenter, obj);
    }
}
