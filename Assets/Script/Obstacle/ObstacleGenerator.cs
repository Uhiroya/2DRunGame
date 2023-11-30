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
    IReadOnlyReactiveDictionary<IObstaclePresenter, Vector2> ObstaclePosition { get; }
    void UpdateObstacleMove(float deltaTime, float speed);
    void Release(IObstaclePresenter obstaclePresenter);
    void Reset();
}
public class ObstacleGenerator : IObstacleGenerator ,  System.IDisposable
{
    Transform _parentTransform;
    ObstacleGeneratorSetting _obstacleGeneratorSetting;

    [Inject] readonly System.Func<ObstacleData,int ,IObstaclePresenter> _obstaclePresenterFactory;
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

    /// <summary>
    /// 次障害物を作成するまでの距離
    /// </summary>
    float _distance;
    List<ObstacleData> _obstacleDataSet => _obstacleGeneratorSetting.ObstacleDataSet;
    /// <summary>
    /// Obstacleの種類毎のオブジェクトプール
    /// </summary>
    ///<param name = "name" > ObstacleDataSetのIndex </ param >
    Dictionary<int, ObjectPool<GameObject>> _objectPool = new();
    /// <summary>
    /// ObstaclePresenter(Model)のユニークIDとの参照関係
    /// </summary>
    Dictionary<int, IObstaclePresenter> _presenterReference = new();
    Dictionary<GameObject , int> _instanceReference = new();
    Dictionary<int ,GameObject> _instanceReverseReference = new();
    

    public readonly ReactiveDictionary<IObstaclePresenter, Vector2> _obstaclePosition = new();
    public IReadOnlyReactiveDictionary<IObstaclePresenter, Vector2> ObstaclePosition => _obstaclePosition;
    public void Release(IObstaclePresenter obstaclePresenter)
    {
        _objectPool[obstaclePresenter.DataIndex]
            .Release(_instanceReverseReference[obstaclePresenter.ModelID]);
    }

    public void Reset()
    {
        foreach (var pair in _instanceReference)
        {
            if (pair.Key.activeSelf)
            {
                _objectPool[_presenterReference[pair.Value].DataIndex].Release(pair.Key);
            }
        }
        _distance = 0f;
    }

    public void UpdateObstacleMove(float deltaTime, float speed)
    {
        _distance += deltaTime * speed * InGameConst.WindowHeight;
        if (_distance > _obstacleGeneratorSetting.ObstacleMakePerDistance)
        {
            GameObject obj;
            IObstaclePresenter presenter;
            var obstacleDataIndex = Random.Range(0, _obstacleDataSet.Count());
            if (_objectPool.ContainsKey(obstacleDataIndex))
            {
                _objectPool[obstacleDataIndex].Get(out obj);
                if (!_instanceReference.ContainsKey(obj))
                {
                    BindObstacleReference(obj, obstacleDataIndex, out presenter);
                    presenter.Position
                        .Subscribe(x => _obstaclePosition[presenter] = x).AddTo(_disposable);
                }
                else
                {
                    presenter = _presenterReference[_instanceReference[obj]];
                }
            }
            else
            {
                _objectPool.Add(obstacleDataIndex, InisializeObjectPool(obstacleDataIndex));
                _objectPool[obstacleDataIndex].Get(out obj);
                BindObstacleReference(obj, obstacleDataIndex, out presenter);
                presenter.Position.Subscribe(x => _obstaclePosition[presenter] = x).AddTo(_disposable);
            }


            presenter.SetObstacle(
                Random.Range(InGameConst.GroundXMargin, InGameConst.WindowWidth - InGameConst.GroundXMargin)
                , InGameConst.WindowHeight + _obstacleGeneratorSetting.ObstacleFrameOutRange
                );
            _distance = 0;
        }

        foreach (var pair in _instanceReference)
        {
            if (pair.Key.activeSelf)
            {
                _presenterReference[pair.Value].UpdateObstacleMove(deltaTime, speed);
                if (pair.Key.transform.position.y < -_obstacleGeneratorSetting.ObstacleFrameOutRange)
                {
                    _objectPool[_presenterReference[pair.Value].DataIndex].Release(pair.Key);
                }
            }
        }
    }
    ObjectPool<GameObject> InisializeObjectPool(int obstacleDataIndex)
    {
        var target = _obstacleDataSet[obstacleDataIndex];
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
    void BindObstacleReference(GameObject obj, int obstacleDataIndex, out IObstaclePresenter presenter)
    {
        presenter = _obstaclePresenterFactory.Invoke(_obstacleDataSet[obstacleDataIndex] , obstacleDataIndex);
        presenter.SetTransform(obj.transform);
        int presenterID = presenter.ModelID;
        _presenterReference.Add(presenterID, presenter);
        _instanceReference.Add(obj, presenterID);
        _instanceReverseReference.Add(presenterID, obj);
    }
}
