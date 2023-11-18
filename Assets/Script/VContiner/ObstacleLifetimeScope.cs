//using UniRx.Triggers;
//using UnityEngine;
//using VContainer;
//using VContainer.Unity;
//using UniRx;
//public class ObstacleLifetimeScope : LifetimeScope
//{
//    //[SerializeField] GamePresenter _gamePresenter;
//    [SerializeField] GameObject _obstacle;
//    [SerializeField] GameObject _explotionEffect;
//    [SerializeField] float _obstacleMakeDistance = 15f;
//    [SerializeField] float _yFrameOut = 20f;
//    private void Start()
//    {

//    }
//    protected override void Configure(IContainerBuilder builder)
//    {
//        //builder.RegisterComponent(_gamePresenter);
//        //builder.RegisterEntryPoint<PlayerPresenter>(Lifetime.Singleton).As<IPlayerPresenter>();
//        builder.RegisterEntryPoint<ObstacleGenerator>(Lifetime.Singleton).AsSelf()
//            .WithParameter(_obstacle)
//            .WithParameter(_explotionEffect)
//            .WithParameter(_obstacleMakeDistance)
//            .WithParameter(_yFrameOut)
//            .WithParameter(this.transform);
//            //.WithParameter(_gamePresenter);

//    }
//}
