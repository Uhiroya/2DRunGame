using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using System.Linq;
using Unity.VisualScripting;
using System.ComponentModel;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField] ObstaclePresenter _obstacle;
    [SerializeField] float _obstacleMakeDistance = 15f;
    [SerializeField] float _yFrameOut = 20f;
    public readonly ReactiveProperty<bool> IsHit = new(false);
    ObjectPool<ObstaclePresenter> _obstaclePool;
    HashSet<ObstaclePresenter> _obstacleList = new();
    float _distance = 0f;
    private void Start()
    {
        _obstaclePool = new(
            createFunc: () => 
            {
                var target = Instantiate(_obstacle, transform);
                target.IsHit.Where(x => x == true).Subscribe(x => ReleaseObstacle(target)).AddTo(this);
                return target;
            } ,// �v�[������̂Ƃ��ɐV�����C���X�^���X�𐶐����鏈��
            actionOnGet: target =>
            {
                target.gameObject.SetActive(true);
            },// �v�[��������o���ꂽ�Ƃ��̏��� 
            actionOnRelease: target =>
            {
                target.gameObject.SetActive(false);
            }
            ,// �v�[���ɖ߂����Ƃ��̏���
            actionOnDestroy: target =>
            {
                print("�j��������");
                Destroy(target.gameObject);
            }, // �v�[����maxSize�𒴂����Ƃ��̏���
            collectionCheck: true, // ����C���X�^���X���o�^����Ă��Ȃ����`�F�b�N���邩�ǂ���
            defaultCapacity: 10,   // �f�t�H���g�̗e��
            maxSize: 100
        );
    }
    public void ReleaseObstacle(ObstaclePresenter hitObj)
    {
        _obstaclePool.Release(hitObj);
    }
    public void Reset()
    {
        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeSelf) continue;
            if (child.TryGetComponent<ObstaclePresenter>(out var obj))
            {
                _obstaclePool.Release(obj);
            }
            else
            {
                Destroy(child.gameObject);
            }           
        }
        IsHit.Value = false;
        _distance = 0f;
    }
    public void ManualUpdate(float deltaTime, float speed)
    {
        _distance += deltaTime * speed * Screen.height;
        if (_distance > _obstacleMakeDistance)
        {
            _obstaclePool.Get(out var obj);
            obj.SetObstacle(
                Random.Range(GamePresenter.MapXMargin, Screen.width - GamePresenter.MapXMargin)
                , Screen.height + _yFrameOut
                );
            _obstacleList.Add(obj);
            _distance = 0;
        }

        foreach (var obj in _obstacleList)
        {
            if (obj.isActiveAndEnabled)
            {
               // obj.transform.position -= new Vector3(0, deltaTime * speed * Screen.height, 0);
                obj.ManualUpdate(deltaTime, speed);
                if (obj.transform.position.y < -_yFrameOut)
                {
                    _obstaclePool.Release(obj);
                }
            }
        }
    }
}
