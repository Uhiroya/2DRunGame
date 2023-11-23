using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.UIElements;
using Cysharp.Threading.Tasks;
using MyScriptableObjectClass;
public interface IGameView
{
    UniTask TitleStart();
    void ManualUpdate(float deltaTime);

    void SetUVSpeed(float speed);

    void SetScore(float currentScore);

    void ShowResultUI();

    UniTaskVoid ShowResultScore(float score);
}
public class GameView : IGameView
{
    float _titleAnimationTime;
    float _resultAnimationTime;
    GameViewSetting _gameViewSetting;
    IBackGroundController _background;
    Text _scoreText;
    GameObject _resultUIGroup;
    Text _resultScoreText;
    
   
    public GameView(GameViewSetting gameViewSetting ,float titleAnimationTime, float resultAnimationTime 
        ,IBackGroundController background , Text scoreText , GameObject resultUIGroup
        ,Text resultScoreText)
    {
        _gameViewSetting = gameViewSetting;
        _titleAnimationTime = titleAnimationTime;
        _resultAnimationTime = resultAnimationTime;
        _background = background;
        _scoreText = scoreText; 
        _resultUIGroup = resultUIGroup;
        _resultScoreText = resultScoreText;
    }
    public async UniTask TitleStart()
    {
        await UniTask.Delay((int)(_titleAnimationTime * 1000));
    }
    public void ManualUpdate(float deltaTime)
    {
        _background.ManualUpdate(deltaTime);
    }
    public void SetUVSpeed(float speed)
    {
        _background.SetUVSpeed(speed);
    }
    public void SetScore(float currentScore)
    {
        _scoreText.text = currentScore.ToString("00000000");
    }
    public void ShowResultUI()
    {
        _resultUIGroup.SetActive(true);
    }
    public async UniTaskVoid ShowResultScore(float score)
    {
        await UniTask.Delay((int)(_resultAnimationTime * 1000));
        //カウントアップ処理
        await DOVirtual.Float(
            0f,
            score,
            _gameViewSetting.ResultScoreCountUpTime,
            value => _resultScoreText.text = value.ToString("00000000")
        );
    }
}
