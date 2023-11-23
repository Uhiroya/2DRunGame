using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyScriptableObjectClass
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameSettings")]
    [System.Serializable]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] GameModelSetting _gameModelSetting;
        [SerializeField] GameViewSetting _gameViewSetting;
        [SerializeField] PlayerModelSetting _playerModelSetting;
        [SerializeField] ObstacleGeneratorSetting _obstacleGeneratorSetting;
        public GameModelSetting GameModelSetting => _gameModelSetting;
        public GameViewSetting GameViewSetting => _gameViewSetting;
        public PlayerModelSetting PlayerModelSetting => _playerModelSetting;
        public ObstacleGeneratorSetting ObstacleGeneratorSetting => _obstacleGeneratorSetting;
    }
    [System.Serializable]
    public class GameModelSetting
    {
        [SerializeField] float _scoreRatePerSecond;
        [SerializeField] float _speedUpRate;
        public float ScoreRatePerSecond => _scoreRatePerSecond;
        public float SpeedUpRate => _speedUpRate;
    }
    [System.Serializable]
    public class GameViewSetting
    {
        [SerializeField] float _resultScoreCountUpTime;
        public float ResultScoreCountUpTime => _resultScoreCountUpTime;
    }
    [System.Serializable]
    public class PlayerModelSetting
    {
        [SerializeField] float _playerHitRange;
        [SerializeField] float _playerDefaultSpeed;
        public float PlayerHitRange => _playerHitRange;
        public float PlayerDefaultSpeed => _playerDefaultSpeed;
    }
    [System.Serializable]
    public class ObstacleGeneratorSetting
    {
        [SerializeField] float _obstacleMakePerDistance;
        [SerializeField] float _obstacleFrameOutRange;
        public float ObstacleMakePerDistance => _obstacleMakePerDistance;
        public float ObstacleFrameOutRange => _obstacleFrameOutRange;
    }
}

