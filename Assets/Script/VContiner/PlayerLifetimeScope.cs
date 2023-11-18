using UniRx.Triggers;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;
public class PlayerLifetimeScope : LifetimeScope
{

    [SerializeField] GamePresenter _gamePresenter;
    [Header("PlayerModel")]
    [SerializeField] Transform _playerTransform;
    [SerializeField] float _defaultSpeed = 500f;
    [Header("PlayerView")]
    [SerializeField] Animator _animator;
    [Header("ObstacleGenerator")]
    [SerializeField] GameObject _obstacle;
    [SerializeField] GameObject _explotionEffect;
    [SerializeField] float _obstacleMakeDistance = 300f;
    [SerializeField] float _yFrameOut = 100f;
    [SerializeField] float _hitRange = 20f;
    [SerializeField] Transform _obstacleParent;
    [Header("ObstacleModel")]
    [SerializeField] ObstacleType _itemType;
    [SerializeField, Range(0f, 1f)] float _xMoveRangeRate;
    [SerializeField] float _xMoveSpeed;
    [SerializeField] float _yMoveSpeed;
    [SerializeField] float _score;
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
        builder.RegisterEntryPoint<ObstacleGenerator>(Lifetime.Singleton).AsSelf()
            .WithParameter("obstacle", _obstacle)
            .WithParameter("explotionEffect", _explotionEffect)
            .WithParameter("obstacleMakeDistance" ,_obstacleMakeDistance)
            .WithParameter("yFrameOut", _yFrameOut)
            .WithParameter("hitRange", _hitRange)
            .WithParameter("parentTransform", _obstacleParent);
        builder.Register<ObstaclePresenter>(Lifetime.Transient).AsSelf();
        builder.Register<ObstacleModel>(Lifetime.Transient).AsSelf()
            .WithParameter("itemType",_itemType )
            .WithParameter("xMoveRangeRate", _xMoveRangeRate)
            .WithParameter("xMoveSpeed" ,_xMoveSpeed)
            .WithParameter("yMoveSpeed" ,_yMoveSpeed)
            .WithParameter("score" , _score);
    }
}
