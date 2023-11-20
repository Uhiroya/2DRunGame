using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using System.Linq;
using VContainer;
using VContainer.Unity;
public interface IObstacleGenerator 
{
    IReadOnlyReactiveDictionary<IObstaclePresenter, Vector2> ObstaclePosition { get; }
    void UpdateObstacleMove(float deltaTime, float speed);
    void Release(IObstaclePresenter obstaclePresenter);
    void Reset();
}
public class ObstacleGenerator : IObstacleGenerator , IStartable
{
    Transform _parentTransform;
    float _obstacleMakeDistance;
    float _yFrameOut;
    ObjectPool<IObstaclePresenter> _obstaclePool;
    Dictionary<IObstaclePresenter,GameObject> _obstacleDataRef = new();
    public Dictionary<IObstaclePresenter,GameObject> ObstacleDataRef => _obstacleDataRef;
    /// <summary>
    /// 次障害物を作成するまでの距離
    /// </summary>
    float _distance;
    public readonly ReactiveDictionary<IObstaclePresenter,Vector2> _obstaclePosition = new();
    public IReadOnlyReactiveDictionary<IObstaclePresenter, Vector2> ObstaclePosition => _obstaclePosition;
    //GamePresenterに移行予定
    public System.Action<float> AddScore;
    [Inject] IObjectResolver _container;
    public ObstacleGenerator(Transform parentTransform , float obstacleMakeDistance, float yFrameOut)
    {
        _parentTransform = parentTransform;
        _obstacleMakeDistance = obstacleMakeDistance;
        _yFrameOut = yFrameOut;
    }
    public void Start()
    {
        _obstaclePool = new(
            createFunc: () =>
            {
                var target = _container.Resolve<IObstaclePresenter>();
                var obj = Object.Instantiate(target.ObstacleData.Obstacle, _parentTransform);
                target.SetTransform(obj.transform);
                _obstacleDataRef.Add(target , obj);
                target.Position.Subscribe(x => _obstaclePosition[target] = x);
                return target;
            },// プールが空のときに新しいインスタンスを生成する処理
            actionOnGet: target =>
            {
                _obstacleDataRef[target].SetActive(true);
            },// プールから取り出されたときの処理 
            actionOnRelease: target =>
            {
                _obstacleDataRef[target].SetActive(false);
            }
            ,// プールに戻したときの処理
            actionOnDestroy: target =>
            {
                Object.Destroy(_obstacleDataRef[target]);
                _obstacleDataRef.Remove(target);
            }, // プールがmaxSizeを超えたときの処理
            collectionCheck: true, // 同一インスタンスが登録されていないかチェックするかどうか
            defaultCapacity: 10,   // デフォルトの容量
            maxSize: 100
        );
        //一つ目を作っておかないと初回生成が重い
        _obstaclePool.Get(out var obj);
        _obstaclePool.Release(obj);
    }
    public void Release(IObstaclePresenter obstaclePresenter)
    {
        _obstaclePool.Release(obstaclePresenter);
    }

    public void Reset()
    {
        foreach (var pair in _obstacleDataRef)
        {
            if (pair.Value.activeSelf)
            {
                _obstaclePool.Release(pair.Key);
            }
        }
        _distance = 0f;
    }
    public void UpdateObstacleMove(float deltaTime, float speed)
    {
        _distance += deltaTime * speed * InGameConst.WindowHeight;
        if (_distance > _obstacleMakeDistance)
        {
            _obstaclePool.Get(out var obj);
             obj.SetObstacle(
                Random.Range(InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin)
                , InGameConst.WindowHeight + _yFrameOut
                );
            _distance = 0;
        }

        foreach (var pair in _obstacleDataRef)
        {
            if (pair.Value.activeSelf)
            {
                pair.Key.UpdateObstacleMove(deltaTime, speed);
                if (pair.Value.transform.position.y < -_yFrameOut)
                {
                    _obstaclePool.Release(pair.Key);
                }
            }
        }
    }
}
