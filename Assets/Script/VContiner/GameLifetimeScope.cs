using UniRx.Triggers;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;
using UnityEngine.UI;
using MyScriptableObjectClass;
public class GameLifetimeScope : LifetimeScope
{
    [Header("GameSettings")]
    [SerializeField] GameSettings _gameSettings;

    [Header("InputProvider")]
    [SerializeField] InputProvider _inputProvider;

    [Header("AudioManager")]
    [SerializeField] AudioSource _audioSESource;
    [SerializeField] AudioSource _audioBGMSource;

    [Header("GameView")]
    [SerializeField] Button _startButton;
    [SerializeField] Button _returnButton;
    [SerializeField] Button _restartButton;
    [SerializeField] AnimationClip _titleAnimation;
    [SerializeField] AnimationClip _resultFadeAnimation;
    [SerializeField] AnimationClip _resultEmphasisAnimation;
    [SerializeField] Text _scoreText;
    [SerializeField] GameObject _resultUIGroup;
    [SerializeField] Text _highScoreText;
    [SerializeField] Text _resultScoreText;
    [SerializeField] GameObject _pauseUI;

    [Header("_backGroundController")]
    [SerializeField] RawImage _backGroundImage;

    [Header("PlayerModel")]
    [SerializeField] Transform _playerTransform;

    [Header("PlayerView")]
    [SerializeField] AnimationClip _deadAnimation;
    [SerializeField] Animator _animator;

    [Header("ObstacleGenerator")]
    [SerializeField] Transform _obstacleParent;
    
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_inputProvider);
        builder.Register<AudioManager>(Lifetime.Singleton).As<IAudioManager>()
            .WithParameter("audioSetting", _gameSettings.AudioSetting)
            .WithParameter("audioSESource", _audioSESource)
            .WithParameter("audioBGMSource", _audioBGMSource);
        builder.Register<PauseManager>(Lifetime.Singleton)
            .As<IPauseManager>();
        builder.RegisterEntryPoint<GamePresenter>(Lifetime.Singleton)
            .As<IGamePresenter>().As<IPauseable>();
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
        builder.RegisterEntryPoint< PlayerPresenter>(Lifetime.Singleton)
            .As<IPlayerPresenter>();
        builder.Register<PlayerModel>(Lifetime.Singleton)
            .As<IPlayerModel>()
            .WithParameter("playerModelSetting", _gameSettings.PlayerModelSetting)
            .WithParameter("playerTransform", _playerTransform);
        builder.Register<PlayerView>(Lifetime.Singleton)
            .As<IPlayerView>()
            .WithParameter("deadAnimationTime", _deadAnimation.length)
            .WithParameter("animator", _animator);
        builder.RegisterEntryPoint<ObstacleGenerator>(Lifetime.Singleton)
            .As<IObstacleGenerator>()
            .WithParameter("parentTransform", _obstacleParent);
        builder.RegisterEntryPoint<ObstacleManager>(Lifetime.Singleton)
            .As<IObstacleManager>().As<IPauseable>()
            .WithParameter("obstacleGeneratorSetting", _gameSettings.ObstacleGeneratorSetting);
        builder.RegisterFactory<Transform ,ObstacleData, IObstaclePresenter>((transform ,data) => new ObstaclePresenter(new ObstacleModel(transform ,data), new ObstacleView()));
        //builder.RegisterFactory<ObstacleData, IObstaclePresenter>((container) =>
        //{
        //    var view = container.Resolve<IObstacleView>();
        //    return data =>
        //    {
        //        return new ObstaclePresenter(new ObstacleModel(data), view);
        //    };
        //} , Lifetime.Transient);
    }
}
