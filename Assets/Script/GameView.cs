using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SocialPlatforms.Impl;

[System.Serializable]
public class GameView
{
    [SerializeField] BackGroundScroller _background;
    [SerializeField] Text _scoreText;
    [SerializeField] GameObject _resultUIGroup;
    [SerializeField] Text _resultScoreText;
    [SerializeField] float _countUpTime = 1.0f;
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
