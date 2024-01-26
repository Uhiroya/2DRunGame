using System.Collections.Generic;
using VContainer;

public interface IPausable
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
    [Inject] private IGamePresenter _gamePresenter;
    private bool _isPause;
    [Inject] private IEnumerable<IPausable> _pausables;

    public void OnPause()
    {
        //インゲーム中でなければポーズしない。
        if (_gamePresenter.NowGameState != GameFlowState.InGame
            && _gamePresenter.NowGameState != GameFlowState.Pause) return;
        if (!_isPause)
        {
            _isPause = true;
            foreach (var pausable in _pausables) pausable.Pause();
        }
        else
        {
            _isPause = false;
            foreach (var pausable in _pausables) pausable.Resume();
        }
    }
}
