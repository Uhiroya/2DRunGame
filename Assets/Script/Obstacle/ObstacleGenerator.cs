using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using System.Linq;
using VContainer;
using VContainer.Unity;
using MyScriptableObjectClass;
using static UnityEngine.GraphicsBuffer;

public interface IObstacleGenerator 
{
    IReadOnlyReactiveDictionary<IObstaclePresenter, Vector2> ObstaclePosition { get; }
    void UpdateObstacleMove(float deltaTime, float speed);
    void Release(IObstaclePresenter obstaclePresenter);
    void Reset();
}
//public class ObstacleObjectPool
//{
//    List<GameObject> _pool;
//    Dictionary<GameObject, IObstaclePresenter> _obstacleDataReference = new();
//    private readonly System.Func<IObstaclePresenter> _createFunc;
//    private readonly System.Action<GameObject> _actionOnGet;
//    private readonly System.Action<GameObject> _actionOnRelease;
//    private readonly System.Action<GameObject> _actionOnDestroy;
//    public MyObjectPool(System.Func<IObstaclePresenter> createFunc, System.Action<GameObject> actionOnGet = null, System.Action<GameObject> actionOnRelease = null, System.Action<GameObject> actionOnDestroy = null)
//    {
//        _pool = new List<GameObject>();
//        _createFunc = createFunc;
//        _actionOnGet = actionOnGet;
//        _actionOnRelease = actionOnRelease;
//        _actionOnDestroy = actionOnDestroy;
//    }

//    public IObstaclePresenter Get(GameObject obj)
//    {
//        IObstaclePresenter result;
//        obj = null;
//        if (_obstacleDataReference.ContainsKey(obj))
//        {
//            obj.SetActive(true);
//            _obstacleDataReference.TryGetValue(obj ,out result);
//        }
//        else
//        {
//            result = _createFunc();
//            _obstacleDataReference.Add(obj,result);
//        }
//        _actionOnGet(obj);
//        return result;
//    }
//    public void Release((T instance, GameObject obj) tuple)
//    {
//        _pool[tuple.Item1].Enqueue(tuple.Item2);
//        _actionOnRelease(tuple.Item2);
//    }
//    public void Clear()
//    {
//        if (_actionOnDestroy != null)
//        {
//            foreach (var queues in _pool.Values)
//            {
//                foreach (var queue in queues)
//                {
//                    _actionOnDestroy(queue);
//                }
//            }
//        }
//        _pool.Clear();
//    }

//}
public class ObstacleGenerator : IObstacleGenerator ,  System.IDisposable
{
    Transform _parentTransform;
    ObstacleGeneratorSetting _obstacleGeneratorSetting;

    
    public readonly ReactiveDictionary<IObstaclePresenter, Vector2> _obstaclePosition = new();
    public IReadOnlyReactiveDictionary<IObstaclePresenter, Vector2> ObstaclePosition => _obstaclePosition;

    
    /// <summary>
    /// 次障害物を作成するまでの距離
    /// </summary>
    float _distance;
    List<ObstacleData> _obstacleDataSet => _obstacleGeneratorSetting.ObstacleDataSet;
    Dictionary<ObstacleData, ObjectPool<GameObject>> _obstacleDataReference = new();
    Dictionary<GameObject , IObstaclePresenter> _obstacleInstanceReference = new();
    Dictionary<IObstaclePresenter ,GameObject> _obstacleInstanceReverseReference = new();
    
    [Inject] IObjectResolver _container;
    public ObstacleGenerator(ObstacleGeneratorSetting obstacleGeneratorSetting, Transform parentTransform)
    {
        _parentTransform = parentTransform;
        _obstacleGeneratorSetting = obstacleGeneratorSetting;
    }
    CompositeDisposable _disposable = new();
    public void Dispose()
    {
        _disposable.Dispose();
    }
    public ObjectPool<GameObject> InisializeObjectPool(ObstacleData target)
    {
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
                Object.Destroy(target);
            } // プールがmaxSizeを超えたときの処理
        );
        //一つ目を作っておかないと初回生成が重い
        obstaclePool.Get(out var obj);
        obstaclePool.Release(obj);
        return obstaclePool;
    }

    public void Release(IObstaclePresenter obstaclePresenter)
    {
        _obstacleDataReference[obstaclePresenter.ObstacleData]
            .Release(_obstacleInstanceReverseReference[obstaclePresenter]);
    }

    public void Reset()
    {
        foreach (var pair in _obstacleInstanceReference)
        {
            if (pair.Key.activeSelf)
            {
                _obstacleDataReference[pair.Value.ObstacleData].Release(pair.Key);
            }
        }
        _distance = 0f;
    }
    void BindObstacleReference(GameObject obj ,ObstacleData obstacleData , out IObstaclePresenter target)
    {
        target = _container.Resolve<IObstaclePresenter>();
        target.SetObstacleData(obstacleData);
        target.SetTransform(obj.transform);
        _obstacleInstanceReference.Add(obj, target);
        _obstacleInstanceReverseReference.Add(target, obj);
        
    }
    public void UpdateObstacleMove(float deltaTime, float speed)
    {
        _distance += deltaTime * speed * InGameConst.WindowHeight;
        if (_distance > _obstacleGeneratorSetting.ObstacleMakePerDistance)
        {
            GameObject obj;
            IObstaclePresenter target;
            var randomObstacleData = _obstacleDataSet[Random.Range(0, _obstacleDataSet.Count())];
            if (_obstacleDataReference.ContainsKey(randomObstacleData))
            {
                _obstacleDataReference[randomObstacleData].Get(out obj);
                if (!_obstacleInstanceReference.ContainsKey(obj))
                {
                    BindObstacleReference(obj, randomObstacleData, out target);
                    target.Position.Subscribe(x => _obstaclePosition[target] = x).AddTo(_disposable);
                }
                else
                {
                    target = _obstacleInstanceReference[obj];
                }
            }
            else
            {
                _obstacleDataReference.Add(randomObstacleData, InisializeObjectPool(randomObstacleData));
                _obstacleDataReference[randomObstacleData].Get(out obj);
                BindObstacleReference(obj, randomObstacleData, out target);
                target.Position.Subscribe(x => _obstaclePosition[target] = x).AddTo(_disposable);
            }


            target.SetObstacle(
                Random.Range(InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin)
                , InGameConst.WindowHeight + _obstacleGeneratorSetting.ObstacleFrameOutRange
                );
            _distance = 0;
        }

        foreach (var pair in _obstacleInstanceReference)
        {
            if (pair.Key.activeSelf)
            {
                pair.Value.UpdateObstacleMove(deltaTime, speed);
                if (pair.Key.transform.position.y < -_obstacleGeneratorSetting.ObstacleFrameOutRange)
                {
                    _obstacleDataReference[pair.Value.ObstacleData].Release(pair.Key);
                }
            }
        }
    }
}
