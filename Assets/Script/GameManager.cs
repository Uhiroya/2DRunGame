using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Playables;
using VContainer;
using VContainer.Unity;


public class GameManager : MonoBehaviour
{
    [Inject] IGamePresenter _gamePresenter;
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ShowResultScore()
        => _gamePresenter.ShowResultScore();
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ChangeStateToTitle()
        => _gamePresenter.ChangeStateToTitle();
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ChangeStateToInGame()
        => _gamePresenter.ChangeStateToInGame();
    /// <summary>アニメーションイベントから呼び出される。</summary>
    public void ChangeStateToResult()
        => _gamePresenter.ChangeStateToResult();
}
