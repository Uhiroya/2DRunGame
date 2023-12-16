using MyScriptableObjectClass;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    [Header("GameSettings")] [SerializeField]
    private GameSettings _gameSettings;

    [Header("InputProvider")] [SerializeField]
    private InputProvider _inputProvider;

    [Header("AudioManager")] [SerializeField]
    private AudioSource _audioSESource;

    [SerializeField] private AudioSource _audioBGMSource;

    [Header("GameView")] [SerializeField] private Button _startButton;

    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private AnimationClip _titleAnimation;
    [SerializeField] private AnimationClip _resultFadeAnimation;
    [SerializeField] private AnimationClip _resultEmphasisAnimation;
    [SerializeField] private Text _scoreText;
    [SerializeField] private GameObject _resultUIGroup;
    [SerializeField] private Text _highScoreText;
    [SerializeField] private Text _resultScoreText;
    [SerializeField] private GameObject _pauseUI;

    [Header("_backGroundController")] [SerializeField]
    private RawImage _backGroundImage;

    [Header("PlayerModel")] [SerializeField]
    private Transform _playerTransform;

    [Header("PlayerView")] [SerializeField]
    private AnimationClip _deadAnimation;

    [SerializeField] private Animator _animator;

    [Header("ObstacleGenerator")] [SerializeField]
    private Transform _obstacleParent;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_inputProvider);
        builder.Register<AudioManager>(Lifetime.Singleton).As<IAudioManager>()
            .WithParameter("audioSetting", _gameSettings.AudioSetting)
            .WithParameter("audioSESource", _audioSESource)
            .WithParameter("audioBGMSource", _audioBGMSource);
        builder.Register<PauseManager>(Lifetime.Singleton)
            .As<IPauseManager>();
        builder.RegisterEntryPoint<GamePresenter>()
            .As<IGamePresenter>().As<IPausable>();
        builder.Register<CollisionChecker>(Lifetime.Singleton)
            .As<ICollisionChecker>()
            .WithParameter("collisionCheckerSetting", _gameSettings.CollisionCheckerSetting);
        builder.Register<GameModel>(Lifetime.Singleton)
            .As<IGameModel>()
            .WithParameter("gameModelSetting", _gameSettings.GameModelSetting);
        builder.Register<GameView>(Lifetime.Singleton)
            .As<IGameView>()
            .WithParameter("startButton", _startButton)
            .WithParameter("returnButton", _returnButton)
            .WithParameter("restartButton", _restartButton)
            .WithParameter("gameViewSetting", _gameSettings.GameViewSetting)
            .WithParameter("titleAnimationTime", _titleAnimation.length)
            .WithParameter("resultAnimationTime", _resultFadeAnimation.length + _resultEmphasisAnimation.length)
            .WithParameter("scoreText", _scoreText)
            .WithParameter("resultUIGroup", _resultUIGroup)
            .WithParameter("resultScoreText", _resultScoreText)
            .WithParameter("highScoreText", _highScoreText)
            .WithParameter("pauseUI", _pauseUI);
        builder.Register<BackGroundController>(Lifetime.Singleton)
            .As<IBackGroundController>()
            .WithParameter("image", _backGroundImage);
        builder.RegisterEntryPoint<PlayerPresenter>()
            .As<IPlayerPresenter>();
        builder.Register<PlayerModel>(Lifetime.Singleton)
            .As<IPlayerModel>()
            .WithParameter("playerModelSetting", _gameSettings.PlayerModelSetting)
            .WithParameter("playerTransform", _playerTransform);
        builder.Register<PlayerView>(Lifetime.Singleton)
            .As<IPlayerView>()
            .WithParameter("deadAnimationTime", _deadAnimation.length)
            .WithParameter("animator", _animator);
        builder.RegisterEntryPoint<ObstacleGenerator>()
            .As<IObstacleGenerator>()
            .WithParameter("parentTransform", _obstacleParent);
        builder.RegisterEntryPoint<ObstacleManager>()
            .As<IObstacleManager>().As<IPausable>()
            .WithParameter("obstacleGeneratorSetting", _gameSettings.ObstacleGeneratorSetting);
        builder.RegisterFactory<Transform, ObstacleData, Animator, IObstaclePresenter>((transform, data, animator)
            => new ObstaclePresenter(new ObstacleModel(transform, data), new ObstacleView(animator)));
    }
}
