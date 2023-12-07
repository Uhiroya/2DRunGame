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
    bool _isPause = false;
    public void OnPause()
    {
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
