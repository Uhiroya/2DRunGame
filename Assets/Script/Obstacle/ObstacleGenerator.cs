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
    void UpdateObstacleMove(float deltaTime, float speed);
    void Reset();
}
public class ObstacleGenerator : IObstacleGenerator , IStartable
{
    float _obstacleMakeDistance;
    float _yFrameOut;
    Transform _parentTransform;
    ObjectPool<IObstaclePresenter> _obstaclePool;
    Dictionary<IObstaclePresenter,GameObject> _obstacleDataRef = new();
    /// <summary>
    /// ����Q�����쐬����܂ł̋���
    /// </summary>
    float _distance;
    //GamePresenter�Ɉڍs�\��
    public System.Action<float> AddScore;
    [Inject] IPlayerPresenter _player;
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
                target.Position.Where(x => Vector2.Distance(x , _player.PlayerPosition) < target.ObstacleData.HitRange)
                        .Subscribe(x => ReleaseObstacle(target));
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
    //GamePresenter�Ɉڍs�\��
    public void ReleaseObstacle(IObstaclePresenter obstacle)
    {
        switch (obstacle.ObstacleData.Param.ItemType)
        {
            case ObstacleType.Item:
                AddScore?.Invoke(obstacle.ObstacleData.Score);
                break;
            case ObstacleType.Enemy:
                _player.GameOver();
                var obj = Object.Instantiate(obstacle.ObstacleData.DestroyEffect, _player.PlayerPosition,Quaternion.identity, _parentTransform );
                Object.Destroy(obj , 3f);//GamePresenter�Ɉڍs�\��
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
