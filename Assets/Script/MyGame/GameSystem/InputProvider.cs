using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using VContainer;
using UniRx;
public class InputProvider : SingletonMonobehavior<InputProvider> 
{
    [Inject] IPlayerPresenter _playerPresenter;
    [Inject] IPauseManager _pauseManager;

    protected override void Initialize()
    {
        //入力の監視
        this.UpdateAsObservable()
             .Select(x => Input.GetAxis("Horizontal"))
             .Subscribe(x => { _playerPresenter.SetInputX(x); })
             .AddTo(this);

        this.UpdateAsObservable()
             .Where(x => Input.GetMouseButtonDown(0))
             .Subscribe(x => _pauseManager.OnPause())
             .AddTo(this);
    }
}
