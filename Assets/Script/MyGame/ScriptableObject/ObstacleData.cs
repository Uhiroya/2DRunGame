using System;
using UnityEngine;

namespace MyScriptableObjectClass
{
    [CreateAssetMenu(menuName = "ScriptableObject/ObstacleData")]
    [Serializable]
    public class ObstacleData : ScriptableObject
    {
        //コピー用Prefab、Generatorへ公開するデータ
        [SerializeField] private GameObject _obstacle;

        [SerializeField] private int _obstacleID;

        //Gamepresenterに処理させるために公開するデータ
        [SerializeField] private CollisionTag _itemType;
        [SerializeField] private float _hitRange;

        [SerializeField] private float _score;

        //Modelのみ
        [SerializeField] private GameObject _destroyEffect;
        [SerializeField] private AnimationClip _destroyAnimation;
        [SerializeField] [Range(0f, 1f)] private float _xMoveArea;
        [SerializeField] private float _xMoveSpeed;
        [SerializeField] private float _yMoveSpeed;

        public GameObject Obstacle => _obstacle;
        public int ObstacleID => _obstacleID;
        public CollisionTag CollisionType => _itemType;
        public float HitRange => _hitRange;
        public float Score => _score;
        public GameObject DestroyEffect => _destroyEffect;
        public AnimationClip DestroyAnimation => _destroyAnimation;
        public float XMoveArea => _xMoveArea;
        public float XMoveSpeed => _xMoveSpeed;
        public float YMoveSpeed => _yMoveSpeed;
    }

    [Serializable]
    public class ObstaclePublicInfo
    {
        [SerializeField] private CollisionTag _itemType;
        [SerializeField] private float _hitRange;
        [SerializeField] private float _score;

        public CollisionTag ItemType => _itemType;
        public float HitRange => _hitRange;
        public float Score => _score;
    }
}
