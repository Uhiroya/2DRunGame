using System.Collections;
using System.Collections.Generic;
using UniRx.Triggers;
using UnityEngine;
using VContainer;
using UniRx;
public class InputProvider : SingletonMonobehavior<InputProvider> 
{
    [Inject] IGamePresenter _gamePresenter;
    [Inject] IPlayerPresenter _playerPresenter;
    [Inject] IPauseManager _pauseManager;

    protected override void Initialize()
    {
        this.UpdateAsObservable()
             .Where(_ => _gamePresenter.NowGameState == GameFlowState.InGame)
             .Select(x => Input.GetAxis("Horizontal"))
             .Subscribe(x => { _playerPresenter.SetInputX(x); })
             .AddTo(this);
        //ゲーム進行中でしかPauseを呼ばない様にこの部分で制約をかけているが、
        //利用者はこの実装を知らないとIPausableを利用した実装が行いにくいため良くない気がする。
        this.UpdateAsObservable()
             .Where(_ => _gamePresenter.NowGameState == GameFlowState.InGame 
                        || _gamePresenter.NowGameState == GameFlowState.Pause)
             .Where(x => Input.GetMouseButtonDown(0))
             .Subscribe(x => _pauseManager.OnPause())
             .AddTo(this);
    }
}
