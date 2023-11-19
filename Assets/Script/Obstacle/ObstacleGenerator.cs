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
    /// ����Q�����쐬����܂ł̋���
    /// </summary>
    float _distance;
    public readonly ReactiveDictionary<IObstaclePresenter,Vector2> _obstaclePosition = new();
    public IReadOnlyReactiveDictionary<IObstaclePresenter, Vector2> ObstaclePosition => _obstaclePosition;
    //GamePresenter�Ɉڍs�\��
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
            },// �v�[������̂Ƃ��ɐV�����C���X�^���X�𐶐����鏈��
            actionOnGet: target =>
            {
                _obstacleDataRef[target].SetActive(true);
            },// �v�[��������o���ꂽ�Ƃ��̏��� 
            actionOnRelease: target =>
            {
                _obstacleDataRef[target].SetActive(false);
            }
            ,// �v�[���ɖ߂����Ƃ��̏���
            actionOnDestroy: target =>
            {
                Object.Destroy(_obstacleDataRef[target]);
                _obstacleDataRef.Remove(target);
            }, // �v�[����maxSize�𒴂����Ƃ��̏���
            collectionCheck: true, // ����C���X�^���X���o�^����Ă��Ȃ����`�F�b�N���邩�ǂ���
            defaultCapacity: 10,   // �f�t�H���g�̗e��
            maxSize: 100
        );
        //��ڂ�����Ă����Ȃ��Ə��񐶐����d��
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
