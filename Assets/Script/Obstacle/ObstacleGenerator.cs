using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using System.Linq;
using VContainer;
using VContainer.Unity;

public class ObstacleGenerator : IStartable
{
    float _obstacleMakeDistance;
    float _yFrameOut;
    Transform _parentTransform;
    [Inject] IPlayerPresenter _player;
    [Inject] IObjectResolver _container;

    ObjectPool<ObstaclePresenter> _obstaclePool;
    Dictionary<ObstaclePresenter,GameObject> _obstacleDataRef = new();
    float _distance;
    //GamePresenterに移行予定
    public System.Action<float> AddScore;
    
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
                var target = _container.Resolve<ObstaclePresenter>();
                var obj = Object.Instantiate(target.ObstacleData.Obstacle, _parentTransform);
                target.SetTransform(obj.transform);
                _obstacleDataRef.Add(target , obj);
                target.Position.Where(x => Vector2.Distance(x , _player.PlayerPosition) < target.ObstacleData.HitRange)
                        .Subscribe(x => ReleaseObstacle(target));
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
    //GamePresenterに移行予定
    public void ReleaseObstacle(ObstaclePresenter obstacle)
    {
        Debug.Log("ぶつかってる");
        //インターフェースでの実装に差し替える。
        switch (obstacle.ObstacleData.Param.ItemType)
        {
            case ObstacleType.Item:
                AddScore?.Invoke(obstacle.ObstacleData.Score);
                break;
            case ObstacleType.Enemy:
                _player.GameOver();
                var obj = Object.Instantiate(obstacle.ObstacleData.DestroyEffect, _player.PlayerPosition,Quaternion.identity, _parentTransform );
                Object.Destroy(obj , 3f);//GamePresenterに移行予定
                break;
            default:
                break;
        }
        
        _obstaclePool.Release(obstacle);
    }
    public void Reset()
    {
        foreach (var pair in _obstacleDataRef)
        {
            if (pair.Value.activeSelf)
            {
                _obstaclePool.Release(pair.Key);
                //稀に例外が出るかも Trying to release an object that has already been released to the pool.
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
                // obj.transform.position -= new Vector3(0, deltaTime * speed * InGameConst.WindowHeight, 0);
                pair.Key.UpdateObstacleMove(deltaTime, speed);
                if (pair.Value.transform.position.y < -_yFrameOut)
                {
                    _obstaclePool.Release(pair.Key);
                }
            }
        }
    }
}
