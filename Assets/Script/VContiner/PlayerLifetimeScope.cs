using UniRx.Triggers;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;
public class PlayerLifetimeScope : LifetimeScope
{
    [SerializeField] GamePresenter _gamePresenter;
    [SerializeField] Rigidbody2D _rb;
    [SerializeField] Collider2D _col;
    [SerializeField] float _defaultSpeed = 500f;
    [SerializeField] Animator _animator;
    private void Start()
    {
        var _iPlayerPresenter = Container.Resolve<IPlayerPresenter>();
        //“ü—Í‚ÌŠÄŽ‹
        this.UpdateAsObservable()
             .Where(x => _iPlayerPresenter.PlayerState.Value == PlayerCondition.Alive)
             .Select(x => Input.GetAxis("Horizontal"))
             .Subscribe(x => { _iPlayerPresenter.Move(x); Debug.Log(x); })
             .AddTo(this);
    }
    protected override void Configure(IContainerBuilder builder)
    {
        builder.RegisterComponent(_gamePresenter);
        builder.RegisterEntryPoint< PlayerPresenter>(Lifetime.Singleton).As<IPlayerPresenter>();
        builder.Register<PlayerModel>(Lifetime.Singleton).As<IPlayerModel>()
            .WithParameter(_rb)
            .WithParameter(_col)
            .WithParameter(_defaultSpeed);
        builder.Register<PlayerView>(Lifetime.Singleton).As<IPlayerView>()
            .WithParameter(_animator);

    }
}
