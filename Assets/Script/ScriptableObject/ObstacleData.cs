using UnityEngine;

namespace MyScriptableObjectClass
{
    [CreateAssetMenu(menuName = "ScriptableObject/ObstacleData")]
    [System.Serializable]
    public class ObstacleData : ScriptableObject
    {
        //コピー用Prefab、Generatorのみが持つデータ、modelが持つ必要はない。
        [SerializeField] GameObject _obstacle;
        //Gamepresenterに処理させるために公開するデータ、Presenterが持つ。
        [SerializeField] ObstaclePublicInfo _obstacleInfo;
        //GameObjectのTransformとリンクさせるデータ、Modelのみが持てばよい
        [SerializeField, Range(0f, 1f)] float _xMoveArea;
        [SerializeField] float _xMoveSpeed;
        [SerializeField] float _yMoveSpeed;

        public GameObject Obstacle => _obstacle;
        public ObstaclePublicInfo ObstacleInfo => _obstacleInfo;
        public float XMoveArea => _xMoveArea;
        public float XMoveSpeed => _xMoveSpeed;
        public float YMoveSpeed => _yMoveSpeed;
    }

    [System.Serializable]
    public class ObstaclePublicInfo
    {
        //GeneratorのRelease関数呼び出しで出現させたい。
        [SerializeField] GameObject _destroyEffect;
        [SerializeField] ItemType _itemType;
        [SerializeField] float _hitRange;
        [SerializeField] float _score;

        public GameObject DestroyEffect => _destroyEffect;
        public ItemType ItemType => _itemType;
        public float HitRange => _hitRange;
        public float Score => _score;
    }

}


