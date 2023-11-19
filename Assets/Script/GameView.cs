using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.UIElements;

public interface IGameView
{
    void ManualUpdate(float deltaTime);

    void SetUVSpeed(float speed);

    void SetScore(float currentScore);

    void ShowResultUI();

    void ShowResultScore(float score);
}
public class GameView : IGameView
{
    IBackGroundController _background;
    Text _scoreText;
    GameObject _resultUIGroup;
    Text _resultScoreText;
    float _countUpTime = 1.0f;
    public GameView(IBackGroundController background , Text scoreText , GameObject resultUIGroup
        ,Text resultScoreText, float countUpTime)
    {
        _background = background;
        _scoreText = scoreText; 
        _resultUIGroup = resultUIGroup;
        _resultScoreText = resultScoreText;
        _countUpTime = countUpTime;
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
    public void ShowResultScore(float score)
    {
        //カウントアップ処理
        DOVirtual.Float(
            0f,
            score,
            _countUpTime,
            value => _resultScoreText.text = value.ToString("00000000")
        );
    }
}
