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
    [Header("GameManager")]
    [SerializeField] GameManager _gameManager;

    [Header("GameView")]
    [SerializeField] AnimationClip _titleAnimation;
    [SerializeField] AnimationClip _resultFadeAnimation;
    [SerializeField] AnimationClip _resultEmphasisAnimation;
    [SerializeField] Text _scoreText;
    [SerializeField] GameObject _resultUIGroup;
    [SerializeField] Text _highScoreText;
    [SerializeField] Text _resultScoreText;

    [Header("_backGroundController")]
    [SerializeField] RawImage _backGroundImage;

    [Header("PlayerModel")]
    [SerializeField] Transform _playerTransform;

    [Header("PlayerView")]
    [SerializeField] AnimationClip _deadAnimation;
    [SerializeField] Animator _animator;

    [Header("ObstacleGenerator")]
    [SerializeField] Transform _obstacleParent;
    
    [Header("ObstaclePresenter")]
    [SerializeField] ObstacleData _obstacleData;
    ObstacleParam _obstacleParam => _obstacleData.Param;
    private void Start()
    {
        var _iPlayerPresenter = Container.Resolve<IPlayerPresenter>();
        //入力の監視
        this.UpdateAsObservable()
             .Where(x => _iPlayerPresenter.PlayerState.Value == PlayerCondition.Alive)
             .Select(x => Input.GetAxis("Horizontal"))
             .Subscribe(x => { _iPlayerPresenter.SetInputX(x);})
             .AddTo(this);
        
    }
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_gameManager);
        builder.RegisterEntryPoint<GamePresenter>(Lifetime.Singleton).AsSelf().As<IGamePresenter>()
            .WithParameter("parentTransform", _obstacleParent);
        builder.Register<GameModel>(Lifetime.Singleton).AsSelf().As<IGameModel>()
             .WithParameter("gameModelSetting", _gameSettings.GameModelSetting);
        builder.Register<GameView>(Lifetime.Singleton).AsSelf().As<IGameView>()
            .WithParameter("gameViewSetting", _gameSettings.GameViewSetting)
            .WithParameter("titleAnimationTime", _titleAnimation.length)
            .WithParameter("resultAnimationTime", _resultFadeAnimation.length + _resultEmphasisAnimation.length)
            .WithParameter("scoreText", _scoreText)
            .WithParameter("resultUIGroup", _resultUIGroup)
            .WithParameter("resultScoreText", _resultScoreText)       
            .WithParameter("highScoreText", _highScoreText);       
        builder.Register<BackGroundController>(Lifetime.Singleton).AsSelf().As<IBackGroundController>()
            .WithParameter("image", _backGroundImage);
        builder.RegisterEntryPoint< PlayerPresenter>(Lifetime.Singleton).AsSelf().As<IPlayerPresenter>();
        builder.Register<PlayerModel>(Lifetime.Singleton).As<IPlayerModel>()
            .WithParameter("playerModelSetting", _gameSettings.PlayerModelSetting)
            .WithParameter("playerTransform", _playerTransform);
        builder.Register<PlayerView>(Lifetime.Singleton).As<IPlayerView>()
            .WithParameter("deadAnimationTime", _deadAnimation.length)
            .WithParameter("animator", _animator);
        builder.RegisterEntryPoint<ObstacleGenerator>(Lifetime.Singleton).AsSelf().As<IObstacleGenerator>()
            .WithParameter("obstacleGeneratorSetting", _gameSettings.ObstacleGeneratorSetting)
            .WithParameter("parentTransform", _obstacleParent);
        //builder.RegisterEntryPoint<ObstaclePresenter>(Lifetime.Transient).AsSelf().As<IObstaclePresenter>();
        //    .WithParameter("obstacleData", _obstacleData);
        //builder.Register<ObstacleModel>(Lifetime.Transient).AsSelf().As<IObstacleModel>()
        //    .WithParameter("obstacleParam", _obstacleParam);
        builder.RegisterFactory<ObstacleParam, IObstacleModel>(parm => new ObstacleModel(parm));

        builder.RegisterFactory<ObstacleData, IObstaclePresenter>(data => new ObstaclePresenter(data));
    }
}
