using UnityEngine;
using VContainer;
/// <summary>
/// ボタンイベントからのみ呼び出される。
/// </summary>
public class GameManager : SingletonMonobehavior<GameManager>
{
    [Inject] IGamePresenter _gamePresenter;
    protected override void Initialize() { }
    public void GoTitle()
        => _gamePresenter.PressReturnButton();
    public void GameStart()
        => _gamePresenter.PressStartButton();
}
