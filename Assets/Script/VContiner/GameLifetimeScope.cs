using UniRx.Triggers;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;
using UnityEngine.UI;
public class GameLifetimeScope : LifetimeScope
{
    [Header("GameManager")]
    [SerializeField] GameManager _gameManager;

    [Header("GamePresenter")]
    [SerializeField] float _scoreRatePerSecond = 30f;
    [SerializeField] float _speedUpRate = 0.01f;

    [Header("GameView")]
    [SerializeField] Text _scoreText;
    [SerializeField] GameObject _resultUIGroup;
    [SerializeField] Text _resultScoreText;
    [SerializeField] float _countUpTime = 1.0f;

    [Header("_backGroundController")]
    [SerializeField] RawImage _backGroundImage;

    [Header("PlayerModel")]
    [SerializeField] Transform _playerTransform;
    [SerializeField] float _defaultSpeed = 500f;

    [Header("PlayerView")]
    [SerializeField] Animator _animator;

    [Header("ObstacleGenerator")]
    [SerializeField] Transform _obstacleParent;
    [SerializeField] float _obstacleMakeDistance = 300f;
    [SerializeField] float _yFrameOut = 100f;
    

    [Header("ObstaclePresenter")]
    [SerializeField] ObstacleData _obstacleData;
    ObstacleParam _obstacleParam => _obstacleData.Param;
    private void Start()
    {
        var _iPlayerPresenter = Container.Resolve<IPlayerPresenter>();
        //“ü—Í‚ÌŠÄŽ‹
        this.UpdateAsObservable()
             .Where(x => _iPlayerPresenter.PlayerState.Value == PlayerCondition.Alive)
             .Select(x => Input.GetAxis("Horizontal"))
             .Subscribe(x => { _iPlayerPresenter.Move(x);})
             .AddTo(this);
    }
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_gameManager);
        builder.RegisterEntryPoint<GamePresenter>(Lifetime.Singleton).AsSelf().As<IGamePresenter>()
            .WithParameter("parentTransform", _obstacleParent)
            .WithParameter("scoreRatePerSecond", _scoreRatePerSecond)
            .WithParameter("speedUpRate", _speedUpRate);
        builder.Register<GameModel>(Lifetime.Singleton).AsSelf().As<IGameModel>();
        builder.Register<GameView>(Lifetime.Singleton).AsSelf().As<IGameView>()
            .WithParameter("scoreText", _scoreText)
            .WithParameter("resultUIGroup", _resultUIGroup)
            .WithParameter("resultScoreText", _resultScoreText)
            .WithParameter("countUpTime", _countUpTime);
        builder.Register<BackGroundController>(Lifetime.Singleton).AsSelf().As<IBackGroundController>()
            .WithParameter("image", _backGroundImage);
        builder.RegisterEntryPoint< PlayerPresenter>(Lifetime.Singleton).AsSelf().As<IPlayerPresenter>();
        builder.Register<PlayerModel>(Lifetime.Singleton).As<IPlayerModel>()
            .WithParameter("transform" , _playerTransform)
            .WithParameter("defaultSpeed" , _defaultSpeed);
        builder.Register<PlayerView>(Lifetime.Singleton).As<IPlayerView>()
            .WithParameter("animator", _animator);
        builder.RegisterEntryPoint<ObstacleGenerator>(Lifetime.Singleton).AsSelf().As<IObstacleGenerator>()
            .WithParameter("obstacleMakeDistance", _obstacleMakeDistance)
            .WithParameter("yFrameOut", _yFrameOut)
            .WithParameter("parentTransform", _obstacleParent);
        builder.Register<ObstaclePresenter>(Lifetime.Transient).AsSelf().As<IObstaclePresenter>()
            .WithParameter("obstacleData", _obstacleData);
        builder.Register<ObstacleModel>(Lifetime.Transient).AsSelf().As<IObstacleModel>()
            .WithParameter("obstacleParam", _obstacleParam);
    }
}
