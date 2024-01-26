using System;
using System.Collections.Generic;
using MyScriptableObjectClass;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using Object = UnityEngine.Object;

public interface IObstacleGenerator
{
    event Action<float> OnCollisionItem;
    event Action OnCollisionEnemy;
    public void ReleasePresenter(IObstaclePresenter presenter);
    GameObject GetObstacle(ObstacleData obstacleData, out IObstaclePresenter presenter);
}

public class ObstacleGenerator : IObstacleGenerator, IDisposable
{
    private readonly CompositeDisposable _disposable = new();

    /// <summary>
    ///     Obstacleの種類毎のオブジェクトプール
    /// </summary>
    /// <param> ObstacleID </param>
    private readonly Dictionary<int, ObjectPool<GameObject>> _objectPool = new();

    private readonly Dictionary<GameObject, IObstaclePresenter> _objectToPresenterReference = new();
    [Inject] private readonly Func<Transform, ObstacleData, Animator, IObstaclePresenter> _obstaclePresenterFactory;
    private readonly Transform _parentTransform;
    private readonly Dictionary<IObstaclePresenter, GameObject> _presenterToObjectReference = new();

    public ObstacleGenerator(Transform parentTransform)
    {
        _parentTransform = parentTransform;
    }

    public void Dispose()
    {
        _disposable.Dispose();
    }

    /// <summary>
    ///     衝突イベント(Modelに登録する)
    /// </summary>
    public event Action<float> OnCollisionItem;

    public event Action OnCollisionEnemy;

    public void ReleasePresenter(IObstaclePresenter presenter)
    {
        UnRegisterEvent(in presenter);
        _objectPool[presenter.ObstacleID].Release(_presenterToObjectReference[presenter]);
        presenter.Dispose();
    }

    public GameObject GetObstacle(ObstacleData obstacleData, out IObstaclePresenter presenter)
    {
        GameObject obj;
        if (_objectPool.TryGetValue(obstacleData.ObstacleID, out var value))
        {
            value.Get(out obj);
            if (_objectToPresenterReference.TryGetValue(obj, out presenter)) {}
            else MakePresenter(obj, obstacleData, out presenter);
        }
        else
        {
            _objectPool.Add(obstacleData.ObstacleID, InitializeObjectPool(obstacleData));
            _objectPool[obstacleData.ObstacleID].Get(out obj);
            MakePresenter(obj, obstacleData, out presenter);
        }

        return obj;
    }

    /// <summary>
    ///     オブジェクトプールの初期化
    /// </summary>
    /// <param name="obstacleData"></param>
    /// <returns></returns>
    private ObjectPool<GameObject> InitializeObjectPool(ObstacleData obstacleData)
    {
        var target = obstacleData;
        var obstaclePool = new ObjectPool<GameObject>(
            () => Object.Instantiate(target.Obstacle, _parentTransform), // プールが空のときに新しいインスタンスを生成する処理
            target => { target.SetActive(true); }, // プールから取り出されたときの処理 
            target => { target.SetActive(false); }
            , // プールに戻したときの処理
            target =>
            {
                _objectToPresenterReference[target].Dispose();
                ReleasePresenter(_objectToPresenterReference[target]);
                Object.Destroy(target);
            } // プールがmaxSizeを超えたときの処理
        );
        //一つ目を作っておかないと初回生成が重い
        obstaclePool.Get(out var obj);
        obstaclePool.Release(obj);
        return obstaclePool;
    }

    private void MakePresenter(GameObject obj, ObstacleData obstacleData, out IObstaclePresenter presenter)
    {
        presenter = _obstaclePresenterFactory.Invoke(obj.transform, obstacleData, obj.GetComponent<Animator>());
        RegisterEvent(in presenter);
        _objectToPresenterReference.Add(obj, presenter);
        _presenterToObjectReference.Add(presenter, obj);
    }

    private void RegisterEvent(in IObstaclePresenter presenter)
    {
        presenter.OnCollisionItem += x => OnCollisionItem?.Invoke(x);
        presenter.OnCollisionEnemy += () => OnCollisionEnemy?.Invoke();
    }

    private void UnRegisterEvent(in IObstaclePresenter presenter)
    {
        presenter.OnCollisionItem -= x => OnCollisionItem?.Invoke(x);
        presenter.OnCollisionEnemy -= () => OnCollisionEnemy?.Invoke();
    }
}
