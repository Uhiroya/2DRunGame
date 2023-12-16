using UniRx;
using UniRx.Triggers;
using UnityEngine;
using VContainer;

public class InputProvider : SingletonMonobehavior<InputProvider>
{
    [Inject] private IPauseManager _pauseManager;
    [Inject] private IPlayerPresenter _playerPresenter;

    protected override void Initialize()
    {
        //入力の監視
        this.UpdateAsObservable()
            .Select(_ => Input.GetAxis("Horizontal"))
            .Subscribe(x => { _playerPresenter.SetInputX(x); })
            .AddTo(this);

        this.UpdateAsObservable()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ => _pauseManager.OnPause())
            .AddTo(this);
    }
}
