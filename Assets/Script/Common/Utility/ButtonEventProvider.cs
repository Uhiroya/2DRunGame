using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Playables;
using VContainer;
using VContainer.Unity;


public class ButtonEventProvider : MonoBehaviour
{
    [Inject] IGamePresenter _gamePresenter;

    /// <summary>bottonから呼び出される。</summary>
    public void GoTitle()
        => _gamePresenter.GoTitle();
    /// <summary>bottonから呼び出される。</summary>
    public void GameStart()
        => _gamePresenter.GameStart();

}
