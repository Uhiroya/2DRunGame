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
    /// <summary>�A�j���[�V�����C�x���g����Ăяo�����B</summary>
    public void ShowResultScore()
        => _gamePresenter.ShowResultScore();
    /// <summary>�A�j���[�V�����C�x���g����Ăяo�����B</summary>
    public void ChangeStateToTitle()
        => _gamePresenter.ChangeStateToTitle();
    /// <summary>�A�j���[�V�����C�x���g����Ăяo�����B</summary>
    public void ChangeStateToInGame()
        => _gamePresenter.ChangeStateToInGame();
    /// <summary>�A�j���[�V�����C�x���g����Ăяo�����B</summary>
    public void ChangeStateToResult()
        => _gamePresenter.ChangeStateToResult();
}
