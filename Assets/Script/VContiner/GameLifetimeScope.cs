using UniRx.Triggers;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;
public class GameLifetimeScope : LifetimeScope
{

    [SerializeField] GamePresenter _gamePresenter;
    [Header("PlayerModel")]
    [SerializeField] Transform _playerTransform;
    [SerializeField] float _defaultSpeed = 500f;
    [Header("PlayerView")]
    [SerializeField] Animator _animator;
    [Header("ObstacleGenerator")]
    [SerializeField] float _obstacleMakeDistance = 300f;
    [SerializeField] float _yFrameOut = 100f;
    [SerializeField] Transform _obstacleParent;
    [Header("ObstaclePresenter")]
    [SerializeField] ObstacleData _obstacleData;
    //[Header("ObstacleModel")]
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
        builder.RegisterComponent(_gamePresenter).AsSelf();
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
