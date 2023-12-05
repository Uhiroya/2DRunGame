using System;
using UnityEngine;

namespace MyScriptableObjectClass
{
    [CreateAssetMenu(menuName = "ScriptableObject/ObstacleData")]
    [System.Serializable]
    public class ObstacleData : ScriptableObject
    {
        //コピー用Prefab、Generatorへ公開するデータ
        [SerializeField] GameObject _obstacle;
        [SerializeField] int _obstacleID;
        //Gamepresenterに処理させるために公開するデータ
        [SerializeField] ObstaclePublicInfo _obstacleInfo;
        //Modelのみ
        [SerializeField] GameObject _destroyEffect;
        [SerializeField] AnimationClip _destroyAnimation;
        [SerializeField, Range(0f, 1f)] float _xMoveArea;
        [SerializeField] float _xMoveSpeed;
        [SerializeField] float _yMoveSpeed;

        public GameObject Obstacle => _obstacle;
        public int ObstacleID => _obstacleID;   
        public ObstaclePublicInfo ObstacleInfo => _obstacleInfo;
        public GameObject DestroyEffect => _destroyEffect;
        public AnimationClip DestroyAnimation => _destroyAnimation;
        public float XMoveArea => _xMoveArea;
        public float XMoveSpeed => _xMoveSpeed;
        public float YMoveSpeed => _yMoveSpeed;
    }

    [System.Serializable]
    public class ObstaclePublicInfo
    {
        [SerializeField] ItemType _itemType;
        [SerializeField] float _hitRange;
        [SerializeField] float _score;

        public ItemType ItemType => _itemType;
        public float HitRange => _hitRange;
        public float Score => _score;
    }

}


