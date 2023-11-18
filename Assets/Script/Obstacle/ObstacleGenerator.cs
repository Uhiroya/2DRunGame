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
    GameObject _obstacle;
    GameObject _explotionEffect;
    float _obstacleMakeDistance = 500f;
    float _yFrameOut = 5f;
    float _hitRange = 20f;
    Transform _parentTransform;
    [Inject] IPlayerPresenter _player;
    [Inject] IObjectResolver _container;
    ObjectPool<GameObject> _obstaclePool;
    Dictionary<GameObject, ObstaclePresenter> _obstacleDic = new();

    public System.Action<float> AddScore;
    float _distance = 0f;
    public ObstacleGenerator(GameObject obstacle
        , GameObject explotionEffect, float obstacleMakeDistance, float yFrameOut
        , float hitRange, Transform parentTransform 
        )
    {
        _obstacle = obstacle;
        _explotionEffect = explotionEffect;
        _obstacleMakeDistance = obstacleMakeDistance;
        _yFrameOut = yFrameOut;
        _hitRange = hitRange;
        _parentTransform = parentTransform;
    }
    public void Start()
    {
        _obstaclePool = new(
            createFunc: () =>
            {
                var target = Object.Instantiate(_obstacle , _parentTransform);
                _obstacleDic.Add(target, _container.Resolve<ObstaclePresenter>());
                _obstacleDic[target].Create(target.GetComponent<Collider2D>() , target.transform);
                _obstacleDic[target].Position.Where(x => Vector2.Distance(x , _player.PlayerPosition) < _hitRange)
                        .Subscribe(x => ReleaseObstacle(_obstacleDic[target].ObstacleType, target));
                return target;
            },// プールが空のときに新しいインスタンスを生成する処理
            actionOnGet: target =>
            {
                target.gameObject.SetActive(true);
            },// プールから取り出されたときの処理 
            actionOnRelease: target =>
            {
                target.gameObject.SetActive(false);
            }
            ,// プールに戻したときの処理
            actionOnDestroy: target =>
            {
                Object.Destroy(target);
            }, // プールがmaxSizeを超えたときの処理
            collectionCheck: true, // 同一インスタンスが登録されていないかチェックするかどうか
            defaultCapacity: 10,   // デフォルトの容量
            maxSize: 100
        );
        //一つ目を作っておかないと初回生成が重い
        _obstaclePool.Get(out var obj);
        _obstaclePool.Release(obj);
    }
    public void ReleaseObstacle(ObstacleType itemType, GameObject obstacle)
    {
        //インターフェースでの実装に差し替える。
        switch (itemType)
        {
            case ObstacleType.Item:
                AddScore?.Invoke(_obstacleDic[obstacle].GetScore());
                break;
            case ObstacleType.Enemy:
                _player.GameOver();
                Object.Instantiate(_explotionEffect, _player.PlayerPosition,Quaternion.identity, _parentTransform );
                break;
            default:
                break;
        }
        
        _obstaclePool.Release(obstacle);
    }
    public void Reset()
    {
        foreach (Transform child in _parentTransform)
        {
            if (_obstacleDic.ContainsKey(child.gameObject))
            {
                _obstacleDic[child.gameObject].SetObstacle(
                    0f
                    , InGameConst.WindowHeight + _yFrameOut
                );
                if (child.gameObject.activeSelf)
                {
                    //稀に例外が出るTrying to release an object that has already been released to the pool.
                    _obstaclePool.Release(child.gameObject);
                } 
            }
            else
            {
                Object.Destroy(child.gameObject);
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
            _obstacleDic[obj].SetObstacle(
                Random.Range(InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin)
                , InGameConst.WindowHeight + _yFrameOut
                );
            _distance = 0;
        }

        foreach (var obj in _obstacleDic.Keys)
        {
            if (obj.activeSelf)
            {
                // obj.transform.position -= new Vector3(0, deltaTime * speed * InGameConst.WindowHeight, 0);
                _obstacleDic[obj].UpdateObstacleMove(deltaTime, speed);
                if (obj.transform.position.y < -_yFrameOut)
                {
                    _obstaclePool.Release(obj);
                }
            }
        }
    }
}
