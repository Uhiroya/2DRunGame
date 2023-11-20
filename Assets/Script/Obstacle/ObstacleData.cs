using UnityEngine;


[CreateAssetMenu(menuName = "ScriptableObject/ObstacleData")]
[System.Serializable]
public class ObstacleData : ScriptableObject
{
    [SerializeField] GameObject _obstacle;
    [SerializeField] GameObject _destroyEffect;
    [SerializeField] float _hitRange;
    [SerializeField] float _score;
    [SerializeField] ObstacleParam _param;
    public GameObject Obstacle => _obstacle;
    public GameObject DestroyEffect => _destroyEffect;
    public float HitRange => _hitRange;
    public float Score => _score;
    public ObstacleParam Param => _param;
}

[System.Serializable]
public class ObstacleParam 
{
    [SerializeField] ObstacleType _itemType;
    [SerializeField, Range(0f, 1f)] float _xMoveArea;
    [SerializeField] float _xMoveSpeed;
    [SerializeField] float _yMoveSpeed;
    public ObstacleType ItemType => _itemType;
    public float XMoveArea => _xMoveArea;
    public float XMoveSpeed => _xMoveSpeed;
    public float YMoveSpeed => _yMoveSpeed;
}


