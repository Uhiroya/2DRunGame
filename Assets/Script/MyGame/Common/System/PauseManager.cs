using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public interface IPauseable
{
    void Pause();
    void Resume();
}
public interface IPauseManager
{
    void OnPause();
}
public class PauseManager : IPauseManager
{
    [Inject] IEnumerable<IPauseable> _pauseables;
    [Inject] IGamePresenter _gamePresenter;
    bool _isPause = false;
    public void OnPause()
    {
        //インゲーム中でなければポーズしない。
        if ((_gamePresenter.NowGameState != GameFlowState.InGame 
                && _gamePresenter.NowGameState != GameFlowState.Pause)) return;
        if (!_isPause)
        {
            _isPause = true;
            foreach (IPauseable pausable in _pauseables)
            {
                pausable.Pause();
            }
        }
        else
        {
            _isPause = false;
            foreach (IPauseable pausable in _pauseables)
            {
                pausable.Resume();
            }
        }
    }
}
