using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyScriptableObjectClass
{
    [CreateAssetMenu(menuName = "ScriptableObject/GameSettings")]
    [Serializable]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private MyAudioSetting _audioSetting;
        [SerializeField] private GameModelSetting _gameModelSetting;
        [SerializeField] private GameViewSetting _gameViewSetting;
        [SerializeField] private CollisionCheckerSetting _collisionCheckerSetting;
        [SerializeField] private PlayerModelSetting _playerModelSetting;
        [SerializeField] private ObstacleGeneratorSetting _obstacleGeneratorSetting;
        public MyAudioSetting AudioSetting => _audioSetting;
        public GameModelSetting GameModelSetting => _gameModelSetting;
        public GameViewSetting GameViewSetting => _gameViewSetting;
        public CollisionCheckerSetting CollisionCheckerSetting => _collisionCheckerSetting;
        public PlayerModelSetting PlayerModelSetting => _playerModelSetting;
        public ObstacleGeneratorSetting ObstacleGeneratorSetting => _obstacleGeneratorSetting;
    }

    [Serializable]
    public class MyAudioSetting
    {
        [SerializeField] private AudioData _audioData;
        public AudioData AudioData => _audioData;
    }

    [Serializable]
    public class GameModelSetting
    {
        [SerializeField] private float _startSpeed;
        [SerializeField] private float _speedUpRate;
        [SerializeField] private float _scoreRatePerSecond;
        public float StartSpeed => _startSpeed;
        public float SpeedUpRate => _speedUpRate;
        public float ScoreRatePerSecond => _scoreRatePerSecond;
    }

    [Serializable]
    public class GameViewSetting
    {
        [SerializeField] private float _resultScoreCountUpTime;
        public float ScoreCountUpTime => _resultScoreCountUpTime;
    }

    [Serializable]
    public class CollisionCheckerSetting
    {
        [SerializeField] private float _yFrameOut;
        public float YFrameOut => _yFrameOut;
    }

    [Serializable]
    public class PlayerModelSetting
    {
        [SerializeField] private float _playerHitRange;
        [SerializeField] private float _playerDefaultSpeed;
        public float PlayerHitRange => _playerHitRange;
        public float PlayerDefaultSpeed => _playerDefaultSpeed;
    }

    [Serializable]
    public class ObstacleGeneratorSetting
    {
        [SerializeField] private List<ObstacleData> _obstacleObjects;
        [SerializeField] private float _obstacleMakePerDistance;
        [SerializeField] private float _obstacleSetYPosition;
        public List<ObstacleData> ObstacleDataSet => _obstacleObjects;
        public float ObstacleMakePerDistance => _obstacleMakePerDistance;
        public float ObstacleSetYPosition => _obstacleSetYPosition;
    }
}
